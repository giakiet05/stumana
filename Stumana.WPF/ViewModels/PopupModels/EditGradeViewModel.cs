using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;
using Stumana.WPF.ViewModels.MainViewModels.YearOption;

namespace Stumana.WPF.ViewModels.PopupModels
{
    public class EditGradeViewModel : BaseViewModel
    {
        private string _gradeId;

        public string GradeId
        {
            get => _gradeId;
            set
            {
                _gradeId = value;
                OnPropertyChanged();
            }
        }

        private string _gradeName;

        public string GradeName
        {
            get => _gradeName;
            set
            {
                _gradeName = value;
                OnPropertyChanged();
            }
        }

        private bool _isGradeNameInvalid = false;

        public bool IsGradeNameInvalid
        {
            get => _isGradeNameInvalid;
            set
            {
                _isGradeNameInvalid = value;
                OnPropertyChanged();
            }
        }

        public ICommand CancelCommand { get; set; }
        public ICommand EditGradeCommand { get; set; }
        private readonly EventHandler? _onEditGrade;
        private readonly YearViewModel.GradeTableRow _selectedGrade;

        public EditGradeViewModel(YearViewModel.GradeTableRow selectedGrade, EventHandler? updateTable)
        {
            _selectedGrade = selectedGrade;
            _onEditGrade = updateTable;

            EditGradeCommand = new RelayCommand(EditGrade);
            CancelCommand = new RelayCommand(() => ModalNavigationStore.Instance.Close());

            LoadGradeData();
        }

        private async void LoadGradeData()
        {
            if (_selectedGrade == null)
                return;

            GradeId = _selectedGrade.GradeID;

            // Load Grade data
            var grade = await GenericDataService<Grade>.Instance.GetOneAsync(g => g.Id == _selectedGrade.GradeID);
            if (grade != null)
            {
                GradeName = grade.Name;
            }
        }

        private async void EditGrade()
        {
            try
            {
                IsGradeNameInvalid = false;
                if (string.IsNullOrEmpty(GradeName) || GradeName.Length > 30)
                {
                    IsGradeNameInvalid = true;
                    ToastMessageViewModel.ShowErrorToast("Cập nhật khối không thành công");
                    return;
                }

                // Update Grade
                var grade = await GenericDataService<Grade>.Instance.GetOneAsync(g => g.Id == GradeId);
                if (grade != null)
                {
                    grade.Name = GradeName;
                    await GenericDataService<Grade>.Instance.UpdateOneAsync(grade, g => g.Id == grade.Id);
                }

                _onEditGrade?.Invoke(this, EventArgs.Empty);
                ModalNavigationStore.Instance.Close();
                ToastMessageViewModel.ShowSuccessToast("Cập nhật khối lớp thành công");
            }
            catch (Exception e)
            {
                ToastMessageViewModel.ShowErrorToast("Cập nhật khối lớp không thành công. " + e.Message);
            }
        }
    }
} 