using System.Collections.ObjectModel;
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels
{
    class EditScoreTypeViewModel : BaseViewModel
    {
        public ObservableCollection<ScoreType> ScoreTypeTable { get; set; } = new();

        private ScoreType? _selectedScoreType;

        public ScoreType? SelectedScoreType
        {
            get => _selectedScoreType;
            set
            {
                _selectedScoreType = value;
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

        private string? _selectedSchoolYear;

        public string? SelectedSchoolYear
        {
            get => _selectedSchoolYear;
            set
            {
                _selectedSchoolYear = value;
                OnPropertyChanged();
                OnFilterChange();
            }
        }

        public ICommand AddScoreTypeCommand { get; set; }
        public ICommand DeleteScoreTypeCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public EventHandler? OnAddedScoreType { get; set; }

        public EditScoreTypeViewModel()
        {
            OnAddedScoreType += UpdateData;

            AddScoreTypeCommand = new NavigateModalCommand(() => new AddScoreTypeViewModel(SchoolYearsDic[SelectedSchoolYear], OnAddedScoreType),
                                                           () => SelectedSchoolYear != null, "Hãy chọn một năm học để thêm");
            DeleteScoreTypeCommand = new NavigateModalCommand(() => new DeleteConfirmViewModel(DeleteScoreType),
                                                              () => SelectedScoreType != null, "Hãy chọn một loại điểm để xóa");
            CancelCommand = new RelayCommand(ModalNavigationStore.Instance.Close);

            LoadSchoolYearFilter();
        }

        private void UpdateData(object? sender, EventArgs e)
        {
            OnFilterChange();
        }

        private async void OnFilterChange()
        {
            if (SelectedSchoolYear == null)
                return;

            await LoadScoreTypeData(SchoolYearsDic[SelectedSchoolYear]);
        }

        private async Task LoadScoreTypeData(SchoolYear schoolYear)
        {
            var scoreTypes = (await GenericDataService<ScoreType>.Instance.GetManyAsync(st => st.YearId == schoolYear.Id)).ToList();

            ScoreTypeTable.Clear();
            foreach (var scoreType in scoreTypes)
                ScoreTypeTable.Add(scoreType);
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

        private async void DeleteScoreType()
        {
            if (SelectedScoreType == null)
            {
                ToastMessageViewModel.ShowErrorToast("Hãy chọn một loại điểm để xóa");
                return;
            }

            ScoreTypeTable.Remove(SelectedScoreType);
            await GenericDataService<ScoreType>.Instance.DeleteOneAsync(st => st.Id == SelectedScoreType.Id);
            SelectedScoreType = null;

            ModalNavigationStore.Instance.Close();
        }
    }
}