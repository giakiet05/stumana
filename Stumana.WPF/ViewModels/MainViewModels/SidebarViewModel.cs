using Stumana.WPF.Stores;
using Stumana.WPF.ViewModels.MainViewModels.AccountOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.WPF.ViewModels.MainViewModels
{
    public class SidebarViewModel : BaseViewModel
    {
        public BaseViewModel CurrentSidebarViewModel => NavigationStore.Instance.CurrentLayoutModel;

        public SidebarViewModel()
        {
            NavigationStore.Instance.CurrentLayoutModelChanged += OnCurrentLayoutModelChanged;
            NavigationStore.Instance.CurrentLayoutModel = new AccountOptionViewModel();
        }
        private void OnCurrentLayoutModelChanged()
        {
            OnPropertyChanged(nameof(CurrentSidebarViewModel));
        }
    }
}
