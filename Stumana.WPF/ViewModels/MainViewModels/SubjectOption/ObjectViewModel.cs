using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stumana.WPF.ViewModels.MainViewModels.SubjectOption
{
    public class ObjectViewModel : BaseViewModel
    {
        public ICommand editSubjectCommand { get; }
        public ICommand deleteSubjectCommand { get; }
        public ICommand addSubjectCommand { get; }
        public ICommand searchSubjectCommand { get; }
        public ICommand filterSubjectCommand { get; }

        public ObjectViewModel() { }
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
