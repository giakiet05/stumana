using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace Stumana.WPF.ViewModels.MainViewModels.StudentOption
{
    public class StudentInfoViewModel : BaseViewModel
    {
        private DataTable _scoreDataTable = new DataTable();

        public DataTable ScoreDataTable
        {
            get => _scoreDataTable;
            set
            {
                _scoreDataTable = value;
                OnPropertyChanged();
            }
        }

        private DataView _tableView;

        public DataView TableView
        {
            get => _tableView;
            set
            {
                _tableView = value;
                OnPropertyChanged(nameof(TableView));
            }
        }

        private string _studentId;

        public string StudentId
        {
            get => _studentId;
            set
            {
                _studentId = value;
                OnPropertyChanged();
            }
        }

        private string _studentName;

        public string StudentName
        {
            get => _studentName;
            set
            {
                _studentName = value;
                OnPropertyChanged();
            }
        }


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

        private string _classroom;

        public string Classroom
        {
            get => _classroom;
            set
            {
                _classroom = value;
                OnPropertyChanged();
            }
        }

        private string _absence;

        public string Absence
        {
            get => _absence;
            set
            {
                _absence = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<string> GradingCollection { get; set; }
        private string _selectedGrading;

        public string SelectedGrading
        {
            get => _selectedGrading;
            set
            {
                _selectedGrading = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ConductCollection { get; set; }
        private string _selectedConduct;

        public string SelectedConduct
        {
            get => _selectedConduct;
            set
            {
                _selectedConduct = value;
                OnPropertyChanged();
            }
        }

        private string _evaluation;

        public string Evaluation
        {
            get => _evaluation;
            set
            {
                _evaluation = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseModalCommand { get; set; }

        public StudentInfoViewModel(Student? student)
        {
            CloseModalCommand = new RelayCommand(SaveInfo);

            LoadSchoolYearFilter();
            LoadSemesterFilter();

            Classroom = "10A3";
        }

        private async void LoadSchoolYearFilter()
        {
            SchoolYearFilter.Clear();
        }

        private async void LoadSemesterFilter()
        {
            SemesterFilter.Clear();
            if (SelectedSchoolYear == null)
                return;
        }

        private async void LoadTableColumn()
        {
            ScoreDataTable = new DataTable();
            ScoreDataTable.Columns.Add("Môn học", typeof(string));


            ScoreDataTable.Columns.Add("Điểm TB", typeof(double));
            TableView = ScoreDataTable.DefaultView;
        }

        private void SaveInfo()
        {
            ModalNavigationStore.Instance.Close();
        }
    }
}