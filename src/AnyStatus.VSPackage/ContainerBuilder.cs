using AnyStatus.Features.Edit;
using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.ViewModels;
using AnyStatus.Views;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AnyStatus.VSPackage
{
    internal class ContainerBuilder
    {
        internal static TinyIoCContainer Build(Package package)
        {
            var container = TinyIoCContainer.Current;

            RegisterCore(container, package);
            RegisterMenuCommands(container);
            RegisterUI(container);
            RegisterItems(container);
            RegisterTemplates(container);
            RegisterItemHandlers(container);

            return container;
        }

        private static void RegisterCore(TinyIoCContainer container, Package package)
        {
            container.Register(package);
            container.Register<AnyStatusApp>().AsSingleton();
            container.Register<IServiceProvider>(package);
            container.Register<IUserSettings, UserSettings>().AsSingleton();
            container.Register<ILogger, Logger>().AsSingleton();
            container.Register<IJobScheduler, JobScheduler>().AsSingleton();
            container.Register<ScheduledJob>().AsMultiInstance();
            container.Register<IUsageReporter, AnalyticsReporter>().AsSingleton();
            container.Register<ICommandRegistry, CommandRegistry>();
        }

        private static void RegisterUI(TinyIoCContainer container)
        {
            container.Register<IViewLocator, ViewLocator>().AsSingleton();

            container.Register<ToolWindowControl>().AsSingleton();
            container.Register<ToolWindowViewModel>().AsSingleton();

            container.Register<NewWindow>().AsMultiInstance();
            container.Register<NewViewModel>().AsMultiInstance();

            container.Register<EditWindow>().AsMultiInstance();
            container.Register<EditViewModel>().AsMultiInstance();

            container.Register<OptionsDialogControl>().AsSingleton();
            container.Register<OptionsViewModel>().AsSingleton();
        }

        private static void RegisterItemHandlers(TinyIoCContainer container)
        {
            var baseHandler = typeof(IHandler<>);

            var handlers = Discovery.FindGenericTypesOf(baseHandler, typeof(IHandler<>).Assembly);

            foreach (var handler in handlers)
            {
                container
                    .Register(handler.GetInterface(baseHandler.Name), handler)
                    .AsMultiInstance();
            }
        }

        private static void RegisterItems(TinyIoCContainer container)
        {
            var items = Discovery.FindTypesOf(typeof(Item), new[] { typeof(Item).Assembly });

            items = from item in items
                    where item.IsBrowsable()
                    orderby item.Name
                    select item;

            container.RegisterMultiple(typeof(Item), items);
        }

        private static void RegisterMenuCommands(TinyIoCContainer container)
        {
            var items = Discovery.FindTypesOf(typeof(IMenuCommand),
                     new[] { typeof(IMenuCommand).Assembly,
                             typeof(ContainerBuilder).Assembly });

            container.RegisterMultiple(typeof(IMenuCommand), items);
        }

        private static void RegisterTemplates(TinyIoCContainer container)
        {
            container.Register<IEnumerable<Template>>((c, p) =>
            {
                var items = c.ResolveAll<Item>();

                var templates = new List<Template>();

                foreach (var item in items)
                {
                    var type = item.GetType();

                    var nameAtt = type.GetCustomAttribute<DisplayNameAttribute>();
                    var descAtt = type.GetCustomAttribute<DescriptionAttribute>();

                    var displayName = nameAtt != null ? nameAtt.DisplayName : type.Name;

                    var template = new Template(item, displayName, descAtt?.Description);

                    templates.Add(template);
                }

                return templates;
            });
        }
    }
}
