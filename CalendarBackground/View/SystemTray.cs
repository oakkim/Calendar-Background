using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CalendarBackground
{
    public partial class MainWindow : Window
    {
        public static System.Windows.Forms.NotifyIcon notify;
        public static System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu();


        public void SetSystemTray()
        {
            notify = new System.Windows.Forms.NotifyIcon();

            SetContextMenu();

            notify.Icon = CalendarBackground.Properties.Resources.icon;
            notify.Text = "Calendar Background";
            notify.ContextMenu = menu;
            notify.Visible = true;
        }

        public void HideSystemTray()
        {
            notify.Visible = false;
            notify.Icon = null;
        }

        private void SetContextMenu()
        {
            System.Windows.Forms.MenuItem SettingItem = new System.Windows.Forms.MenuItem("Setting");
            System.Windows.Forms.MenuItem ExitItem = new System.Windows.Forms.MenuItem("Exit");

            SettingItem.Click += SettingItem_Click;
            ExitItem.Click += ExitItem_Click;

            menu.MenuItems.Add(SettingItem);
            menu.MenuItems.Add(ExitItem);
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
            App.Current.Shutdown();
        }
    }
}
