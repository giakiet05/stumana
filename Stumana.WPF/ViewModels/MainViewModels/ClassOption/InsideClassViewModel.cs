using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stumana.WPF.ViewModels.MainViewModels.ClassOption
{
    public class InsideClassViewModel : BaseViewModel
    {
        public ICommand addCommand { get; }
        public ICommand removeCommand { get; }
        public ICommand exportCommand { get; }
        public InsideClassViewModel() { }
    }
}
