using System.Collections.ObjectModel;
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels
{
    class AddClassroomViewModel : BaseViewModel
    {
        private string _classroomName { get; set; }

        public string ClassroomName
        {
            get => _classroomName;
            set
            {
                _classroomName = value;
                OnPropertyChanged();
            }
        }

        private bool _isClassroomNameInvalid { get; set; }

        public bool IsClassroomNameInvalid
        {
            get => _isClassroomNameInvalid;
            set
            {
                _isClassroomNameInvalid = value;
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
            }
        }

        private bool _isSchoolYearInvalid { get; set; }

        public bool IsSchoolYearInvalid
        {
            get => _isSchoolYearInvalid;
            set
            {
                _isSchoolYearInvalid = value;
                OnPropertyChanged();
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
            }
        }

        private bool _isGradeInvalid { get; set; }

        public bool IsGradeInvalid
        {
            get => _isGradeInvalid;
            set
            {
                _isGradeInvalid = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddClassCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private readonly EventHandler? _onAddClassroom;

        public AddClassroomViewModel(EventHandler? onUpdateData)
        {
            _onAddClassroom = onUpdateData;

            AddClassCommand = new RelayCommand(AddClass);
            CancelCommand = new RelayCommand(ModalNavigationStore.Instance.Close);

            LoadSelector();
        }

        public async void AddClass()
        {
            try
            {
                CheckError();
                if (IsClassroomNameInvalid || IsGradeInvalid || IsSchoolYearInvalid)
                    throw new Exception();

                Classroom newClass = new Classroom
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = ClassroomName,
                    YearId = SchoolYearsDic[SelectedSchoolYear].Id,
                    GradeId = GradeDic[SelectedGrade].Id
                };

                await GenericDataService<Classroom>.Instance.CreateOneAsync(newClass);
                ToastMessageViewModel.ShowSuccessToast("Thêm lớp thành công.");
                _onAddClassroom?.Invoke(this, EventArgs.Empty);
                ModalNavigationStore.Instance.Close();
            }
            catch (Exception e)
            {
                ToastMessageViewModel.ShowErrorToast("Thêm lớp không thành công");
            }
        }

        private async void CheckError()
        {
            IsSchoolYearInvalid = false;
            IsGradeInvalid = false;
            IsClassroomNameInvalid = false;

            if (string.IsNullOrEmpty(SelectedSchoolYear))
                IsSchoolYearInvalid = true;

            if (string.IsNullOrEmpty(SelectedGrade))
                IsGradeInvalid = true;

            if (string.IsNullOrEmpty(ClassroomName) || ClassroomName.Length > 120)
                IsClassroomNameInvalid = true;
        }

        private async void LoadSelector()
        {
            await LoadSchoolYearSelector();
            await LoadGradeSelector();
        }

        private async Task LoadSchoolYearSelector()
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

        private async Task LoadGradeSelector()
        {
            GradeCollection = new ObservableCollection<string>();
            var grades = await GenericDataService<Grade>.Instance.GetAllAsync();
            foreach (Grade grade in grades)
            {
                GradeCollection.Add(grade.Name);
                GradeDic.Add(grade.Name, grade);
            }
        }
    }
}