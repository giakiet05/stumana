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

        public DataTable StudentDataTable { get; set; } = new DataTable();
        private DataView _studentTableView;

        public DataView StudentTableView
        {
            get => _studentTableView;
            set
            {
                _studentTableView = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        public EventHandler? OnUpdateData;

        public ClassListViewModel()
        {
            OnUpdateData += UpdateTable;

            LoadSchoolYearFilter();
            LoadGradeFilter();

            LoadClassTableColumn();
            LoadStudentTableColumn();

            AddClassroomCommand = new NavigateModalCommand(() => new AddClassroomViewModel(OnUpdateData));
            DeleteClassroomCommand = new RelayCommand(DeleteClassroom);
            AddStudentToClassCommand = new NavigateModalCommand(() => new AddStudentToClassViewModel(ClassroomDic[SelectedClass["Tên lớp"].ToString()]), HaveSelectClass);
        }

        private void LoadClassTableColumn()
        {
            ClassDataTable.Columns.Add("Tên lớp", typeof(string));
            ClassDataTable.Columns.Add("Sĩ số", typeof(int));

            ClassTableView = ClassDataTable.DefaultView;
        }

        private void LoadStudentTableColumn()
        {
            StudentDataTable.Columns.Add("Mã học sinh", typeof(string));
            StudentDataTable.Columns.Add("Họ tên", typeof(string));
            StudentDataTable.Columns.Add("Giới tính", typeof(string));
            StudentDataTable.Columns.Add("Năm sinh", typeof(string));
            StudentDataTable.Columns.Add("Địa chỉ", typeof(string));

            StudentTableView = StudentDataTable.DefaultView;
        }

        private async Task LoadStudentData(Classroom classroom)
        {
            var studentAssignments = await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.ClassroomId == classroom.Id,
                                                                                                       query => query.Include(sa => sa.Student));
            StudentDataTable.Rows.Clear();
            foreach (var studentAssignment in studentAssignments)
            {
                DataRow newRow = StudentDataTable.NewRow();

                newRow["Mã học sinh"] = studentAssignment.StudentId;
                newRow["Họ tên"] = studentAssignment.Student.Name;
                newRow["Giới tính"] = studentAssignment.Student.Gender;
                newRow["Năm sinh"] = studentAssignment.Student.Birthday.Year;
                newRow["Địa chỉ"] = studentAssignment.Student.Address;

                StudentDataTable.Rows.Add(newRow);
            }
        }

        private async void OnSelectedClassChanged()
        {
            if (SelectedClass == null)
                return;

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
                var studentAssignment = await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.ClassroomId == classroom.Id);
                ClassDataTable.Rows.Add(classroom.Name, studentAssignment.Count());
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

            await GenericDataService<Classroom>.Instance.DeleteOneAsync(c => c.Id == ClassroomDic[SelectedClass["Tên lớp"].ToString()].Id);
            ClassroomDic.Remove(SelectedClass["Tên lớp"].ToString());
            ClassDataTable.Rows.Remove(SelectedClass.Row);
        }

        private void UpdateTable(object? sender, EventArgs e)
        {
            OnSelectionClassroomFilterChange();
            OnSelectedClassChanged();
        }

        private bool HaveSelectClass()
        {
            if (SelectedClass == null)
            {
                ToastMessageViewModel.ShowErrorToast("Chưa chọn lớp");
                return false;
            }

            return true;
        }
    }
}