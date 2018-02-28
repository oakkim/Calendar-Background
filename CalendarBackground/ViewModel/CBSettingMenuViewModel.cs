using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CalendarBackground.Annotations;
using CalendarBackground.Helper;
using CalendarBackground.Model;

namespace CalendarBackground.ViewModel
{
    public class CBSettingMenuViewModel : INotifyPropertyChanged
    {
        private CBSettingMenu _selectedMenu;
        public ObservableCollection<CBSettingMenu> Items { get; } = new ObservableCollection<CBSettingMenu>();

        public CBSettingMenu SelectedMenu
        {
            get => _selectedMenu;
            set
            {
                _selectedMenu = value;
                OnPropertyChanged(nameof(SelectedMenu));
            }
        }

        public CBSettingMenuViewModel()
        {
            InitializeItems();
        }

        private void InitializeItems()
        {
            Items.Add(new CBSettingMenu(){Idx = 0, Title = "Background", Icon = IconHelper.FindIconCanvasByName("icon_picture") });
            Items.Add(new CBSettingMenu(){Idx = 1, Title = "Calendar", Icon = IconHelper.FindIconCanvasByName("icon_calendar") });
            SelectedMenu = Items[0];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
