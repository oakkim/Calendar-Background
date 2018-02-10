using System;
using System.Threading.Tasks;
using System.Windows;

namespace CalendarBackground.View
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
