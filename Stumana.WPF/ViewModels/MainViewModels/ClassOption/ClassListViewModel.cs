using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Helpers;
using Stumana.WPF.Stores;
using Stumana.WPF.ViewModels.PopupModels;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Input;

namespace Stumana.WPF.ViewModels.MainViewModels.ClassOption
{
    public class ClassListViewModel : BaseViewModel
    {
        #region Properties

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

        private List<ClassTableRow> OriginalClassTable { get; set; } = new();
        public ObservableCollection<ClassTableRow> ClassDataTable { get; set; } = new();

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

        private Dictionary<string, SchoolYear> SchoolYearsDic { get; set; } = new Dictionary<string, SchoolYear>();
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
                OnSelectionClassroomFilterChange();
            }
        }

        private Dictionary<string, Grade> GradeDic { get; set; } = new Dictionary<string, Grade>();
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

        private ClassTableRow? _selectedClass;

        public ClassTableRow? SelectedClass
        {
            get => _selectedClass;
            set
            {
                if (_selectedClass != value)
                {
                    _selectedClass = value;
                    OnPropertyChanged();
                    OnSelectedClassChanged();
                }
            }
        }

        #endregion Properties

        #region Commands
        public ICommand FilterGradeCommand { get; set; }
        public ICommand AddStudentToClassCommand { get; set; }
        public ICommand DeleteStudentCommand { get; set; }
        public ICommand AddClassroomCommand { get; set; }
        public ICommand DeleteClassroomCommand { get; set; }

        #endregion Commands

        public EventHandler? OnClassDataChanged { get; set; }
        public EventHandler? OnStudentDataChanged { get; set; }

        public ClassListViewModel()
        {
            OnClassDataChanged += UpdateClassTable;
            OnStudentDataChanged += UpdateStudentTable;
            OnStudentDataChanged += UpdateClassTable;

            LoadFilter();

            FilterGradeCommand = new RelayCommand(FilterGrade);
            AddClassroomCommand = new NavigateModalCommand(() => new AddClassroomViewModel(OnClassDataChanged));
           DeleteClassroomCommand = new NavigateModalCommand(()=>new DeleteConfirmViewModel(DeleteClassroom));
            AddStudentToClassCommand = new RelayCommand(() =>
            {
                if (SelectedClass == null)
                {
                    ToastMessageViewModel.ShowErrorToast("Hãy chọn một lớp để thêm học sinh");
                    return;
                }
                else
                {
                    AddStudentToClassViewModel modal = new AddStudentToClassViewModel(SelectedClass.Classroom, OnStudentDataChanged);
                    if (modal.UnassignStudentCount == 0)
                    {
                        ToastMessageViewModel.ShowInfoToast("Không có học sinh nào phù hợp");
                        return;
                    }
                    else
                        ModalNavigationStore.Instance.CurrentModalViewModel = modal;
                }
            });

            // DeleteStudentCommand = new RelayCommand(DeleteStudent);
            DeleteStudentCommand = new NavigateModalCommand(() => new DeleteConfirmViewModel(DeleteStudent));
        }

        private void LoadFilter()
        {
            LoadSchoolYearFilter();
            LoadGradeFilter();
            DisplayGradeFilterText = ProcessDisplayText(GradeFilter);
            OnSelectionClassroomFilterChange();
        }

        private async Task LoadStudentData(Classroom classroom)
        {
            var studentAssignments = await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.ClassroomId == classroom.Id,
                                                                                                       query => query.Include(sa => sa.Student));
            List<Student> students = studentAssignments.GroupBy(sa => sa.StudentId).Select(g => g.First().Student).Where(s => s != null).ToList();
            StudentTable.Clear();
            foreach (var student in students)
            {
                StudentTable.Add(student);
            }
        }

        private async void OnSelectedClassChanged()
        {
            if (SelectedClass == null)
            {
                StudentTable.Clear();
                return;
            }

            Classroom classroom = SelectedClass.Classroom;
            await LoadStudentData(classroom);
        }

        private async void OnSelectionClassroomFilterChange()
        {
            if (string.IsNullOrEmpty(SelectedSchoolYear))
            {
                ClassDataTable.Clear();
                OriginalClassTable.Clear();
                return;
            }

            SchoolYear schoolYear = SchoolYearsDic[SelectedSchoolYear];

            List<Grade> grades = new List<Grade>();
            foreach (FilterItem item in GradeFilter)
            {
                if (item.IsChecked && item.Name != "All")
                    grades.Add(GradeDic[item.Name]);
            }

            LoadClassTable(schoolYear, grades);
        }

        private async void LoadClassTable(SchoolYear schoolYear, List<Grade> grades)
        {
            var gradeIds = grades.Select(g => g.Id).Distinct();
            List<Classroom> classrooms = (await GenericDataService<Classroom>.Instance.GetManyAsync(cl => cl.YearId == schoolYear.Id && gradeIds.Contains(cl.GradeId))).ToList();

            ClassDataTable.Clear();
            OriginalClassTable.Clear();
            foreach (Classroom classroom in classrooms)
            {
                var studentAssignments = await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.ClassroomId == classroom.Id,
                                                                                                           query => query.Include(sa => sa.Student));
                List<Student> students = studentAssignments.GroupBy(sa => sa.StudentId).Select(g => g.First().Student).Where(s => s != null).ToList();

                var newRow = new ClassTableRow
                {
                    Name = classroom.Name,
                    Capacity = students.Count,
                    Classroom = classroom
                };

                ClassDataTable.Add(newRow);
                OriginalClassTable.Add(newRow);
            }
        }

        private async void LoadSchoolYearFilter()
        {
            SchoolYearCollection = new ObservableCollection<string>();
            var sy = await GenericDataService<SchoolYear>.Instance.GetAllAsync();
            foreach (SchoolYear schoolYear in sy)
            {
                string schoolyearstr = schoolYear.StartYear + "-" + schoolYear.EndYear;
                SchoolYearCollection.Add(schoolyearstr);
                SchoolYearsDic.Add(schoolyearstr, schoolYear);
            }

            if (SchoolYearCollection.Any())
                SelectedSchoolYear = SchoolYearCollection[0];
        }

        public async void LoadGradeFilter()
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

        private void FilterGrade(object param)
        {
            FilterItem filterItem = (FilterItem)param;
            ProcessFilterItemSelection(filterItem, GradeFilter);
            DisplayGradeFilterText = ProcessDisplayText(GradeFilter);
            OnSelectionClassroomFilterChange();
        }

        private string ProcessDisplayText(ObservableCollection<FilterItem> filterItems)
        {
            string displayText;
            int selectionCount = filterItems.Count(i => i.IsChecked);
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

        private async void DeleteClassroom()
        {
            if (SelectedClass == null)
            {
                ToastMessageViewModel.ShowErrorToast("Hãy chọn một lớp để xóa");
                return;
            }

            await GenericDataService<Classroom>.Instance.DeleteOneAsync(c => c.Id == SelectedClass.Classroom.Id);
            ClassDataTable.Remove(SelectedClass);
            OriginalClassTable.Remove(SelectedClass);
            ModalNavigationStore.Instance.Close();
        }

        private async void DeleteStudent()
        {
            if (SelectedStudent == null)
            {
                ToastMessageViewModel.ShowErrorToast("Hãy chọn một học sinh để xóa khỏi lớp");
                return;
            }

            var studentAssignmentIds = (await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.StudentId == SelectedStudent.Id)).Select(sa => sa.Id);
            await GenericDataService<StudentAssignment>.Instance.DeleteManyAsync(sa => studentAssignmentIds.Contains(sa.Id));
            ToastMessageViewModel.ShowSuccessToast("Xóa học sinh khỏi lớp thành công");
            StudentTable.Remove(SelectedStudent);
            OnClassDataChanged?.Invoke(this, EventArgs.Empty);
            ModalNavigationStore.Instance.Close();

        }

        private void UpdateClassTable(object? sender, EventArgs e)
        {
            OnSelectionClassroomFilterChange();
        }

        private void UpdateStudentTable(object? sender, EventArgs e)
        {
            OnSelectedClassChanged();
        }

        public void OnSearchTextChange()
        {
            SearchAllColumns(OriginalClassTable, ClassDataTable, SearchText, false, false);
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

    public class ClassTableRow
    {
        public ClassTableRow(Classroom classroom, string name, int capacity)
        {
            Classroom = classroom;
            Name = name;
            Capacity = capacity;
        }

        public ClassTableRow() { }

        public Classroom Classroom { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
    }
}