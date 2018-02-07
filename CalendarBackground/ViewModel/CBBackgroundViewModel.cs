using BingHelper;
using CalendarBackground.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CalendarBackground.ViewModel
{
    public class CBBackgroundViewModel : INotifyPropertyChanged
    {
        private BackgroundType _currentType;
        public BackgroundType CurrentType
        {
            get
            {
                return _currentType;
            }
            set
            {
                _currentType = value;
                NotifyPropertyChanged("CurrentType");
            }
        }

        private Stretch _currentStretchType;
        public Stretch CurrentStretchType
        {
            get
            {
                return _currentStretchType;
            }
            set
            {
                _currentStretchType = value;
                NotifyPropertyChanged("CurrentStretchType");
            }
        }

        public ObservableCollection<ICBBackground> Items { get; set; } = new ObservableCollection<ICBBackground>();

        //async public static Task<CBBackgroundViewModel> GetInstance()
        //{
        //    var vm = new CBBackgroundViewModel();
        //    await vm.InitItemsAsync();
        //    vm.CurrentStretchType = Stretch.Fill;
        //    return vm;
        //}

        public CBBackgroundViewModel()
        {
            InitItemsAsync();
            CurrentStretchType = Stretch.Fill;
        }

        private async Task InitItemsAsync()
        {
            await ChangeBackgroundByDefaultValueAsync(BackgroundType.BING_IMAGE);
        }

        public async Task ChangeBackgroundByDefaultValueAsync(BackgroundType type)
        {
            var list = new List<ICBBackground>();

            switch (type)
            {
                case BackgroundType.BING_IMAGE:
                    list.Add(await GetBingImageAsync());
                    break;
                case BackgroundType.PICTURE:
                    list.Add(new CBPictureBackground(Common.DEFAULT_PHOTO));
                    break;
                case BackgroundType.SOLID_COLOR:
                    list.Add(new CBSolidColorBackground("#555555"));
                    break;
                case BackgroundType.SLIDE_SHOW:
                    list.Add(new CBPictureBackground(Common.DEFAULT_PHOTO));
                    break;
                case BackgroundType.YOUTUBE:
                    list.Add(new CBYoutubeBackground(""));
                    break;
            }
            ChangeBackground(type, list);
        }

        public void ChangeBackground(BackgroundType type, IEnumerable<ICBBackground> backgrounds)
        {
            Items.Clear();
            CurrentType = type;
            foreach(var item in backgrounds)
            {
                AddBackground(item);
            }
        }

        public void AddBackground(ICBBackground background)
        {
            if(CurrentType != BackgroundType.SLIDE_SHOW)
            {
                Items.Clear();
            }
            if(ValidateType(background))
            {
                Items.Add(background);
            }
        }

        private bool ValidateType(ICBBackground background)
        {
            var isValid = background != null
                          && (background.BackgroundType == CurrentType
                          || (background.BackgroundType == BackgroundType.SLIDE_SHOW && CurrentType == BackgroundType.PICTURE)
                          || (background.BackgroundType == BackgroundType.PICTURE && CurrentType == BackgroundType.SLIDE_SHOW));
            return isValid;
        }

        public async Task<CBBingImageBackground> GetBingImageAsync()
        {
            BingImage bingImage = new BingImage();
            var bingUnit = await bingImage.GetTodayBingUnit(BingImageResolution.LARGE);
            if (bingUnit != null)
            {
                var bg = new CBBingImageBackground();
                bg.Background = bingUnit.Path;
                bg.Copyright = bingUnit.Copyright;
                return bg;
            }
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
