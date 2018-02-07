using CalendarBackground.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CalendarBackground.Converter
{
    public class BackgroundTypeToIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is BackgroundType))
            {
                return -1;
            }
            var type = (BackgroundType)value;
            return (int)type;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
            {
                return -1;
            }
            var type = (BackgroundType)value;
            return type;
        }
    }
}
