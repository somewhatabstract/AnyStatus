using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace AnyStatus.VSPackage
{
    internal class ContainerBuilder
    {
        internal static TinyIoCContainer Build(AnyStatusPackage package)
        {
            var container = TinyIoCContainer.Current;

            RegisterCore(container, package);
            RegisterMenuCommands(container);
            RegisterUI(container);
            RegisterItems(container);
            RegisterTemplates(container);
            RegisterHandlers(container);

            return container;
        }

        private static void RegisterCore(TinyIoCContainer container, AnyStatusPackage package)
        {
            container.Register<IPackage>(package);
            container.Register<IInfoBarService, InfoBarService>();
            container.Register<IServiceProvider>(package);
            container.Register<AnyStatusApp>().AsSingleton();
            container.Register<ISettingsStore, SettingsStore>().AsSingleton();
            container.Register<ILogger, Logger>().AsSingleton();
            container.Register<IJobScheduler, JobScheduler>().AsSingleton();
            container.Register<IScheduledJob, ScheduledJob>().AsMultiInstance();
            container.Register<IUsageReporter, AnalyticsReporter>().AsSingleton();
            container.Register<ICommandRegistry, CommandRegistry>().AsSingleton();
            container.Register<IMediator, Mediator>().AsMultiInstance();
            container.Register<IProcessStarter, ProcessStarter>().AsMultiInstance();
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

        private static void RegisterHandlers(TinyIoCContainer container)
        {
            var baseType = typeof(IHandler);

            var handlers = from handler in TypeFinder.FindTypesOf(baseType, baseType.Assembly)
                           from @interface in handler.GetInterfaces().Where(k => k.IsGenericType)
                           where handler.IsAbstract == false
                           select new
                           {
                               Type = @interface,
                               Implementation = handler
                           };

            foreach (var handler in handlers)
            {
                container.Register(handler.Type, handler.Implementation).AsMultiInstance();
            }
        }

        private static void RegisterItems(TinyIoCContainer container)
        {
            var type = typeof(Item);

            var items = TypeFinder.FindTypesOf(type, type.Assembly);

            items = from item in items
                    where item.IsBrowsable()
                    orderby item.Name
                    select item;

            container.RegisterMultiple(typeof(Item), items);
        }

        private static void RegisterMenuCommands(TinyIoCContainer container)
        {
            var items = TypeFinder.FindTypesOf(typeof(IMenuCommand),
                            new[] {
                                typeof(IMenuCommand).Assembly,
                                typeof(ContainerBuilder).Assembly
                            });

            container.RegisterMultiple(typeof(IMenuCommand), items);
        }

        private static void RegisterTemplates(TinyIoCContainer container)
        {
            container.Register<IEnumerable<Template>>((c, p) =>
            {
                var items = c.ResolveAll<Item>();

                var templates = from item in items
                                select CreateTemplateFor(item);

                return templates.ToList();
            });
        }

        private static Template CreateTemplateFor(Item item)
        {
            var type = item.GetType();

            var nameAtt = type.GetCustomAttribute<DisplayNameAttribute>();
            var descAtt = type.GetCustomAttribute<DescriptionAttribute>();
            var columnAtt = type.GetCustomAttribute<DisplayColumnAttribute>();

            var name = string.IsNullOrEmpty(nameAtt?.DisplayName) ? type.Name : nameAtt.DisplayName;

            var group = string.IsNullOrEmpty(columnAtt?.DisplayColumn) ? "General" : columnAtt.DisplayColumn;

            return new Template(item, name, descAtt?.Description, group);
        }
    }
}
