using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarBackground.Model
{
    public enum BackgroundType
    {
        BING_IMAGE,
        PICTURE,
        SOLID_COLOR,
        SLIDE_SHOW,
        YOUTUBE
    }

    public interface ICBBackground : INotifyPropertyChanged
    {
        BackgroundType BackgroundType { get; }
        string Background { get; set; }
    }

    public class CBBingImageBackground : ICBBackground
    {
        public BackgroundType BackgroundType
        {
            get
            {
                return BackgroundType.BING_IMAGE;
            }
        }

        private string _background;
        public string Background
        {
            get { return _background; }
            set
            {
                _background = value;
                NotifyPropertyChanged("Background");
            }
        }

        private string _copyright;
        public string Copyright
        {
            get { return _copyright; }
            set
            {
                _copyright = value;
                NotifyPropertyChanged("Copyright");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CBSolidColorBackground : ICBBackground
    {
        public BackgroundType BackgroundType
        {
            get
            {
                return BackgroundType.SOLID_COLOR;
            }
        }

        private string _background;
        public string Background
        {
            get { return _background; }
            set
            {
                _background = value;
                NotifyPropertyChanged("Background");
            }
        }

        public CBSolidColorBackground(string background)
        {
            Background = background;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CBPictureBackground : ICBBackground
    {
        public BackgroundType BackgroundType
        {
            get
            {
                return BackgroundType.PICTURE;
            }
        }

        private string _background;
        public string Background
        {
            get { return _background; }
            set
            {
                _background = value;
                NotifyPropertyChanged("Background");
            }
        }

        public CBPictureBackground(string background)
        {
            Background = background;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CBYoutubeBackground : ICBBackground
    {
        public BackgroundType BackgroundType
        {
            get
            {
                return BackgroundType.PICTURE;
            }
        }

        private string _background;
        public string Background
        {
            get { return _background; }
            set
            {
                _background = value;
                NotifyPropertyChanged("Background");
            }
        }

        public CBYoutubeBackground(string background)
        {
            Background = background;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
