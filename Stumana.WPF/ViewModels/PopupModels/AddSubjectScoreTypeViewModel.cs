using System.Collections.ObjectModel;
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels
{
    class AddSubjectScoreTypeViewModel : BaseViewModel
    {
        private Dictionary<string, ScoreType> ScoreTypeDic { get; set; } = new();
        public ObservableCollection<string> ScoreTypeCollection { get; set; } = new();
        private string? _selectedScoreType;

        public string? SelectedScoreType
        {
            get => _selectedScoreType;
            set
            {
                if (_selectedScoreType != value)
                {
                    _selectedScoreType = value;
                    OnPropertyChanged();

                    if (_selectedScoreType != null)
                        Coefficient = ScoreTypeDic[_selectedScoreType].Coefficient;
                }
            }
        }

        private bool _isScoreTypeInvalid;

        public bool IsScoreTypeInvalid
        {
            get => _isScoreTypeInvalid;
            set
            {
                _isScoreTypeInvalid = value;
                OnPropertyChanged();
            }
        }

        private double _coefficient;

        public double Coefficient
        {
            get => _coefficient;
            set
            {
                _coefficient = value;
                OnPropertyChanged();
            }
        }

        private string _amount;

        public string Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        private bool _isAmountInvalid;

        public bool IsAmountInvalid
        {
            get => _isAmountInvalid;
            set
            {
                _isAmountInvalid = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddSubjectScoreTypeCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private readonly Subject CurSubject;
        private readonly EventHandler? OnAddedData;

        public AddSubjectScoreTypeViewModel(Subject subject, EventHandler? onAddedData)
        {
            CurSubject = subject;
            OnAddedData = onAddedData;

            AddSubjectScoreTypeCommand = new RelayCommand(AddSubjectScoreType);
            CancelCommand = new RelayCommand(ModalNavigationStore.Instance.Close);

            LoadScoreTypeData();
        }

        private async void LoadScoreTypeData()
        {
            var scoreType = await GenericDataService<ScoreType>.Instance.GetManyAsync(st => st.YearId == CurSubject.YearId);
            var subjectScoreTypeId = (await GenericDataService<SubjectScoreType>.Instance.GetManyAsync(sst => sst.SubjectId == CurSubject.Id)).Select(sst => sst.ScoreTypeId).Distinct();

            scoreType = scoreType.Where(st => !subjectScoreTypeId.Contains(st.Id));

            ScoreTypeCollection.Clear();
            foreach (ScoreType type in scoreType)
            {
                ScoreTypeCollection.Add(type.Name);
                ScoreTypeDic[type.Name] = type;
            }
        }

        private async void AddSubjectScoreType()
        {
            try
            {
                IsAmountInvalid = false;
                IsScoreTypeInvalid = false;

                if (string.IsNullOrEmpty(SelectedScoreType))
                    IsScoreTypeInvalid = true;

                if (string.IsNullOrEmpty(Amount) || !int.TryParse(Amount, out int amount) || amount <= 0 || amount > 50)
                    IsAmountInvalid = true;

                if (IsScoreTypeInvalid || IsAmountInvalid)
                    throw new Exception("Input không hợp lệ");

                SubjectScoreType newSubjectScoreType = new SubjectScoreType
                {
                    Id = Guid.NewGuid().ToString(),
                    Amount = int.Parse(Amount),
                    SubjectId = CurSubject.Id,
                    ScoreTypeId = ScoreTypeDic[SelectedScoreType].Id
                };

                await GenericDataService<SubjectScoreType>.Instance.CreateOneAsync(newSubjectScoreType);
                ToastMessageViewModel.ShowSuccessToast("Thêm loại điểm vào môn thành công");
                OnAddedData?.Invoke(this, EventArgs.Empty);
                ModalNavigationStore.Instance.Close();
            }
            catch (Exception e)
            {
                ToastMessageViewModel.ShowErrorToast("Thêm loại điểm vào môn không thành công.\n" + e.Message);
                return;
            }
        }
    }
}
