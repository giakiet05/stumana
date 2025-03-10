
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stumana.WPF.Stores;
using Stumana.WPF.ViewModels;

namespace Stumana.WPF.Commands
{
    public class NavigateLayoutCommand: BaseCommand
     
    {
        private readonly Func<BaseViewModel> _createViewModel;
        public NavigateLayoutCommand(Func<BaseViewModel> createViewModel)
        {
            _createViewModel = createViewModel;
        }
        public override void Execute(object parameter)
        {
            NavigationStore.Instance.CurrentLayoutModel = _createViewModel();
        }
    }
    
    
}
