using CefSharp;
using CefSharp.Wpf;
using GlobalLowLevelHooks;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace CalendarBackground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IntPtr workerw = IntPtr.Zero;
        IntPtr desktopWorkerw = IntPtr.Zero;
#if false
#region GlobalMouseHook
        MouseHook mouseHook = new MouseHook();
        #endregion
#endif
        private IntPtr currentHwnd = IntPtr.Zero;

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

            grMain.DataContext = App.backgroundViewModel;
        }

        private void InitOnDisplaySettingChanged()
        {
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                FillDisplay();
            }));
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
            Point pt = new Point(mouseStruct.pt.x, mouseStruct.pt.y);
            //Debug.WriteLine(pt);  

            IntPtr win = W32.GetForegroundWindow();
            Debug.WriteLine("point: " + pt + ", hWnd: " + win.ToInt32());
            if(win == desktopWorkerw)
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

        public void InitWorkerW()
        {
            // Fetch the Progman window
            IntPtr progman = W32.FindWindow("Progman", null);

            IntPtr result = IntPtr.Zero;

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
                IntPtr p = W32.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    // Gets the SHELLDLL_DefView's WorkerW;
                    desktopWorkerw = W32.GetParent(p);

                    // Gets the WorkerW Window after the current one.
                    workerw = W32.FindWindowEx(IntPtr.Zero,
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
            //FillYoutube("a");
        }

        private IntPtr GetWindowHandle()
        {
            if(currentHwnd == IntPtr.Zero)
            {
                Window window = Window.GetWindow(this);
                currentHwnd = new WindowInteropHelper(window).EnsureHandle();
            }

            return currentHwnd;
        }

        //Temporary
        private void FillDisplay()
        {
            var point = System.Windows.Forms.Cursor.Position;
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint(point);
            var area = screen.WorkingArea;

            W32.SetWindowPos(workerw, W32.HWND.Bottom, area.X, area.Y, area.Width, area.Height, 0);
            this.Top = area.Top;
            this.Left = area.Left;
            this.Width = area.Width;
            this.Height = area.Height;
        }

        private void FillYoutube(string url)
        {
            CefSettings s = new CefSettings();
            s.SetOffScreenRenderingBestPerformanceArgs();
            Cef.Initialize(s);
            ChromiumWebBrowser wbMain = new ChromiumWebBrowser(); 
            //wbMain.Address = "https://www.youtube.com/embed/videoseries?list=PLjkUDT6hWqyrotJAOd5JAdF_63LFku36i;autoplay=1";
            wbMain.Address = "https://www.youtube.com/embed/DnDFThL1qlI?autoplay=1;loop=1";
            grMain.Children.Add(wbMain);
        }

        private void ShowOnWorkerW()
        {
            var hwnd = GetWindowHandle();
            Debug.WriteLine("curhwnd: " + hwnd.ToInt32());

            W32.SetParent(hwnd, workerw);
        }

        private async void Window_StateChanged_1(object sender, EventArgs e)
        {
            await MaximizeWindow(this);
        }

        public Task MaximizeWindow(Window window)
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
