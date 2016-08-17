using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using AnyStatus.ViewModels;
using AnyStatus.Views;
using FluentScheduler;
using Microsoft.VisualStudio.Shell;
using System;

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

            container.Register<Package>(_package);
            container.Register<IServiceProvider>(_package);
            container.Register<ToolWindowCommand>().AsSingleton();
            container.Register<IUserSettings, UserSettings>().AsSingleton();
            container.Register<ILogger, Logger>().AsSingleton();
            container.Register<Registry, ItemRegistry>().AsSingleton();

            //views
            container.Register<IViewLocator, ViewLocator>().AsSingleton();

            container.Register<ToolWindowControl>().AsSingleton();
            container.Register<ToolWindowViewModel>().AsSingleton();

            container.Register<NewStatusDialog>().AsMultiInstance();
            container.Register<NewStatusViewModel>().AsMultiInstance();

            //handlers
            container.Register<IHandler<JenkinsBuild>, JenkinsJobHandler>().AsMultiInstance();
            container.Register<IHandler<HttpStatus>, HttpStatusHandler>().AsMultiInstance();

            return container;
        }
    }
}
