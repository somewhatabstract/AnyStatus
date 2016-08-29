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

            ScanAndRegisterItems();
            RegisterItemTemplates();
            ScanAndRegisterItemHandlers();

            return container;
        }

        private static void ScanAndRegisterItemHandlers()
        {
            var assembly = typeof(IHandler<>).Assembly;
            var baseHandlerType = typeof(IHandler<>);
            var baseHandlerTypeName = baseHandlerType.Name;

            var handlerTypes = FindGenericTypesOf(baseHandlerType, assembly);

            foreach (var handlerType in handlerTypes)
            {
                TinyIoCContainer.Current
                    .Register(handlerType.GetInterface(baseHandlerTypeName), handlerType)
                    .AsMultiInstance();
            }
        }

        private static void ScanAndRegisterItems()
        {
            var types = FindTypesOf(typeof(Item), typeof(Item).Assembly);

            TinyIoCContainer.Current.RegisterMultiple(typeof(Item), types);
        }

        private static void RegisterItemTemplates()
        {
            TinyIoCContainer.Current.Register<IEnumerable<Template>>((c, p) =>
            {
                var items = c.ResolveAll<Item>();

                var templates = new List<Template>();

                foreach (var item in items)
                {
                    var itemType = item.GetType();
                    var nameAtt = itemType.GetCustomAttribute<DisplayNameAttribute>();
                    var descAtt = itemType.GetCustomAttribute<DescriptionAttribute>();

                    var displayName = nameAtt != null ? nameAtt.DisplayName : itemType.Name;
                    var desc = descAtt?.Description;

                    var template = new Template(item, displayName, desc);

                    templates.Add(template);
                }

                return templates;
            });
        }

        private static IEnumerable<Type> FindGenericTypesOf(Type baseType, Assembly assembly)
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

        private static IEnumerable<Type> FindTypesOf(Type baseType, Assembly assembly)
        {
            return from type in assembly.GetTypes()
                   where type.BaseType == typeof(Item)
                   select type;
        }
    }
}
