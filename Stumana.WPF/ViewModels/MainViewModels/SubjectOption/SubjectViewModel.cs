using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Helpers;
using Stumana.WPF.ViewModels.PopupModels;

namespace Stumana.WPF.ViewModels.MainViewModels.SubjectOption
{
    public class SubjectViewModel : BaseViewModel
    {
        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                OnSearchTextChange();
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
                OnSelectionSubjectFilterChange();
            }
        }

        private Dictionary<string, Grade> GradeDic { get; set; } = new Dictionary<string, Grade>();
        private ObservableCollection<FilterItem> _gradeFilter = new();

        public ObservableCollection<FilterItem> GradeFilter
        {
            get => _gradeFilter;
            set
            {
                _gradeFilter = value;
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
                OnSelectionSubjectFilterChange();
            }
        }

        private List<SubjectTableRow> OriginalSubjectTable { get; set; } = new();
        public ObservableCollection<SubjectTableRow> SubjectTable { get; set; } = new();

        private SubjectTableRow? _selectedSubject;

        public SubjectTableRow? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                if (_selectedSubject != value)
                {
                    _selectedSubject = value;
                    OnPropertyChanged();
                    if (_selectedSubject != null)
                        LoadScoreTypeData(_selectedSubject.SubjectId);
                }
            }
        }

        public ObservableCollection<ScoreTypeTableRow> ScoreTypeTable { get; set; } = new();

        private SubjectTableRow _selectedScoreType;

        public SubjectTableRow SelectedScoreType
        {
            get => _selectedScoreType;
            set
            {
                _selectedScoreType = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddSubjectCommand { get; set; }
        public ICommand DeleteSubjectCommand { get; set; }
        public ICommand ChangeScoreTypeCommand { get; set; }
        public ICommand AddScoreTypeCommand { get; set; }
        public ICommand FilterGradeCommand { get; set; }

        public SubjectViewModel()
        {
            AddSubjectCommand = new NavigateModalCommand(() => new AddSubjectViewModel());
            DeleteSubjectCommand = new RelayCommand(DeleteSubjectRow);
            ChangeScoreTypeCommand = new NavigateModalCommand(() => new ChangeScoreTypeViewModel());
            AddScoreTypeCommand = new NavigateModalCommand(() => new AddScoreTypeToSubjectViewModel());
            FilterGradeCommand = new RelayCommand(FilterGrade);

            LoadSchoolYearFilter();
            LoadGradeFilter();
        }

        private async void OnSelectionSubjectFilterChange()
        {
            int countGradeFilter = GradeFilter.Count(i => i.IsChecked);

            if (string.IsNullOrEmpty(SelectedSchoolYear) || countGradeFilter == 0)
                return;

            List<Grade> grades = new List<Grade>();
            foreach (FilterItem item in GradeFilter)
            {
                if (item.IsChecked && item.Name != "All")
                    grades.Add(GradeDic[item.Name]);
            }

            LoadSubjectData(SchoolYearsDic[SelectedSchoolYear], grades);
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

        public async void LoadGradeFilter()
        {
            GradeFilter.Clear();

            var grades = (await GenericDataService<Grade>.Instance.GetAllAsync()).ToList();
            if (!grades.Any())
                return;

            GradeFilter.Add(new FilterItem("All", false));
            foreach (Grade grade in grades)
            {
                string gradeName = $"{grade.Name}";
                GradeFilter.Add(new FilterItem(gradeName, false));
                GradeDic.Add(gradeName, grade);
            }
        }

        private void FilterGrade(object param)
        {
            FilterItem filterItem = (FilterItem)param;
            ProcessFilterItemSelection(filterItem, GradeFilter);
            OnSelectionSubjectFilterChange();
        }

        private void ProcessFilterItemSelection(FilterItem filterItem, ObservableCollection<FilterItem> filterItems)
        {
            if (filterItem.Name == "All")
            {
                if (filterItem.IsChecked)
                {
                    foreach (FilterItem item in filterItems)
                    {
                        if (!item.IsChecked)
                            item.IsChecked = true;
                    }
                }
                else
                {
                    int countIsChecked = filterItems.Count(i => i.IsChecked);
                    if (countIsChecked == filterItems.Count - 1)
                    {
                        foreach (FilterItem item in filterItems)
                            item.IsChecked = false;
                    }
                }
            }
            else
            {
                if (filterItem.IsChecked == false && filterItems[0].IsChecked)
                {
                    filterItems[0].IsChecked = false;
                }
            }
        }

        private async void LoadSubjectData(SchoolYear schoolYear, List<Grade> grades)
        {
            var gradeId = grades.Select(g => g.Id).Distinct();
            var subjects = (await GenericDataService<Subject>.Instance.GetManyAsync(s => s.YearId == schoolYear.Id && gradeId.Contains(s.GradeId),
                                                                                    query => query.Include(s => s.Grade)));

            SubjectTable.Clear();
            foreach (var subject in subjects)
            {
                SubjectTableRow newRow = new SubjectTableRow
                {
                    SubjectId = subject.Id,
                    GradeId = subject.GradeId,
                    GradeName = subject.Grade.Name,
                    PassScore = subject.ScoreToPass,
                    SubjectName = subject.Name
                };

                SubjectTable.Add(newRow);
                OriginalSubjectTable.Add(newRow);
            }
        }

        private async void LoadScoreTypeData(string subjectId)
        {
            var scoreTypes = (await GenericDataService<SubjectScoreType>.Instance.GetManyAsync(sst => sst.SubjectId == subjectId,
                                                                                               query => query.Include(sst => sst.ScoreType))).ToList();

            ScoreTypeTable.Clear();
            foreach (var scoreType in scoreTypes)
            {
                ScoreTypeTableRow newRow = new ScoreTypeTableRow
                {
                    SubjectId = subjectId,
                    ScoreTypeId = scoreType.Id,
                    ScoreTypeName = scoreType.ScoreType.Name,
                    Coefficient = scoreType.ScoreType.Coefficient,
                    Amount = scoreType.Amount
                };

                ScoreTypeTable.Add(newRow);
            }
        }

        private async void DeleteSubjectRow()
        {
            if (SelectedSubject == null)
            {
                ToastMessageViewModel.ShowErrorToast("Hãy chọn một môn học để xóa");
                return;
            }

            SubjectTable.Remove(SelectedSubject);
            OriginalSubjectTable.Remove(SelectedSubject);
            await GenericDataService<Subject>.Instance.DeleteOneAsync(s => s.Id == SelectedSubject.SubjectId);
            SelectedSubject = null;
        }

        public void OnSearchTextChange()
        {
            SearchAllColumns(OriginalSubjectTable, SubjectTable, SearchText, false, false);
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

        public class SubjectTableRow
        {
            public string SubjectId { get; set; }
            public string GradeId { get; set; }
            public string GradeName { get; set; }
            public string SubjectName { get; set; }
            public double PassScore { get; set; }
        }

        public class ScoreTypeTableRow
        {
            public string SubjectId { get; set; }
            public string ScoreTypeId { get; set; }
            public string ScoreTypeName { get; set; }
            public double Coefficient { get; set; }
            public int Amount { get; set; }
        }
    }
}