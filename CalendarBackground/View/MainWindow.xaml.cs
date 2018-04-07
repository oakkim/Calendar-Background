using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using CefSharp;
using CefSharp.Wpf;
using GlobalLowLevelHooks;
using Microsoft.Win32;

namespace CalendarBackground.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IntPtr _workerw = IntPtr.Zero;
        IntPtr _desktopWorkerw = IntPtr.Zero;
#if false
#region GlobalMouseHook
        MouseHook mouseHook = new MouseHook();
        #endregion
#endif
        private IntPtr _currentHwnd = IntPtr.Zero;

        public MainWindow()
        {
            InitializeComponent();
            InitWorkerW();
            InitTimer();
#if false
#region GlobalMouseHook
            InitMouseHook();
#endregion
#endif
            InitOnDisplaySettingChanged();

            grMain.DataContext = App.BackgroundViewModel;
        }

        private void InitOnDisplaySettingChanged()
        {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(FillDisplay));
        }
#if false
#region GlobalMouseHook
        private void InitMouseHook()
        {
            // Capture the events
            mouseHook.LeftButtonUp += MouseHook_LeftButtonUp;

            //Installing the Mouse Hooks
            mouseHook.Install();
        }
#endregion
#endif
        private void MouseHook_LeftButtonUp(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            var pt = new Point(mouseStruct.pt.x, mouseStruct.pt.y);
            //Debug.WriteLine(pt);  

            var win = W32.GetForegroundWindow();
            Debug.WriteLine("point: " + pt + ", hWnd: " + win.ToInt32());
            if(win == _desktopWorkerw)
            {
                Debug.WriteLine("바탕화면클릭함. 이걸로 클릭하겟음 - " + GetWindowHandle().ToInt32());

                //var lparam = (IntPtr)((500 << 16) | 500);

                var lparam = (IntPtr)((mouseStruct.pt.x << 16) | mouseStruct.pt.y);
                W32.PostMessage(GetWindowHandle(), (uint)W32.WMessages.WM_LBUTTONDOWN, (IntPtr)0x0, lparam);
                W32.PostMessage(GetWindowHandle(), (uint)W32.WMessages.WM_LBUTTONUP, (IntPtr)0x1, lparam);

                //W32.SendMessage(GetWindowHandle(), (uint)W32.WMessages.WM_LBUTTONDOWN, 0, lparam);
            }
            //ClickOnPoint(GetWindowHandle());
        }

        private void InitWorkerW()
        {
            // Fetch the Progman window
            var progman = W32.FindWindow("Progman", null);

            var result = IntPtr.Zero;

            // Send 0x052C to Progman. This message directs Progman to spawn a 
            // WorkerW behind the desktop icons. If it is already there, nothing 
            // happens.
            W32.SendMessageTimeout(progman,
                                   0x052C,
                                   new IntPtr(0),
                                   IntPtr.Zero,
                                   W32.SendMessageTimeoutFlags.SMTO_NORMAL,
                                   1000,
                                   out result);


            // We enumerate all Windows, until we find one, that has the SHELLDLL_DefView 
            // as a child. 
            // If we found that window, we take its next sibling and assign it to workerw.
            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                var p = W32.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    // Gets the SHELLDLL_DefView's WorkerW;
                    _desktopWorkerw = W32.GetParent(p);

                    // Gets the WorkerW Window after the current one.
                    _workerw = W32.FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               IntPtr.Zero);
                }

                return true;
            }), IntPtr.Zero);

            // We now have the handle of the WorkerW behind the desktop icons.
            // We can use it to create a directx device to render 3d output to it, 
            // we can use the System.Drawing classes to directly draw onto it, 
            // and of course we can set it as the parent of a windows form.
            //
            // There is only one restriction. The window behind the desktop icons d oes
            // NOT receive any user input. So if you want to capture mouse movement, 
            // it has to be done the LowLevel way (WH_MOUSE_LL, WH_KEYBOARD_LL).
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowOnWorkerW();
            FillDisplay();
            SetCalendar();
            //SetMealList();
            SetSystemTray();
            //FillYoutube("https://www.youtube.com/watch?v=Sk-U8ruIQyA");
        }

        private IntPtr GetWindowHandle()
        {
            if (_currentHwnd != IntPtr.Zero) return _currentHwnd;
            var window = Window.GetWindow(this);
            _currentHwnd = new WindowInteropHelper(window).EnsureHandle();

            return _currentHwnd;
        }

        //Temporary
        private void FillDisplay()
        {
            var point = System.Windows.Forms.Cursor.Position;
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint(point);
            var area = screen.WorkingArea;

            W32.SetWindowPos(_workerw, W32.HWND.Bottom, area.X, area.Y, area.Width, area.Height, 0);
            this.Top = area.Top;
            this.Left = area.Left;
            this.Width = area.Width;
            this.Height = area.Height;
        }

        private void FillYoutube(string url)
        {
            const string reg = @"https\:\/\/www\.youtube\.com\/watch\?v=([\w-]{11})";
            var result = Regex.Match(url, reg);
            var code = result.Groups[1].Value;
            cefBrowser.Address = $"https://www.youtube.com/embed/{code}?autoplay=1;loop=1";
        }

        private void ShowOnWorkerW()
        {
            var hwnd = GetWindowHandle();
            Debug.WriteLine("curhwnd: " + hwnd.ToInt32());

            W32.SetParent(hwnd, _workerw);
        }

        private async void Window_StateChanged_1(object sender, EventArgs e)
        {
            await MaximizeWindow(this);
        }

        private Task MaximizeWindow(Window window)
        {
            return Task.Factory.StartNew(() =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    window.WindowState = WindowState.Maximized;
                }));
            });
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
#if false
#region GlobalMouseHook
            mouseHook.LeftButtonUp -= MouseHook_LeftButtonUp;
            mouseHook.Uninstall();
#endregion
#endif
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("asdf");
            var lparam = new IntPtr((500 << 16) | 500);
            W32.PostMessage(GetWindowHandle(), (uint)W32.WMessages.WM_LBUTTONDOWN, (IntPtr)0x1, lparam);
            W32.PostMessage(GetWindowHandle(), (uint)W32.WMessages.WM_LBUTTONUP, (IntPtr)0x1, lparam);
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine("asdf");
            MessageBox.Show("asdf");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            HideSystemTray();
        }
    }
}
