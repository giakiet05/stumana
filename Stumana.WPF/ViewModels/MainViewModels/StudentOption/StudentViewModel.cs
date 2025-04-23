using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stumana.WPF.ViewModels.MainViewModels.StudentOption
{
    public class StudentViewModel : BaseViewModel
    {
        public ICommand addCommand { get; }
        public ICommand editCommand { get; }
        public ICommand deleteCommand { get; }


    }
}
