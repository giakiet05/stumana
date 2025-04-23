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
      
        public BaseViewModel CurrentLayoutVM => NavigationStore.Instance.CurrentLayoutModel;



        public SidebarViewModel()
        {
            NavigationStore.Instance.CurrentLayoutModelChanged += OnCurrentLayoutModelChanged;
            NavigationStore.Instance.CurrentLayoutModel = new ClassListViewModel();
            
        }
        public void OnCurrentLayoutModelChanged()
        {
            OnPropertyChanged(nameof(CurrentLayoutVM));
        }
    }
}
