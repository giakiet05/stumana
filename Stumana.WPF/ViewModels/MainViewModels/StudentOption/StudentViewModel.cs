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
        public ICommand editCommand { get; }
        public ICommand deleteCommand { get; }
        public ICommand addCommand { get; }
        public ICommand searchCommand { get; }
        public ICommand filterCommand { get; }

        public StudentViewModel() { }
        public string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }


    }
}
