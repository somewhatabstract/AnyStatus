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

            ScanAndRegisterHandlers();

            //templates
            container.Register<IEnumerable<Template>>((c, p) =>
            {
                return new List<Template>(){
                    new Template("Ping", new Ping()),
                    new Template("TCP Port", new TcpPort()),
                    new Template("HTTP Status", new HttpStatus()),
                    new Template("Windows Service", new WindowsService()),
                    new Template("Jenkins Build", new JenkinsBuild()),
                    new Template("TeamCity Build", new TeamCityBuild()),
                    new Template("AppVeyor Build", new AppVeyorBuild()),
                    //new Template("TFS Build", new TfsBuild()),
                };
            });

            return container;
        }

        private static void ScanAndRegisterHandlers()
        {
            var assembly = typeof(Item).Assembly;
            var baseHandlerType = typeof(IHandler<>);
            var baseHandlerTypeName = baseHandlerType.Name;

            var handlerTypes = FindTypesOf(baseHandlerType, assembly);

            foreach (var handlerType in handlerTypes)
            {
                TinyIoCContainer.Current.Register(handlerType.GetInterface(baseHandlerTypeName), handlerType).AsMultiInstance();
            }
        }

        private static IEnumerable<Type> FindTypesOf(Type baseType, Assembly assembly)
        {
            return from type in assembly.GetTypes()
                   where !type.IsAbstract && !type.IsGenericTypeDefinition
                   let handlerInterfaces =
                       from iface in type.GetInterfaces()
                       where iface.IsGenericType
                       where iface.GetGenericTypeDefinition() == baseType
                       select iface
                   where handlerInterfaces.Any()
                   select type;
        }
    }
}
