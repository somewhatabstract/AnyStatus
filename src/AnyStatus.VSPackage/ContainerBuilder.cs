using AnyStatus.Features.Edit;
using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
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
        private readonly Package _package;

        public ContainerBuilder(Package package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            _package = package;
        }

        internal TinyIoCContainer Build()
        {
            var container = TinyIoCContainer.Current;

            //Core
            container.Register<Package>(_package);
            container.Register<IServiceProvider>(_package);
            container.Register<ToolWindowCommand>().AsSingleton();
            container.Register<IUserSettings, UserSettings>().AsSingleton();
            container.Register<ILogger, Logger>().AsSingleton();
            container.Register<IJobScheduler, JobScheduler>().AsSingleton();
            container.Register<ScheduledJob>().AsMultiInstance();
            container.Register<IUsageReporter>((c, p) =>
            {
                //todo: optimize startup
                var userSettings = c.Resolve<IUserSettings>();
                var clientId = userSettings.ClientId ?? Guid.NewGuid().ToString();
                var reporter = new AnalyticsReporter("UA-83802855-1", "AnyStatus", "AnyStatus", clientId, "0.7");

                reporter.IsEnabled = userSettings.ReportAnonymousUsage;

                return reporter;
            });

            //User Interface
            container.Register<IViewLocator, ViewLocator>().AsSingleton();

            container.Register<ToolWindowControl>().AsSingleton();
            container.Register<ToolWindowViewModel>().AsSingleton();

            container.Register<NewWindow>().AsMultiInstance();
            container.Register<NewViewModel>().AsMultiInstance();

            container.Register<EditWindow>().AsMultiInstance();
            container.Register<EditViewModel>().AsMultiInstance();

            container.Register<OptionsDialogControl>().AsSingleton();
            container.Register<OptionsViewModel>().AsSingleton();

            //Dynamic registration
            ScanAndRegisterItems();
            RegisterItemTemplates();
            ScanAndRegisterItemHandlers();

            return container;
        }

        #region Helpers

        private static void ScanAndRegisterItemHandlers()
        {
            var baseHandler = typeof(IHandler<>);

            var handlers = Discovery.FindGenericTypesOf(baseHandler, typeof(IHandler<>).Assembly);

            foreach (var handler in handlers)
            {
                TinyIoCContainer.Current
                    .Register(handler.GetInterface(baseHandler.Name), handler)
                    .AsMultiInstance();
            }
        }

        private static void ScanAndRegisterItems()
        {
            var items = Discovery.FindTypesOf(typeof(Item), typeof(Item).Assembly);

            items = from item in items
                    where item.IsBrowsable()
                    orderby item.Name
                    select item;

            TinyIoCContainer.Current.RegisterMultiple(typeof(Item), items);
        }

        private static void RegisterItemTemplates()
        {
            TinyIoCContainer.Current.Register<IEnumerable<Template>>((c, p) =>
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

        #endregion
    }
}
