using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stumana.WPF.Commands;
using Stumana.WPF.ViewModels.PopupModels;

namespace Stumana.WPF.ViewModels.MainViewModels.SettleOption
{
    public class SemesterViewModel : BaseViewModel
    {
        public ICommand AddSemesterCommand { get; set; }

        public SemesterViewModel()
        {
            AddSemesterCommand = new NavigateModalCommand(() => new AddSemesterViewModel());
        }
    }
}
