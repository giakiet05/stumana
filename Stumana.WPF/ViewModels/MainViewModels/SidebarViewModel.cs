using Stumana.WPF.Stores;
using Stumana.WPF.ViewModels.MainViewModels.AccountOption;
using Stumana.WPF.ViewModels.MainViewModels.ClassOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.WPF.ViewModels.MainViewModels
{
    public class SidebarViewModel : BaseViewModel
    {
        private string text;


        public string Text
        {
            get { return text; }
            set {
                text = value; 
                OnPropertyChanged(nameof(Text));
            }
        }

        public BaseViewModel CurrentSidebarViewModel => NavigationStore.Instance.CurrentLayoutModel;

        public SidebarViewModel()
        {
            NavigationStore.Instance.CurrentLayoutModelChanged += OnCurrentLayoutModelChanged;
            NavigationStore.Instance.CurrentLayoutModel = new ClassListViewModel();
            Text = "Testing";
        }
        private void OnCurrentLayoutModelChanged()
        {
            OnPropertyChanged(nameof(CurrentSidebarViewModel));
        }
    }
}
