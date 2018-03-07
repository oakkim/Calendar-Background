using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BingHelper;
using CalendarBackground.Model;
using CalendarBackground.ViewModel;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace CalendarBackground.View
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : MetroWindow
    {
        private static SettingWindow _instance;

        private SettingWindow()
        {
            InitializeComponent();
            DataContext = App.SettingMenuViewModel;
        }

        public static SettingWindow GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SettingWindow();
            }
            return _instance;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _instance = null;
        }
    }
}
