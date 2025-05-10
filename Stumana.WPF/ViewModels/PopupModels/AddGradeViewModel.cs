using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels
{
    class AddGradeViewModel : BaseViewModel
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
        public ICommand AddGradeCommand { get; set; }

        public AddGradeViewModel()
        {
            AddGradeCommand = new RelayCommand(AddGrade);
            CancelCommand = new RelayCommand(() => ModalNavigationStore.Instance.Close());

            GenerateGradeID();
        }

        private async void GenerateGradeID()
        {
            string idPrefix = "GR";

            Grade? lastGrade = null;
            lastGrade = (await GenericDataService<Grade>.Instance.GetAllAsync()).OrderByDescending(g => int.Parse(g.Id.Substring(idPrefix.Length))).FirstOrDefault();
            int idCount = 1;
            if (lastGrade != null)
            {
                idCount = int.Parse(lastGrade.Id.Substring(idPrefix.Length));
                idCount++;
            }

            GradeId = idPrefix + idCount.ToString("D3");
        }

        private async void AddGrade()
        {
            IsGradeNameInvalid = false;
            if (string.IsNullOrEmpty(GradeName) || GradeName.Length > 30)
            {
                IsGradeNameInvalid = true;
                ToastMessageViewModel.ShowErrorToast("Thêm khối không thành công");
                return;
            }

            Grade newGrade = new Grade
            {
                Id = GradeId,
                Name = GradeName
            };
            await GenericDataService<Grade>.Instance.CreateOneAsync(newGrade);

            ModalNavigationStore.Instance.Close();
        }
    }
}