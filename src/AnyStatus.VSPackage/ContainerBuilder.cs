using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using AnyStatus.ViewModels;
using AnyStatus.Views;
using FluentScheduler;
using Microsoft.VisualStudio.Shell;
using System;
using TinyIoC;

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
            container.Register<Registry, ItemRegistry>().AsSingleton();

            //views
            container.Register<IViewLocator, ViewLocator>().AsSingleton();

            container.Register<ToolWindowControl>().AsSingleton();
            container.Register<ToolWindowViewModel>().AsSingleton();

            container.Register<NewStatusDialog>().AsMultiInstance();
            container.Register<NewStatusViewModel>().AsMultiInstance();

            //handlers
            container.Register<IHandler<JenkinsJob>, JenkinsJobHandler>().AsMultiInstance();
            container.Register<IHandler<HttpStatus>, HttpStatusHandler>().AsMultiInstance();


            return container;
        }
    }
}
