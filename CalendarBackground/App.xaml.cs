using CalendarBackground.ViewModel;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CalendarBackground
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly CBBackgroundViewModel BackgroundViewModel = CBBackgroundViewModel.LoadInstance();
        public static readonly CBSettingMenuViewModel SettingMenuViewModel = new CBSettingMenuViewModel();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Custom UI Load
            // add custom accent and theme resource dictionaries to the ThemeManager
            // you should replace MahAppsMetroThemesSample with your application name
            // and correct place where your custom accent lives
            ThemeManager.AddAccent("CustomAccent", new Uri("pack://application:,,,/CustomTheme/CustomWindowTheme.xaml"));

            // get the current app style (theme and accent) from the application
            Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(Application.Current);

            // now change app style to the custom accent and current theme
            ThemeManager.ChangeAppStyle(Application.Current,
                                        ThemeManager.GetAccent("CustomAccent"),
                                        theme.Item1);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            CBBackgroundViewModel.SaveInstance(BackgroundViewModel);
        }
    }
}
