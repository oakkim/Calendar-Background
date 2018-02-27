using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BingHelper;
using CalendarBackground.Model;
using Microsoft.Win32;

namespace CalendarBackground.View.Pages.Setting
{
    /// <summary>
    /// Interaction logic for SettingBackgroundPage.xaml
    /// </summary>
    public partial class SettingBackgroundPage : Page
    {
        public SettingBackgroundPage()
        {
            InitializeComponent();
            this.DataContext = App.BackgroundViewModel;
        }

        private void SetPreviewImageSize()
        {
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.PrimaryScreen;
            var ratio = screen.WorkingArea.Width / 300;
            var height = screen.WorkingArea.Height / ratio;
            rectPreview.Height = height;
        }

        private void btnPictureSelect_Click(object sender, RoutedEventArgs e)
        {
            var filePath = OpenDialog()[0];
            App.BackgroundViewModel.AddBackground(new CBPictureBackground(filePath));
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
                imgPreview.ImageSource = new BitmapImage(new Uri(bingUnit.Path));
            }
        }

        private async void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            var cbxItem = sender as ComboBoxItem;
            if (cbxItem != null)
            {
                var grid = cbxItem.DataContext as Grid;
                if (grid != null && grid.DataContext != null)
                {
                    BackgroundType type = (BackgroundType)grid.DataContext;
                    if (App.BackgroundViewModel.CurrentType != type)
                    {
                        await App.BackgroundViewModel.ChangeBackgroundByDefaultValueAsync(type);
                    }
                    Debug.WriteLine(type);
                }
            }
        }

        private void SettingBackgroundPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetPreviewImageSize();
        }
    }
}
