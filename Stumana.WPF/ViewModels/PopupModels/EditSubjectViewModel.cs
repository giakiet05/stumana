using System.Collections.ObjectModel;
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels
{
    public class EditSubjectViewModel : BaseViewModel
    {
        private readonly Subject _originalSubject;

        private string _subjectId;

        public string SubjectId
        {
            get => _subjectId;
            set
            {
                _subjectId = value;
                OnPropertyChanged();
            }
        }

        private string _subjectName;

        public string SubjectName
        {
            get => _subjectName;
            set
            {
                _subjectName = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, SchoolYear> SchoolYearsDic { get; set; } = new Dictionary<string, SchoolYear>();
        public ObservableCollection<string> SchoolYearCollection { get; set; } = new();

        private string _selectedSchoolYear;

        public string SelectedSchoolYear
        {
            get => _selectedSchoolYear;
            set
            {
                _selectedSchoolYear = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, Grade> GradeDic { get; set; } = new Dictionary<string, Grade>();
        public ObservableCollection<string> GradeCollection { get; set; } = new();

        private string _selectedGrade;

        public string SelectedGrade
        {
            get => _selectedGrade;
            set
            {
                _selectedGrade = value;
                OnPropertyChanged();
            }
        }

        private string _subjectPassScore;

        public string SubjectPassScore
        {
            get => _subjectPassScore;
            set
            {
                _subjectPassScore = value;
                OnPropertyChanged();
            }
        }

        private bool _isSubjectIdInvalid;

        public bool IsSubjectIdInvalid
        {
            get => _isSubjectIdInvalid;
            set
            {
                _isSubjectIdInvalid = value;
                OnPropertyChanged();
            }
        }

        private bool _isSubjectNameInvalid;

        public bool IsSubjectNameInvalid
        {
            get => _isSubjectNameInvalid;
            set
            {
                _isSubjectNameInvalid = value;
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

        private bool _isSubjectPassScoreInvalid;

        public bool IsSubjectPassScoreInvalid
        {
            get => _isSubjectPassScoreInvalid;
            set
            {
                _isSubjectPassScoreInvalid = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateSubjectCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        private readonly EventHandler? _onUpdateSubject;

        public EditSubjectViewModel(Subject subject, EventHandler? onUpdateData)
        {
            _originalSubject = subject;
            _onUpdateSubject = onUpdateData;

            UpdateSubjectCommand = new RelayCommand(UpdateSubject);
            CancelCommand = new RelayCommand(ModalNavigationStore.Instance.Close);

            LoadSelector();
            LoadExistingData();
        }

        private async void UpdateSubject()
        {
            try
            {
                CheckError();
                if (IsSubjectIdInvalid || IsSubjectNameInvalid || IsGradeInvalid || IsSchoolYearInvalid || IsSubjectPassScoreInvalid)
                    throw new Exception("Input không hợp lệ");

                _originalSubject.Name = SubjectName;
                _originalSubject.ScoreToPass = double.Parse(SubjectPassScore);
                _originalSubject.YearId = SchoolYearsDic[SelectedSchoolYear].Id;
                _originalSubject.GradeId = GradeDic[SelectedGrade].Id;

                await GenericDataService<Subject>.Instance.UpdateOneAsync(_originalSubject, s => s.Id == _originalSubject.Id);
                ToastMessageViewModel.ShowSuccessToast("Cập nhật môn học thành công.");
                _onUpdateSubject?.Invoke(this, EventArgs.Empty);
                ModalNavigationStore.Instance.Close();
            }
            catch (Exception e)
            {
                ToastMessageViewModel.ShowErrorToast("Cập nhật môn học không thành công.\n" + e.Message);
            }
        }

        private async void CheckError()
        {
            IsSubjectIdInvalid = false;
            IsSubjectNameInvalid = false;
            IsSchoolYearInvalid = false;
            IsGradeInvalid = false;
            IsSubjectPassScoreInvalid = false;

            if (string.IsNullOrEmpty(SelectedSchoolYear))
                IsSchoolYearInvalid = true;

            if (string.IsNullOrEmpty(SelectedGrade))
                IsGradeInvalid = true;

            if (string.IsNullOrEmpty(SubjectName) || SubjectName.Length > 120)
                IsSubjectNameInvalid = true;

            // For editing, we don't validate ID uniqueness since it's readonly
            if (string.IsNullOrEmpty(SubjectId))
                IsSubjectIdInvalid = true;

            if (string.IsNullOrEmpty(SubjectPassScore) || !double.TryParse(SubjectPassScore, out double score) || score > 10 || score < 0)
                IsSubjectPassScoreInvalid = true;
        }

        private async void LoadSelector()
        {
            await LoadSchoolYearSelector();
            await LoadGradeSelector();
        }

        private async void LoadExistingData()
        {
            SubjectId = _originalSubject.Id;
            SubjectName = _originalSubject.Name;
            SubjectPassScore = _originalSubject.ScoreToPass.ToString();

            // Load school year
            var schoolYear = await GenericDataService<SchoolYear>.Instance.GetOneAsync(sy => sy.Id == _originalSubject.YearId);
            if (schoolYear != null)
            {
                SelectedSchoolYear = $"{schoolYear.StartYear}-{schoolYear.EndYear}";
            }

            // Load grade
            var grade = await GenericDataService<Grade>.Instance.GetOneAsync(g => g.Id == _originalSubject.GradeId);
            if (grade != null)
            {
                SelectedGrade = grade.Name;
            }
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