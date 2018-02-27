using System;
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
        public SettingWindow()
        {
            InitializeComponent();
            DataContext = App.SettingMenuViewModel;
        }
    }
}
