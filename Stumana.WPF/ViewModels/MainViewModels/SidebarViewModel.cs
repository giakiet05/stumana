using Stumana.WPF.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.WPF.ViewModels.MainViewModels
{
    public class SidebarViewModel: BaseViewModel
    {
        public BaseViewModel CurrentSidebarViewModel => NavigationStore.Instance.CurrentLayoutModel;
    }
}
