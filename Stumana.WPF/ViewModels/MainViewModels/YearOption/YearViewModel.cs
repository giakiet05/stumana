using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.ViewModels.PopupModels;

namespace Stumana.WPF.ViewModels.MainViewModels.YearOption
{
    public class YearViewModel : BaseViewModel
    {
        private List<YearTableRow> YearList { get; set; } = new();
        public ObservableCollection<YearTableRow> YearTable { get; set; } = new();

        private YearTableRow? _selectedYear;

        public YearTableRow? SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged();
            }
        }

        private string _searchTextYear;

        public string SearchTextYear
        {
            get => _searchTextYear;
            set
            {
                _searchTextYear = value;
                OnPropertyChanged();
                OnSearchTextChange();
            }
        }


        private List<GradeTableRow> GradeList { get; set; } = new();
        public ObservableCollection<GradeTableRow> GradeTable { get; set; } = new();

        private GradeTableRow? _selectedGrade;

        public GradeTableRow? SelectedGrade
        {
            get => _selectedGrade;
            set
            {
                _selectedGrade = value;
                OnPropertyChanged();
            }
        }

        private string _searchTextGrade;

        public string SearchTextGrade
        {
            get => _searchTextGrade;
            set
            {
                _searchTextGrade = value;
                OnPropertyChanged();
                OnSearchTextChange();
            }
        }

        public ICommand AddYearCommand { get; set; }
        public ICommand DeleteYearCommand { get; set; }
        public ICommand AddGradeCommand { get; set; }
        public ICommand DeleteGradeCommand { get; set; }

        public YearViewModel()
        {
            AddYearCommand = new NavigateModalCommand(() => new AddYearViewModel());
            DeleteYearCommand = new RelayCommand(DeleteYear);
            AddGradeCommand = new NavigateModalCommand(() => new AddGradeViewModel());
            DeleteGradeCommand = new RelayCommand(DeleteGrade);

            LoadData();
        }

        private async void LoadData()
        {
            await LoadYearTable();
            await LoadGradeTable();
        }

        private async Task LoadYearTable()
        {
            var years = (await GenericDataService<SchoolYear>.Instance.GetAllAsync()).ToList();

            foreach (var year in years)
            {
                var row = new YearTableRow(year.Id, $"{year.StartYear}-{year.EndYear}");
                YearTable.Add(row);
                YearList.Add(row);
            }
        }

        private async Task LoadGradeTable()
        {
            var grades = (await GenericDataService<Grade>.Instance.GetAllAsync());

            foreach (var grade in grades)
            {
                var row = new GradeTableRow(grade.Id, grade.Name);
                GradeTable.Add(row);
                GradeList.Add(row);
            }
        }

        private async void DeleteYear()
        {
            if (SelectedYear == null)
            {
                ToastMessageViewModel.ShowErrorToast("Hãy chọn một năm để xóa");
                return;
            }

            YearTable.Remove(SelectedYear);
            YearList.Remove(SelectedYear);
            await GenericDataService<SchoolYear>.Instance.DeleteOneAsync(sc => sc.Id == SelectedYear.YearID);
            SelectedYear = null;
        }

        private async void DeleteGrade()
        {
            if (SelectedGrade == null)
            {
                ToastMessageViewModel.ShowErrorToast("Hãy chọn một khối để xóa");
                return;
            }

            GradeTable.Remove(SelectedGrade);
            GradeList.Remove(SelectedGrade);
            await GenericDataService<Grade>.Instance.DeleteOneAsync(g => g.Id == SelectedGrade.GradeID);
            SelectedGrade = null;
        }

        public void OnSearchTextChange()
        {
            SearchAllColumns(YearList, YearTable, SearchTextYear, false, false);
            SearchAllColumns(GradeList, GradeTable, SearchTextGrade, false, false);
        }

        public void SearchAllColumns<T>(List<T> originalList, ObservableCollection<T> table, string searchText, bool exactMatch = false, bool caseSensitive = false)
        {
            if (originalList == null)
                return;

            var culture = new CultureInfo("vi-VN");
            var compareInfo = culture.CompareInfo;
            var compareOptions = caseSensitive ? CompareOptions.None : CompareOptions.IgnoreCase;

            IEnumerable<T> filtered;

            if (string.IsNullOrEmpty(searchText))
                filtered = originalList;
            else
            {
                filtered = originalList.Where(s =>
                {
                    bool Match(string? value) => !string.IsNullOrEmpty(value) && (exactMatch ? compareInfo.Compare(value, searchText, compareOptions) == 0 : compareInfo.IndexOf(value, searchText, compareOptions) >= 0);
                    PropertyInfo[] properties = typeof(T).GetProperties();
                    bool IsMatch = false;

                    foreach (var property in properties)
                    {
                        var rawValue = property.GetValue(s);
                        if (rawValue == null)
                            continue;

                        string? stringValue = rawValue is DateTime dt ? dt.ToString("dd/MM/yyyy", culture) : rawValue.ToString();
                        IsMatch = IsMatch || Match(stringValue);
                    }

                    return IsMatch;
                });
            }

            table.Clear();
            foreach (var row in filtered)
            {
                table.Add(row);
            }
        }

        public class YearTableRow
        {
            public string YearID;
            public string YearName;

            public YearTableRow(string yearId, string yearName)
            {
                YearID = yearId;
                YearName = yearName;
            }
        }

        public class GradeTableRow
        {
            public string GradeID;
            public string GradeName;

            public GradeTableRow(string gradeId, string gradeName)
            {
                GradeID = gradeId;
                GradeName = gradeName;
            }
        }
    }
}