namespace BuildExplorer.Core.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class TypeDiscovery
    {
        //static readonly Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();

        //public static IEnumerable<Type> Discover<T>()
        //{
        //    return Discover(typeof(T));
        //}

        //private static IEnumerable<Type> Discover(Type type)
        //{
        //    return Assemblies.SelectMany(TryGetTypes).Where(x => NewMethod(type, x));
        //}

        //private static bool NewMethod(Type type, Type x)
        //{
        //    return type.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract;
        //}

        //private static Type[] TryGetTypes(Assembly assembly)
        //{
        //    try
        //    {
        //        return assembly.GetTypes();
        //    }
        //    catch
        //    {
        //        return new Type[0];
        //    }
        //}

        //public static IEnumerable<Type> DiscoverCloseTypesOf(Type type)
        //{
        //    return Assemblies.SelectMany(TryGetTypes).Where(k => k.IsClosedTypeOf(type));
        //}
    }
}