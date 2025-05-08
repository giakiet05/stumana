using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.ViewModels.MainViewModels.ScoreOption;
using Stumana.WPF.ViewModels.PopupModels;

namespace Stumana.WPF.ViewModels.MainViewModels.StudentOption
{
    public class StudentViewModel : BaseViewModel
    {
        private DataTable StudentDataTable { get; set; } = new DataTable();
        private DataView _tableView;

        public DataView TableView
        {
            get => _tableView;
            set
            {
                _tableView = value;
                OnPropertyChanged();
            }
        }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                OnSearchTextChange();
            }
        }

        public Dictionary<string, SchoolYear> SchoolYearDic { get; set; } = new();
        public Dictionary<string, Grade> GradeDic { get; set; } = new();
        public Dictionary<string, Classroom> ClassroomDic { get; set; }

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

        private string _selectedSchoolYear;

        public string SelectedSchoolYear
        {
            get => _selectedSchoolYear;
            set
            {
                if (_selectedSchoolYear != value)
                {
                    _selectedSchoolYear = value;
                    OnPropertyChanged();
                    OnFilterChange();
                }
            }
        }

        private ObservableCollection<FilterItem> _gradeFilter = new();

        public ObservableCollection<FilterItem> GradeFilter
        {
            get => _gradeFilter;
            set
            {
                _gradeFilter = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<FilterItem> _classFilter = new();

        public ObservableCollection<FilterItem> ClassFilter
        {
            get => _classFilter;
            set
            {
                _classFilter = value;
                OnPropertyChanged();
            }
        }

        public ICommand FilterGradeCommand { get; set; }
        public ICommand FilterClassroomCommand { get; set; }
        public ICommand AddStudentCommand { get; set; }

        public StudentViewModel()
        {
            FilterGradeCommand = new RelayCommand(FilterGrade);
            FilterClassroomCommand = new RelayCommand(FilterClassRoom);
            AddStudentCommand = new NavigateModalCommand(() => new AddStudentViewModel());

            LoadStudentTableColumn();
            LoadInitFilter();
        }

        private async void LoadInitFilter()
        {
            await LoadSchoolYearFilter();
            await LoadGradeFilter();
        }

        private void LoadStudentTableColumn()
        {
            StudentDataTable.Columns.Add("Khối", typeof(string));
            StudentDataTable.Columns.Add("Lớp", typeof(string));
            StudentDataTable.Columns.Add("Họ và tên", typeof(string));
            StudentDataTable.Columns.Add("Giới tính", typeof(string));
            StudentDataTable.Columns.Add("Ngày sinh", typeof(string));
            StudentDataTable.Columns.Add("Địa chỉ", typeof(string));
            StudentDataTable.Columns.Add("Số điện thoại", typeof(string));
            StudentDataTable.Columns.Add("Email", typeof(string));
            StudentDataTable.Columns.Add("TB học kỳ 1", typeof(double));
            StudentDataTable.Columns.Add("TB học kỳ 2", typeof(double));

            TableView = StudentDataTable.DefaultView;
        }

        private async Task LoadSchoolYearFilter()
        {
            SchoolYearFilter.Clear();

            List<SchoolYear> schoolYears = (await GenericDataService<SchoolYear>.Instance.GetAllAsync()).ToList();

            if (!schoolYears.Any())
                return;

            foreach (SchoolYear schoolYear in schoolYears)
            {
                string schoolyearName = schoolYear.StartYear + " - " + schoolYear.EndYear;
                SchoolYearFilter.Add(schoolyearName);
                SchoolYearDic.Add(schoolyearName, schoolYear);
            }
        }

        public async Task LoadGradeFilter()
        {
            GradeFilter.Clear();

            var grades = (await GenericDataService<Grade>.Instance.GetAllAsync()).ToList();
            if (!grades.Any())
                return;

            GradeFilter.Add(new FilterItem("All", false));
            foreach (Grade grade in grades)
            {
                string gradeName = $"Khối {grade.Level}";
                GradeFilter.Add(new FilterItem(gradeName, false));
                GradeDic.Add(gradeName, grade);
            }
        }

        public async Task LoadClassroomFilter(SchoolYear schoolYear, IEnumerable<Grade> grades)
        {
            ClassFilter.Clear();

            var gradeId = grades.Select(g => g.Id);
            List<Classroom> classrooms = (await GenericDataService<Classroom>.Instance.GetManyAsync(c => c.YearId == schoolYear.Id && gradeId.Contains(c.GradeId))).ToList();

            if (!classrooms.Any())
                return;

            ClassroomDic.Clear();
            ClassFilter.Add(new FilterItem("All", false));
            foreach (Classroom classroom in classrooms)
            {
                string className = $"Lớp {classroom.Name}";
                ClassFilter.Add(new FilterItem(className, false));
                ClassroomDic[className] = classroom;
            }
        }

        private void FilterGrade(object param)
        {
            FilterItem filterItem = (FilterItem)param;
            ProcessFilterItemSelection(filterItem, GradeFilter);
            OnFilterChange();
        }

        private void FilterClassRoom(object param)
        {
            FilterItem filterItem = (FilterItem)param;
            ProcessFilterItemSelection(filterItem, ClassFilter);
            OnFilterChange();
        }

        private void ProcessFilterItemSelection(FilterItem filterItem, ObservableCollection<FilterItem> filterItems)
        {
            if (filterItem.Name == "All")
            {
                if (filterItem.IsChecked)
                {
                    foreach (FilterItem item in filterItems)
                    {
                        if (!item.IsChecked)
                            item.IsChecked = true;
                    }
                }
                else
                {
                    int countIsChecked = filterItems.Count(i => i.IsChecked);
                    if (countIsChecked == filterItems.Count - 1)
                    {
                        foreach (FilterItem item in filterItems)
                            item.IsChecked = false;
                    }
                }
            }
            else
            {
                if (filterItem.IsChecked == false && filterItems[0].IsChecked)
                {
                    filterItems[0].IsChecked = false;
                }
            }
        }

        private async Task LoadData(List<Classroom> classrooms)
        {
            var classIds = classrooms.Select(c => c.Id).Distinct().ToList();
            List<StudentAssignment>? studentAssignments = (await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => classIds.Contains(sa.ClassroomId),
                                                                                                                             query => query.Include(sa => sa.Student))).ToList();
            if (studentAssignments == null || studentAssignments.Count == 0)
                return;

            foreach (var studentAssignment in studentAssignments)
            {
                Classroom classroom = classrooms.First(c => c.Id == studentAssignment.ClassroomId);
                string gradeName = classroom.Grade.Name;
                string className = classroom.Name;

                string studentName = studentAssignment.Student.Name;
                string studentGender = studentAssignment.Student.Gender;
                string studentBirthday = studentAssignment.Student.Birthday.ToString("yyyy-MM-dd");
                string studentAddress = studentAssignment.Student.Address;
                string studentPhoneNumber = studentAssignment.Student.Phone;
                string studentEmail = studentAssignment.Student.Email;

                double semester1Score = 0.0;
                

                StudentDataTable.Rows.Add(gradeName, className, studentName, studentGender, studentBirthday, studentAddress, studentPhoneNumber, studentEmail, 0, 0);
            }
        }

        private async void OnFilterChange()
        {
            var countGradeFilter = GradeFilter.Count(i => i.IsChecked);

            if (string.IsNullOrEmpty(SelectedSchoolYear) || countGradeFilter == 0)
                return;

            List<Grade> grades = GradeFilter.Where(i => i.IsChecked).Select(i => GradeDic[i.Name]).ToList();
            await LoadClassroomFilter(SchoolYearDic[SelectedSchoolYear], grades);

            List<Classroom> classrooms = new List<Classroom>();
            foreach (var filterItem in ClassFilter)
            {
                if (filterItem.IsChecked)
                    classrooms.Add(ClassroomDic[filterItem.Name]);
            }

            await LoadData(classrooms);
        }

        public void OnSearchTextChange()
        {
            TableView = SearchAllColumns(SearchText, false, false);
        }

        public DataView SearchAllColumns(string searchText, bool exactMatch = false, bool caseSensitive = false)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return StudentDataTable.DefaultView;
            }

            var culture = new CultureInfo("vi-VN");
            var compareInfo = culture.CompareInfo;
            var compareOptions = caseSensitive ? CompareOptions.None : CompareOptions.IgnoreCase;

            var filteredRows = StudentDataTable.AsEnumerable()
                .Where(row => row.ItemArray.Any(cell =>
                {
                    if (cell == null)
                        return false;

                    string? cellValue = cell.ToString();
                    if (string.IsNullOrEmpty(cellValue))
                        return false;

                    if (exactMatch)
                        return compareInfo.Compare(cellValue, searchText, compareOptions) == 0;
                    else
                        return compareInfo.IndexOf(cellValue, searchText, compareOptions) >= 0;
                }));

            return filteredRows.Any() ? filteredRows.CopyToDataTable().DefaultView : new DataView(StudentDataTable.Clone());
        }
    }
}