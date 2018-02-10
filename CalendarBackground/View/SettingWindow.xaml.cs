using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BingHelper;
using CalendarBackground.Model;
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

            this.DataContext = App.backgroundViewModel;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetPreviewImageSize();
        }

        private void SetPreviewImageSize()
        {
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.PrimaryScreen;
            var ratio = screen.WorkingArea.Width / 300;
            var height = screen.WorkingArea.Height / ratio;
            imgPreview.Height = height;
        }

        private void btnPictureSelect_Click(object sender, RoutedEventArgs e)
        {
            var filePath = OpenDialog()[0];
            App.backgroundViewModel.AddBackground(new CBPictureBackground(filePath));
        }

        private string[] OpenDialog(bool multiSelect = false)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = multiSelect;
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";//이미지 필터링 설정
            var result = openFileDialog.ShowDialog();
            if (result ?? false)
            {
                return openFileDialog.FileNames;
            }
            return null;
        }

        public async Task SetBingImageAsync()
        {
            BingImage bingImage = new BingImage();
            var bingUnit = await bingImage.GetTodayBingUnit(BingImageResolution.LARGE);
            if (bingUnit != null)
            {
                imgPreview.Source = new BitmapImage(new Uri(bingUnit.Path));
            }
        }

        private async void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            var cbxItem = sender as ComboBoxItem;
            if(cbxItem != null)
            {
                var grid = cbxItem.DataContext as Grid;
                if (grid != null && grid.DataContext != null)
                {
                    BackgroundType type = (BackgroundType)grid.DataContext;
                    if(App.backgroundViewModel.CurrentType != type)
                    {
                        await App.backgroundViewModel.ChangeBackgroundByDefaultValueAsync(type);
                    }
                    Debug.WriteLine(type);
                }
            }
        }
    }
}
