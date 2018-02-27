using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalendarBackground.Annotations;
using CalendarBackground.Model;

namespace CalendarBackground.ViewModel
{
    public class CBSettingMenuViewModel
    {
        public ObservableCollection<CBSettingMenu> Items { get; } = new ObservableCollection<CBSettingMenu>();

        public CBSettingMenu SelectedMenu { get; set; }

        public CBSettingMenuViewModel()
        {
            InitializeItems();
        }

        private void InitializeItems()
        {
            Items.Add(new CBSettingMenu(){Idx = 0, Title = "Background"});
            Items.Add(new CBSettingMenu(){Idx = 1, Title = "Calendar"});
        }
    }
}
