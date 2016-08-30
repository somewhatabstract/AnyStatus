using System;
using System.ComponentModel;
using System.Reflection;

namespace AnyStatus
{
    public static class Extensions
    {
        public static bool IsBrowsable(this Type type)
        {
            var att = type.GetCustomAttribute<BrowsableAttribute>();

            return att == null || att.Browsable;
        }
    }
}
