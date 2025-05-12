using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.ViewModels.PopupModels;

namespace Stumana.WPF.ViewModels.MainViewModels.ClassOption
{
    public class ClassListViewModel : BaseViewModel
    {
        #region Commands

        public ICommand AddStudentToClassCommand { get; set; }
        public ICommand DeleteStudentCommand { get; set; }
        public ICommand AddClassroomCommand { get; set; }
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
                OnSelectionClassroomFilterChange();
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

            LoadSchoolYearFilter();
            LoadGradeFilter();

            LoadClassTableColumn();

            AddClassroomCommand = new NavigateModalCommand(() => new AddClassroomViewModel(OnClassDataChanged));
            DeleteClassroomCommand = new RelayCommand(DeleteClassroom);
            AddStudentToClassCommand = new NavigateModalCommand(() => new AddStudentToClassViewModel(ClassroomDic[SelectedClass["Tên lớp"].ToString()], OnStudentDataChanged),
                                                                () => SelectedClass != null, "Hãy chọn một lớp để thêm");
            DeleteStudentCommand = new RelayCommand(DeleteStudent);
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

            Grade? grade = null;
            if (!string.IsNullOrEmpty(SelectedGrade))
                grade = GradeDic[SelectedGrade];

            LoadClassTable(schoolYear, grade);
        }

        private async void LoadClassTable(SchoolYear schoolYear, Grade? grade)
        {
            List<Classroom> classrooms = new List<Classroom>();

            classrooms = (await GenericDataService<Classroom>.Instance.GetManyAsync(cl => cl.YearId == schoolYear.Id)).ToList();

            if (grade != null)
                classrooms = classrooms.Where(cl => cl.GradeId == grade.Id).ToList();

            ClassDataTable.Rows.Clear();
            ClassroomDic.Clear();
            foreach (var classroom in classrooms)
            {
                var studentAssignments = await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.ClassroomId == classroom.Id,
                                                                                                           query => query.Include(sa => sa.Student));
                List<Student> students = studentAssignments.GroupBy(sa => sa.StudentId).Select(g => g.First().Student).Where(s => s != null).ToList();
                ClassDataTable.Rows.Add(classroom.Name, students.Count());
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
        }

        private async void LoadGradeFilter()
        {
            GradeCollection = new ObservableCollection<string>();
            var grades = await GenericDataService<Grade>.Instance.GetAllAsync();
            foreach (Grade grade in grades)
            {
                GradeCollection.Add(grade.Name);
                GradeDic.Add(grade.Name, grade);
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