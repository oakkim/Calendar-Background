using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CalendarBackground
{
    /// <summary>
    /// Interaction logic for CalendarBarWindow.xaml
    /// </summary>
    public partial class CalendarBarWindow : Window
    {
        public CalendarBarWindow()
        {
            InitializeComponent();
        }

        private async void Window_StateChanged_1(object sender, EventArgs e)
        {
            await MaximizeWindow(this);
        }

        public Task MaximizeWindow(Window window)
        {
            return Task.Factory.StartNew(() =>
            {
                this.Dispatcher.Invoke((() =>
                {
                    window.WindowState = WindowState.Maximized;
                }));
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillDisplay();
        }

        private void FillDisplay()
        {
            this.Width = SystemParameters.WorkArea.Width;
            this.Height = SystemParameters.WorkArea.Height;
            this.Top = 0;
            this.Left = 0;
        }
    }
}
