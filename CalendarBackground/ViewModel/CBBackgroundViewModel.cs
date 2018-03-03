using BingHelper;
using CalendarBackground.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json;

namespace CalendarBackground.ViewModel
{
    [Serializable]
    public class CBBackgroundViewModel : INotifyPropertyChanged
    {
        private BackgroundType _currentType;
        public BackgroundType CurrentType
        {
            get => _currentType;
            set
            {
                _currentType = value;
                NotifyPropertyChanged("CurrentType");
            }
        }

        private Stretch _currentStretchType;
        public Stretch CurrentStretchType
        {
            get => _currentStretchType;
            set
            {
                _currentStretchType = value;
                NotifyPropertyChanged("CurrentStretchType");
            }
        }

        public ObservableCollection<ICBBackground> Items { get; set; } = new ObservableCollection<ICBBackground>();

        //public static async Task<CBBackgroundViewModel> GetInstance()
        //{
        //    var vm = new CBBackgroundViewModel();
        //    await vm.InitItemsAsync();
        //    vm.CurrentStretchType = Stretch.Fill;
        //    return vm;
        //}

        public static void SaveInstance(CBBackgroundViewModel vm)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.SAVE_FILE);

            var bf = new BinaryFormatter();
            try
            {
                using (var fs = new FileStream(Common.SAVE_FILE, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    bf.Serialize(fs, vm);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static CBBackgroundViewModel LoadInstance()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Common.SAVE_FILE);

            if (File.Exists(path))
            {
                var bf = new BinaryFormatter();
                try
                {
                    using (var fs = new FileStream(Common.SAVE_FILE, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        fs.Position = 0;
                        var des = bf.Deserialize(fs);
                        return (CBBackgroundViewModel) des;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (e is SerializationException)
                    {
                        File.Delete(path);
                    }
                    throw;
                }
            }
            return new CBBackgroundViewModel();
        }

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

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
