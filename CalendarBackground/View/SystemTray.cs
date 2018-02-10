using System;
using System.Windows;

namespace CalendarBackground.View
{
    public partial class MainWindow : Window
    {
        private static System.Windows.Forms.NotifyIcon _notify;
        private static readonly System.Windows.Forms.ContextMenu Menu = new System.Windows.Forms.ContextMenu();


        private void SetSystemTray()
        {
            _notify = new System.Windows.Forms.NotifyIcon();

            SetContextMenu();

            _notify.Icon = CalendarBackground.Properties.Resources.icon;
            _notify.Text = "Calendar Background";
            _notify.ContextMenu = Menu;
            _notify.Visible = true;
        }

        private void HideSystemTray()
        {
            _notify.Visible = false;
            _notify.Icon = null;
        }

        private void SetContextMenu()
        {
            System.Windows.Forms.MenuItem settingItem = new System.Windows.Forms.MenuItem("Setting");
            System.Windows.Forms.MenuItem exitItem = new System.Windows.Forms.MenuItem("Exit");

            settingItem.Click += SettingItem_Click;
            exitItem.Click += ExitItem_Click;

            Menu.MenuItems.Add(settingItem);
            Menu.MenuItems.Add(exitItem);
        }

        private void SettingItem_Click(object sender, EventArgs e)
        {
            SettingWindow win = new SettingWindow();
            win.Show();
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            //App.Current.Shutdown();
            Close();
        }

        private void Notify_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
