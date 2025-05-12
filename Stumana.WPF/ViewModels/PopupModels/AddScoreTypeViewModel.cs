using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels
{
    class AddScoreTypeViewModel : BaseViewModel
    {
        private SchoolYear CurSchoolYear { get; set; }

        private string _schoolYearName;

        public string SchoolYearName
        {
            get => _schoolYearName;
            set
            {
                _schoolYearName = value;
                OnPropertyChanged();
            }
        }

        private string _scoreTypeName;

        public string ScoreTypeName
        {
            get => _scoreTypeName;
            set
            {
                _scoreTypeName = value;
                OnPropertyChanged();
            }
        }

        private string _scoreTypeCoefficient;

        public string ScoreTypeCoefficient
        {
            get => _scoreTypeCoefficient;
            set
            {
                _scoreTypeCoefficient = value;
                OnPropertyChanged();
            }
        }

        private bool _isScoreTypeNameInvalid = false;

        public bool IsScoreTypeNameInvalid
        {
            get => _isScoreTypeNameInvalid;
            set
            {
                _isScoreTypeNameInvalid = value;
                OnPropertyChanged();
            }
        }

        private bool _isScoreTypeCoefficientInvalid = false;

        public bool IsScoreTypeCoefficientInvalid
        {
            get => _isScoreTypeCoefficientInvalid;
            set
            {
                _isScoreTypeCoefficientInvalid = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddScoreTypeCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private readonly EventHandler? OnAddedScoreType;

        public AddScoreTypeViewModel(SchoolYear schoolYear, EventHandler? onAddedScoreType)
        {
            OnAddedScoreType = onAddedScoreType;

            CurSchoolYear = schoolYear;
            SchoolYearName = $"{CurSchoolYear.StartYear} - {CurSchoolYear.EndYear}";

            AddScoreTypeCommand = new RelayCommand(AddScoreType);
            CancelCommand = new RelayCommand(ModalNavigationStore.Instance.Close);
        }

        public async void AddScoreType()
        {
            try
            {
                IsScoreTypeNameInvalid = false;
                IsScoreTypeCoefficientInvalid = false;

                if (string.IsNullOrEmpty(ScoreTypeName) || ScoreTypeName.Length > 40)
                    IsScoreTypeNameInvalid = true;

                if (string.IsNullOrEmpty(ScoreTypeCoefficient) || !double.TryParse(ScoreTypeCoefficient, out double coefficient) || coefficient <= 0)
                    IsScoreTypeCoefficientInvalid = true;

                if (IsScoreTypeNameInvalid || IsScoreTypeCoefficientInvalid)
                {
                    throw new Exception("Input không hợp lệ");
                }

                ScoreType newScoreType = new ScoreType
                {
                    Id = Guid.NewGuid().ToString(),
                    YearId = CurSchoolYear.Id,
                    Name = ScoreTypeName,
                    Coefficient = double.Parse(ScoreTypeCoefficient)
                };

                await GenericDataService<ScoreType>.Instance.CreateOneAsync(newScoreType);
                ToastMessageViewModel.ShowSuccessToast("Thêm loại điểm thành công");
                OnAddedScoreType?.Invoke(this, EventArgs.Empty);
                ModalNavigationStore.Instance.Close();
            }
            catch (Exception e)
            {
                ToastMessageViewModel.ShowErrorToast("Thêm năm học không thành công.\n" + e.Message);
                return;
            }
        }
    }
}