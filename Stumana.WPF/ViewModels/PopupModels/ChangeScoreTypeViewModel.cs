using System.Collections.ObjectModel;
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Helpers;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels
{
    class ChangeScoreTypeViewModel : BaseViewModel
    {
        public ObservableCollection<ScoreType> ScoreTypeTable { get; set; } = new();

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
                OnFilterChange();
            }
        }

        public ICommand AddScoreTypeCommand { get; set; }
        public ICommand DeleteScoreTypeCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public ChangeScoreTypeViewModel()
        {
            AddScoreTypeCommand = new NavigateModalCommand(() => new AddScoreTypeViewModel());
            CancelCommand = new RelayCommand(ModalNavigationStore.Instance.Close);

            LoadSchoolYearFilter();
        }

        private async void OnFilterChange() { }

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
    }
}