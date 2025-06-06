using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Helpers;
using Stumana.WPF.ViewModels.PopupModels;

namespace Stumana.WPF.ViewModels.MainViewModels.ClassOption
{
    public class ClassListViewModel : BaseViewModel
    {
        #region Commands

        public ICommand FilterGradeCommand { get; set; }
        public ICommand AddStudentToClassCommand { get; set; }
        public ICommand TransferStudentCommand { get; set; }
        public ICommand DeleteStudentCommand { get; set; }
        public ICommand AddClassroomCommand { get; set; }
        public ICommand EditClassroomCommand { get; set; }
        public ICommand DeleteClassroomCommand { get; set; }

        #endregion Commands

        #region Properties

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

        private Dictionary<string, Classroom> ClassroomDic { get; set; } = new();
        private DataRowView? _selectedClass;

        public DataRowView? SelectedClass
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

        public DataTable ClassDataTable { get; set; } = new DataTable();
        private DataView _classTableView;

        public DataView ClassTableView
        {
            get => _classTableView;
            set
            {
                _classTableView = value;
                OnPropertyChanged();
            }
        }

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

        #endregion Properties

        public EventHandler? OnClassDataChanged { get; set; }
        public EventHandler? OnStudentDataChanged { get; set; }

        public ClassListViewModel()
        {
            OnClassDataChanged += UpdateClassTable;
            OnStudentDataChanged += UpdateStudentTable;
            OnStudentDataChanged += UpdateClassTable;

            LoadClassTableColumn();
            LoadFilter();

            FilterGradeCommand = new RelayCommand(FilterGrade);
            AddClassroomCommand = new NavigateModalCommand(() => new AddClassroomViewModel(OnClassDataChanged));
            EditClassroomCommand = new NavigateModalCommand(() => new EditClassroomViewModel(ClassroomDic[SelectedClass["Tên lớp"].ToString()], OnClassDataChanged),
                                                            () => SelectedClass != null, "Hãy chọn một lớp để chỉnh sửa");
            DeleteClassroomCommand = new NavigateModalCommand(() => new DeleteConfirmViewModel(DeleteClassroom),
                                                              () => SelectedClass != null, "Hãy chọn một lớp để xóa");
            AddStudentToClassCommand = new NavigateModalCommand(() => new AddStudentToClassViewModel(ClassroomDic[SelectedClass["Tên lớp"].ToString()], OnStudentDataChanged),
                                                                () => SelectedClass != null, "Hãy chọn một lớp để thêm");
            DeleteStudentCommand = new NavigateModalCommand(() => new DeleteConfirmViewModel(DeleteStudent),
                                                            () => SelectedStudent != null, "Hãy chọn một học sinh để xóa khỏi lớp");
        }

        private void LoadFilter()
        {
            LoadSchoolYearFilter();
            LoadGradeFilter();
            DisplayGradeFilterText = ProcessDisplayText(GradeFilter);
            OnSelectionClassroomFilterChange();
        }

        private void LoadClassTableColumn()
        {
            ClassDataTable.Columns.Add("Tên lớp", typeof(string));
            ClassDataTable.Columns.Add("Sĩ số", typeof(int));

            ClassTableView = ClassDataTable.DefaultView;
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

            Classroom classroom = ClassroomDic[SelectedClass["Tên lớp"].ToString()];
            await LoadStudentData(classroom);
        }

        private async void OnSelectionClassroomFilterChange()
        {
            if (string.IsNullOrEmpty(SelectedSchoolYear))
            {
                ClassDataTable.Rows.Clear();
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
            List<Classroom> classrooms = new List<Classroom>();
            var gradeIds = grades.Select(g => g.Id).Distinct();
            classrooms = (await GenericDataService<Classroom>.Instance.GetManyAsync(cl => cl.YearId == schoolYear.Id && gradeIds.Contains(cl.GradeId))).ToList();

            ClassDataTable.Rows.Clear();
            ClassroomDic.Clear();
            foreach (var classroom in classrooms)
            {
                var studentAssignments = await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.ClassroomId == classroom.Id,
                                                                                                           query => query.Include(sa => sa.Student));
                List<Student> students = studentAssignments.GroupBy(sa => sa.StudentId).Select(g => g.First().Student).Where(s => s != null).ToList();
                DataRow dataRow = ClassDataTable.NewRow();
                dataRow["Tên lớp"] = classroom.Name;
                dataRow["Sĩ số"] = students.Count;
                ClassDataTable.Rows.Add(dataRow);
                ClassroomDic[classroom.Name] = classroom;
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

            Classroom classroom = ClassroomDic[SelectedClass["Tên lớp"].ToString()];
            await GenericDataService<Classroom>.Instance.DeleteOneAsync(c => c.Id == classroom.Id);
            ClassroomDic.Remove(classroom.Name);
            ClassDataTable.Rows.Remove(SelectedClass.Row);
            
            Stumana.WPF.Stores.ModalNavigationStore.Instance.Close();
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
            
            Stumana.WPF.Stores.ModalNavigationStore.Instance.Close();
        }

        private void UpdateClassTable(object? sender, EventArgs e)
        {
            OnSelectionClassroomFilterChange();
        }

        private void UpdateStudentTable(object? sender, EventArgs e)
        {
            OnSelectedClassChanged();
        }
    }
}