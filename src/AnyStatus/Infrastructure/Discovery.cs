using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AnyStatus.Infrastructure
{
    public class Discovery
    {
        public static IEnumerable<Type> FindGenericTypesOf(Type baseType, Assembly assembly)
        {
            return from  type in assembly.GetTypes()
                   where type.IsAbstract == false &&
                         type.IsGenericTypeDefinition == false

                   let handlerInterfaces =
                       from iface in type.GetInterfaces()
                       where iface.IsGenericType
                       where iface.GetGenericTypeDefinition() == baseType
                       select iface

                   where handlerInterfaces.Any()

                   select type;
        }

        public static IEnumerable<Type> FindTypesOf(Type baseType, Assembly[] assemblies)
        {
            return from assembly in assemblies
                   from type in assembly.GetTypes()

                   where type.IsAbstract == false &&
                         baseType.IsAssignableFrom(type)

                   select type;
        }
    }
}
