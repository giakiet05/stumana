using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Helpers;
using Stumana.WPF.ViewModels.PopupModels;

namespace Stumana.WPF.ViewModels.MainViewModels.StudentOption
{
    public class StudentViewModel : BaseViewModel
    {
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

        private bool IsProcessingFilter { get; set; } = false;

        public Dictionary<string, SchoolYear> SchoolYearDic { get; set; } = new();
        public Dictionary<string, Grade> GradeDic { get; set; } = new();
        public Dictionary<string, Classroom> ClassroomDic { get; set; } = new();

        private List<Student> OriginalStudentTable { get; set; } = new();
        public ObservableCollection<Student> StudentTable { get; set; } = new();

        private Student? _selectedStudent;

        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged();
            }
        }

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
                    LoadClassroomFilter();
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

        private string _displayGradeFilterText = String.Empty;

        public string DisplayGradeFilterText
        {
            get => _displayGradeFilterText;
            set
            {
                _displayGradeFilterText = value;
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

        private string _displayClassFilterText = String.Empty;

        public string DisplayClassFilterText
        {
            get => _displayClassFilterText;
            set
            {
                _displayClassFilterText = value;
                OnPropertyChanged();
            }
        }

        public ICommand FilterGradeCommand { get; set; }
        public ICommand FilterClassroomCommand { get; set; }
        public ICommand AddStudentCommand { get; set; }
        public ICommand DeleteStudentCommand { get; set; }
        public ICommand EditStudentCommand { get; set; }

        public EventHandler? OnStudentDataChanged { get; set; }

        public StudentViewModel()
        {
            OnStudentDataChanged += UpdateStudentTable;

            FilterGradeCommand = new RelayCommand(FilterGrade);
            FilterClassroomCommand = new RelayCommand(FilterClassroom);
            AddStudentCommand = new NavigateModalCommand(() => new AddStudentViewModel(OnStudentDataChanged));
            DeleteStudentCommand = new RelayCommand(DeleteStudent);
            EditStudentCommand = new NavigateModalCommand(() => new EditStudentViewModel(SelectedStudent, OnStudentDataChanged),
                                                          () => SelectedStudent != null, "Hãy chọn một học sinh để chỉnh sửa.");

            LoadInitFilter();
        }

        private async void LoadInitFilter()
        {
            await LoadSchoolYearFilter();
            await LoadGradeFilter();
            LoadClassroomFilter();
            DisplayGradeFilterText = ProcessDisplayText(GradeFilter, "Khối*");

            OnFilterChange();
            DisplayClassFilterText = ProcessDisplayText(ClassFilter);
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
            if (SchoolYearFilter.Any())
                SelectedSchoolYear = SchoolYearFilter[0];
        }

        public async Task LoadGradeFilter()
        {
            GradeFilter.Clear();

            var grades = (await GenericDataService<Grade>.Instance.GetAllAsync()).ToList();
            if (!grades.Any())
                return;

            GradeFilter.Add(new FilterItem("All", true));
            foreach (Grade grade in grades)
            {
                string gradeName = $"{grade.Name}";
                GradeFilter.Add(new FilterItem(gradeName, true));
                GradeDic.Add(gradeName, grade);
            }
        }

        public async void LoadClassroomFilter()
        {
            if (SelectedSchoolYear == null || GradeFilter.Count(i => i.IsChecked) == 0)
                return;

            SchoolYear schoolYear = SchoolYearDic[SelectedSchoolYear];
            List<Grade> grades = new List<Grade>();
            foreach (FilterItem item in GradeFilter)
            {
                if (item.IsChecked && item.Name != "All")
                    grades.Add(GradeDic[item.Name]);
            }

            ClassFilter.Clear();
            var gradeId = grades.Select(g => g.Id);
            List<Classroom> classrooms = (await GenericDataService<Classroom>.Instance.GetManyAsync(c => c.YearId == schoolYear.Id && gradeId.Contains(c.GradeId))).ToList();

            if (!classrooms.Any())
                return;

            ClassroomDic.Clear();
            ClassFilter.Add(new FilterItem("All", true));
            classrooms = classrooms.OrderByDescending(c => c.Name).ToList();
            foreach (Classroom classroom in classrooms)
            {
                string className = $"Lớp {classroom.Name}";
                ClassFilter.Add(new FilterItem(className, true));
                ClassroomDic[className] = classroom;
            }

            ClassFilter.Add(new FilterItem("Chưa có lớp", true));
        }

        private async void FilterGrade(object param)
        {
            if (IsProcessingFilter) 
                return;

            try
            {
                IsProcessingFilter = true;
                FilterItem filterItem = (FilterItem)param;
                ProcessFilterItemSelection(filterItem, GradeFilter);
                DisplayGradeFilterText = ProcessDisplayText(GradeFilter, "Khối*");
                LoadClassroomFilter();
            }
            finally
            {
                IsProcessingFilter = false;
            }
        }

        private void FilterClassroom(object param)
        {
            if (IsProcessingFilter) 
                return;
    
            try
            {
                IsProcessingFilter = true;
                FilterItem filterItem = (FilterItem)param;
                ProcessFilterItemSelection(filterItem, ClassFilter);
                DisplayClassFilterText = ProcessDisplayText(ClassFilter, "Lớp");
                OnFilterChange();
            }
            finally
            {
                IsProcessingFilter = false;
            }
        }

        private string ProcessDisplayText(ObservableCollection<FilterItem> filterItems, string defaultText = "")
        {
            int selectionCount = filterItems.Count(i => i.IsChecked);
            if (selectionCount == 0)
                return defaultText;

            string displayText;
            if (selectionCount < filterItems.Count - 1)
                displayText = $"{selectionCount} được chọn";
            else
                displayText = "Tất cả";
            return displayText;
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
                    filterItems[0].IsChecked = false;

                if (filterItem.IsChecked && filterItems.Count(i => i.IsChecked) >= filterItems.Count - 1)
                    filterItems[0].IsChecked = true;
            }
        }

        private void UpdateStudentTable(object? sender, EventArgs e)
        {
            OnFilterChange();
        }

        private async void OnFilterChange()
        {
            List<Classroom> classrooms = new List<Classroom>();
            bool haveNoClassStudent = false;
            foreach (var filterItem in ClassFilter)
            {
                if (filterItem.IsChecked && filterItem.Name == "Chưa có lớp")
                {
                    haveNoClassStudent = true;
                    continue;
                }

                if (filterItem.IsChecked && filterItem.Name != "All")
                    classrooms.Add(ClassroomDic[filterItem.Name]);
            }

            await LoadStudentData(classrooms, haveNoClassStudent);
        }

        private async Task LoadStudentData(List<Classroom> classrooms, bool haveNoClassStudent)
        {
            if (classrooms.Count == 0 && !haveNoClassStudent)
                return;

            var classIds = classrooms.Select(c => c.Id).Distinct().ToList();
            List<StudentAssignment> studentAssignments = (await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => classIds.Contains(sa.ClassroomId),
                                                                                                                            query => query.Include(sa => sa.Student))).ToList();

            List<Student> studentsWithClass = studentAssignments.GroupBy(sa => sa.StudentId).Select(g => g.First().Student).Where(s => s != null).ToList();


            List<Student> studentsWithNoClass = new List<Student>();
            if (haveNoClassStudent)
            {
                var studentWithClassId = studentsWithClass.Select(s => s.Id).Distinct();
                studentsWithNoClass = (await GenericDataService<Student>.Instance.GetManyAsync(s => !studentWithClassId.Contains(s.Id))).ToList();
            }

            List<Student> students = new List<Student>(studentsWithClass);
            students.AddRange(studentsWithNoClass);

            OriginalStudentTable.Clear();
            StudentTable.Clear();
            foreach (var student in students)
            {
                StudentTable.Add(student);
                OriginalStudentTable.Add(student);
            }
        }

        private async void DeleteStudent()
        {
            if (SelectedStudent == null)
            {
                ToastMessageViewModel.ShowErrorToast("Hãy chọn một học sinh để xóa");
                return;
            }

            await GenericDataService<Student>.Instance.DeleteOneAsync(s => s.Id == SelectedStudent.Id);
            StudentTable.Remove(SelectedStudent);
            OriginalStudentTable.Remove(SelectedStudent);
            ToastMessageViewModel.ShowSuccessToast("Xóa học sinh thành công");
        }

        public void OnSearchTextChange()
        {
            SearchAllColumns(OriginalStudentTable, StudentTable, SearchText, false, false);
        }

        public void SearchAllColumns<T>(List<T> originalList, ObservableCollection<T> table, string searchText, bool exactMatch = false, bool caseSensitive = false)
        {
            if (originalList == null)
                return;

            var culture = new CultureInfo("vi-VN");
            var compareInfo = culture.CompareInfo;
            var compareOptions = caseSensitive ? CompareOptions.None : CompareOptions.IgnoreCase;

            IEnumerable<T> filtered;

            if (string.IsNullOrEmpty(searchText))
                filtered = originalList;
            else
            {
                filtered = originalList.Where(s =>
                {
                    bool Match(string? value) => !string.IsNullOrEmpty(value) && (exactMatch ? compareInfo.Compare(value, searchText, compareOptions) == 0 : compareInfo.IndexOf(value, searchText, compareOptions) >= 0);
                    PropertyInfo[] properties = typeof(T).GetProperties();
                    bool IsMatch = false;

                    foreach (var property in properties)
                    {
                        var rawValue = property.GetValue(s);
                        if (rawValue == null)
                            continue;

                        string? stringValue = rawValue is DateTime dt ? dt.ToString("dd/MM/yyyy", culture) : rawValue.ToString();
                        IsMatch = IsMatch || Match(stringValue);
                    }

                    return IsMatch;
                });
            }

            table.Clear();
            foreach (var row in filtered)
            {
                table.Add(row);
            }
        }
    }
}