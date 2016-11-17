using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace AnyStatus
{
    public class TooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var item = (Item)value;

            return $"Status:  {item.State.Metadata.DisplayName}\r\nType: {GetDisplayName(item)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string GetDisplayName(Item item)
        {
            var type = item.GetType();

            var name = type.GetCustomAttribute<DisplayNameAttribute>();

            return name != null ? name.DisplayName : CamelCaseToWords(type.Name);
        }

        private static string CamelCaseToWords(string input)
        {
            return Regex.Replace(input, "(\\B[A-Z])", " $1");
        }
    }
}
