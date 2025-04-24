
using System.Collections.ObjectModel;
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;

namespace Stumana.WPF.ViewModels.MainViewModels.ClassOption
{
    public class ClassListViewModel: BaseViewModel
    {
        #region Commands
        public ICommand exportCommand { get; }

        #endregion Commands

        #region Properties

        private Dictionary<string, SchoolYear> SchoolYearsDic = new Dictionary<string, SchoolYear>();

        private ObservableCollection<string> _schoolyearCollection;
        public ObservableCollection<string> SchoolYearCollection
        {
            get => _schoolyearCollection;
            set
            {
                _schoolyearCollection = value;
                OnPropertyChanged();
            }
        }

        private string _selectedschoolyear;
        public string SelectedSchoolYear
        {
            get => _selectedschoolyear;
            set
            {
                _selectedschoolyear = value;
                OnPropertyChanged();
                SelectionChange();
            }
        }

        private Dictionary<string, Grade> GradeDic = new Dictionary<string, Grade>();

        private ObservableCollection<string> _gradeCollection;
        public ObservableCollection<string> GradeCollection
        {
            get => _gradeCollection;
            set
            {
                _gradeCollection = value;
                OnPropertyChanged();
            }
        }

        private string _selectedgrade;
        public string SelectedGrade
        {
            get => _selectedgrade;
            set
            {
                _selectedgrade = value;
                OnPropertyChanged();
                SelectionChange();
            }
        }

        private ObservableCollection<Classroom> _classroomCollection;
        public ObservableCollection<Classroom> ClassroomCollection
        {
            get => _classroomCollection;
            set
            {
                _classroomCollection = value;
                OnPropertyChanged();
            }
        }


        private string _searchString;
        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                OnPropertyChanged();
            }
        }
        #endregion Properties
       

        public ClassListViewModel()
        {
            LoadSchoolYear();
            LoadGrade();
        }


        private async void SelectionChange()
        {
            if (string.IsNullOrEmpty(SelectedGrade) || string.IsNullOrEmpty(SelectedSchoolYear))
            {
                ClassroomCollection = null;
                return;
            }

            IEnumerable<Classroom> classrooms;

            if (SelectedSchoolYear != "Tất cả các năm")
            {
                SchoolYear curSchoolYear = SchoolYearsDic[SelectedSchoolYear];
                classrooms = await GenericDataService<Classroom>.Instance.GetManyAsync(cl => cl.YearId == curSchoolYear.Id);
            }
            else
                classrooms = await GenericDataService<Classroom>.Instance.GetAllAsync();

            if (SelectedGrade != "Tất cả các khối")
            {
                Grade curGrade = GradeDic[SelectedGrade];
                classrooms = classrooms.Where(cl => cl.GradeId == curGrade.Id);
            }

            ClassroomCollection = new ObservableCollection<Classroom>(classrooms);
        }

        private async void LoadSchoolYear()
        {
            SchoolYearCollection = new ObservableCollection<string>();
            var sy = await GenericDataService<SchoolYear>.Instance.GetAllAsync();
            foreach (SchoolYear schoolYear in sy)
            {
                string schoolyearstr = schoolYear.StartYear + "-" + schoolYear.EndYear;
                SchoolYearCollection.Add(schoolyearstr);
                SchoolYearsDic.Add(schoolyearstr, schoolYear);
            }
            SchoolYearCollection.Add("Tất cả các năm");
        }

        private async void LoadGrade()
        {
            GradeCollection = new ObservableCollection<string>();
            var grades = await GenericDataService<Grade>.Instance.GetAllAsync();
            foreach (Grade grade in grades)
            {
                GradeCollection.Add("Khối" + grade.Level);
                GradeDic.Add("Khối" + grade.Level, grade);
            }
            GradeCollection.Add("Tất cả các khối");
        }
    }
}
