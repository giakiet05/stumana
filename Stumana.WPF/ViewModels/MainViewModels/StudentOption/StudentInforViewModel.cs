using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Stumana.WPF.ViewModels.MainViewModels.StudentOption
{
    public class StudentInfoViewModel : BaseViewModel
    {
        public Dictionary<string, SchoolYear> SchoolYearDic { get; set; } = new();
        private ObservableCollection<string> _schoolyearFilter = new();

        public ObservableCollection<string> SchoolYearFilter
        {
            get => _schoolyearFilter;
            set
            {
                _schoolyearFilter = value;
                OnPropertyChanged();
            }
        }

        private string? _selectedSchoolYear;

        public string? SelectedSchoolYear
        {
            get => _selectedSchoolYear;
            set
            {
                if (_selectedSchoolYear != value)
                {
                    _selectedSchoolYear = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<int> _semesterFilter = new();

        public ObservableCollection<int> SemesterFilter
        {
            get => _semesterFilter;
            set
            {
                _semesterFilter = value;
                OnPropertyChanged();
            }
        }

        private int? _selectedSemester;

        public int? SelectedSemester
        {
            get => _selectedSemester;
            set
            {
                _selectedSemester = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseModalCommand { get; set; }

        public StudentInfoViewModel(Student? student)
        {
            CloseModalCommand = new RelayCommand(ModalNavigationStore.Instance.Close);

            LoadSchoolYearFilter();
            LoadSemesterFilter();
        }

        private async void LoadSchoolYearFilter()
        {
            SchoolYearFilter.Clear();
        }

        private async void LoadSemesterFilter()
        {
            if (SelectedSchoolYear == null) 
                return;

            SemesterFilter.Clear();
        }
    }
}
