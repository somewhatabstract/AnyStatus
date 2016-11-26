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

            RegisterHandlers(container, typeof(IHandler<>));
            RegisterHandlers(container, typeof(IOpenInBrowser<>));

            return container;
        }

        private static void RegisterCore(TinyIoCContainer container, Package package)
        {
            container.Register(package);
            container.Register<AnyStatusApp>().AsSingleton();
            container.Register<IServiceProvider>(package);
            container.Register<ISettingsStore, SettingsStore>().AsSingleton();
            container.Register<ILogger, Logger>().AsSingleton();
            container.Register<IJobScheduler, JobScheduler>().AsSingleton();
            container.Register<IScheduledJob, ScheduledJob>().AsMultiInstance();
            container.Register<IUsageReporter, AnalyticsReporter>().AsSingleton();

            container.Register<ICommandRegistry, CommandRegistry>();
            container.Register<IMediator, Mediator>();
            container.Register<IProcessStarter, ProcessStarter>();
        }

        private static void RegisterUI(TinyIoCContainer container)
        {
            container.Register<IDialogService, DialogService>().AsMultiInstance();

            container.Register<ToolWindowControl>().AsSingleton();
            container.Register<ToolWindowViewModel>().AsSingleton();

            container.Register<NewWindow>().AsMultiInstance();
            container.Register<NewViewModel>().AsMultiInstance();

            container.Register<EditWindow>().AsMultiInstance();
            container.Register<EditViewModel>().AsMultiInstance();

            container.Register<GeneralOptionsView>().AsSingleton();
            container.Register<GeneralOptionsViewModel>().AsSingleton();

            container.Register<UserInterfaceOptionsView>().AsSingleton();
            container.Register<UserInterfaceOptionsViewModel>().AsSingleton();
        }

        private static void RegisterHandlers(TinyIoCContainer container, Type type)
        {
            var handlers = TypeFinder.FindGenericTypesOf(type, type.Assembly);

            foreach (var handler in handlers)
            {
                container
                    .Register(handler.GetInterface(type.Name), handler)
                    .AsMultiInstance();
            }
        }

        private static void RegisterItems(TinyIoCContainer container)
        {
            var items = TypeFinder.FindTypesOf(typeof(Item), new[] { typeof(Item).Assembly });

            items = from item in items
                    where item.IsBrowsable()
                    orderby item.Name
                    select item;

            container.RegisterMultiple(typeof(Item), items);
        }

        private static void RegisterMenuCommands(TinyIoCContainer container)
        {
            var items = TypeFinder.FindTypesOf(typeof(IToolbarCommand),
                     new[] { typeof(IToolbarCommand).Assembly,
                             typeof(ContainerBuilder).Assembly });

            container.RegisterMultiple(typeof(IToolbarCommand), items);
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
