using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CalendarBackground.Theme;

namespace CalendarBackground.Helper
{
    public static class IconHelper
    {
        private static Icon res = new Icon();

        static IconHelper()
        {
            res.InitializeComponent();
        }

        public static Canvas FindIconCanvasByName(string name)
        {
            var item = res[name];
            var canvas = item as Canvas;
            return canvas;
        }
    }
}
