using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.ViewModels;
using AnyStatus.Views;
using Microsoft.VisualStudio.Shell;
using TinyIoC;
using System;

namespace AnyStatus.VSPackage
{
    internal class ContainerBuilder
    {
        internal TinyIoCContainer Build(Package package)
        {
            var container = TinyIoCContainer.Current;

            container.Register<Package>(package);
            container.Register<IServiceProvider>(package);
            container.Register<ToolWindowCommand>().AsSingleton();
            container.Register<IUserSettings, UserSettings>().AsSingleton();
            container.Register<ILogger, Logger>().AsSingleton();

            //views
            container.Register<IViewLocator, ViewLocator>();

            container.Register<ToolWindowControl>().AsSingleton();
            container.Register<ToolWindowViewModel>().AsSingleton();

            container.Register<NewStatusDialog>().AsMultiInstance();
            container.Register<NewStatusViewModel>().AsMultiInstance();

            return container;
        }
    }
}
