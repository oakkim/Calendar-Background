using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CalendarBackground.Converter
{
    public class StretchEnumToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Stretch))
            {
                return -1;
            }
            var type = (Stretch)value;
            return (int)type - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
            {
                return -1;
            }
            int i = (int)value + 1;
            var type = (Stretch)i;
            return type;
        }
    }
}
