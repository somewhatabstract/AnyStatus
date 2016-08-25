using AnyStatus.Features.Edit;
using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using AnyStatus.ViewModels;
using AnyStatus.Views;
using FluentScheduler;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;

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
            container.Register<NewWindow>().AsMultiInstance();
            container.Register<NewViewModel>().AsMultiInstance();
            container.Register<EditWindow>().AsMultiInstance();
            container.Register<EditViewModel>().AsMultiInstance();

            //handlers
            container.Register<IHandler<JenkinsBuild>, JenkinsBuildHandler>().AsMultiInstance();
            container.Register<IHandler<HttpStatus>, HttpStatusHandler>().AsMultiInstance();
            container.Register<IHandler<Ping>, PingHandler>().AsMultiInstance();
            container.Register<IHandler<TcpPort>, TcpPortHandler>().AsMultiInstance();
            container.Register<IHandler<TeamCityBuild>, TeamCityBuildHandler>().AsMultiInstance();
            container.Register<IHandler<AppVeyorBuild>, AppVeyorBuildHandler>().AsMultiInstance();

            //templates
            container.Register<IEnumerable<Template>>((c, p) =>
            {
                return new List<Template>(){
                    new Template("Ping", new Ping()),
                    new Template("TCP Port", new TcpPort()),
                    new Template("HTTP Status", new HttpStatus()),
                    new Template("Jenkins Build", new JenkinsBuild()),
                    new Template("TeamCity Build", new TeamCityBuild()),
                    new Template("AppVeyor Build", new AppVeyorBuild()),
                };
            });

            return container;
        }
    }
}
