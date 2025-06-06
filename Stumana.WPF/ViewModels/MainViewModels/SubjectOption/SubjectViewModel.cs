using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        private string _displayGradeFilterText = String.Empty;

        public string DisplayGradeFilterText
        {
            get => _displayGradeFilterText;
            set
            {
                _displayGradeFilterText = value;
                OnPropertyChanged();
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
                    LoadSubjectScoreTypeData(_selectedSubject);
                }
            }
        }

        public ObservableCollection<SubjectScoreTypeTableRow> SubjectScoreTypeTable { get; set; } = new();

        private SubjectScoreTypeTableRow? _selectedSubjectScoreType;

        public SubjectScoreTypeTableRow? SelectedSubjectScoreType
        {
            get => _selectedSubjectScoreType;
            set
            {
                _selectedSubjectScoreType = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddSubjectCommand { get; set; }
        public ICommand DeleteSubjectCommand { get; set; }
        public ICommand EditScoreTypeCommand { get; set; }
        public ICommand AddSubjectScoreTypeCommand { get; set; }
        public ICommand DeleteSubjectScoreTypeCommand { get; set; }
        public ICommand FilterGradeCommand { get; set; }
        public EventHandler? OnSubjectDataChanged { get; set; }
        public EventHandler? OnSubjectScoreTypeDataChanged { get; set; }

        public SubjectViewModel()
        {
            OnSubjectDataChanged += UpdateSubjectData;
            OnSubjectScoreTypeDataChanged += UpdateSubjectScoreTypeData;

            AddSubjectCommand = new NavigateModalCommand(() => new AddSubjectViewModel(OnSubjectDataChanged));
            DeleteSubjectCommand = new NavigateModalCommand(() => new DeleteConfirmViewModel(DeleteSubjectRow),
                                                            () => SelectedSubject != null, "Hãy chọn một môn học để xóa");
            EditScoreTypeCommand = new NavigateModalCommand(() => new EditScoreTypeViewModel());
            AddSubjectScoreTypeCommand = new NavigateModalCommand(() => new AddSubjectScoreTypeViewModel(SelectedSubject.MySubject, OnSubjectScoreTypeDataChanged),
                                                                  () => SelectedSubject != null, "Hãy chọn một môn học");
            DeleteSubjectScoreTypeCommand = new NavigateModalCommand(() => new DeleteConfirmViewModel(DeleteSubjectScoreType),
                                                                     () => SelectedSubjectScoreType != null, "Hãy chọn loại điểm để xóa");
            FilterGradeCommand = new RelayCommand(FilterGrade);

            LoadFilter();
        }

        private void LoadFilter()
        {
            LoadSchoolYearFilter();
            LoadGradeFilter();
            DisplayGradeFilterText = ProcessDisplayText(GradeFilter);
            OnSelectionSubjectFilterChange();
        }

        private async void UpdateSubjectData(object? sender, EventArgs e)
        {
            OnSelectionSubjectFilterChange();
        }

        private void UpdateSubjectScoreTypeData(object? sender, EventArgs e)
        {
            LoadSubjectScoreTypeData(SelectedSubject);
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
            if (SchoolYearCollection.Any())
                SelectedSchoolYear = SchoolYearCollection[0];
        }

        public async void LoadGradeFilter()
        {
            GradeFilter.Clear();

            var grades = (await GenericDataService<Grade>.Instance.GetAllAsync()).ToList();
            if (!grades.Any())
                return;

            GradeFilter.Add(new FilterItem("All", true));
            foreach (Grade grade in grades)
            {
                string gradeName = $"{grade.Name}";
                GradeFilter.Add(new FilterItem(gradeName, true));
                GradeDic.Add(gradeName, grade);
            }
        }

        private void FilterGrade(object param)
        {
            FilterItem filterItem = (FilterItem)param;
            ProcessFilterItemSelection(filterItem, GradeFilter);
            DisplayGradeFilterText = ProcessDisplayText(GradeFilter);
            OnSelectionSubjectFilterChange();
        }

        private string ProcessDisplayText(ObservableCollection<FilterItem> filterItems)
        {
            string displayText;
            int selectionCount = filterItems.Count(i => i.IsChecked);
            if (selectionCount < filterItems.Count - 1)
                displayText = $"{selectionCount} được chọn";
            else
                displayText = "Tất cả";
            return displayText;
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
                    filterItems[0].IsChecked = false;

                if (filterItem.IsChecked && filterItems.Count(i => i.IsChecked) >= filterItems.Count - 1)
                    filterItems[0].IsChecked = true;
            }
        }

        private async void LoadSubjectData(SchoolYear schoolYear, List<Grade> grades)
        {
            var gradeId = grades.Select(g => g.Id).Distinct();
            var subjects = (await GenericDataService<Subject>.Instance.GetManyAsync(s => s.YearId == schoolYear.Id && gradeId.Contains(s.GradeId),
                                                                                    query => query.Include(s => s.Grade)));

            SubjectTable.Clear();
            OriginalSubjectTable.Clear();
            foreach (var subject in subjects)
            {
                SubjectTableRow newRow = new SubjectTableRow
                {
                    MySubject = subject,
                    SubjectId = subject.Id,
                    GradeId = subject.GradeId,
                    GradeName = subject.Grade.Name,
                    PassScore = subject.ScoreToPass,
                    SubjectName = subject.Name
                };
                newRow.PropertyChanged += OnSubjectRowPropertyChanged;

                SubjectTable.Add(newRow);
                OriginalSubjectTable.Add(newRow);
            }
        }

        private async void OnSubjectRowPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == null)
                return;

            var row = (SubjectTableRow)sender;
            Subject subject = new Subject
            {
                Id = row.SubjectId,
                Name = row.SubjectName,
                ScoreToPass = row.PassScore,
                GradeId = row.GradeId,
                YearId = SchoolYearsDic[SelectedSchoolYear].Id
            };

            await GenericDataService<Subject>.Instance.UpdateOneAsync(subject, s => s.Id == subject.Id);
            ToastMessageViewModel.ShowSuccessToast("Chỉnh sửa điểm đạt môn thành công");
        }

        private async void LoadSubjectScoreTypeData(SubjectTableRow? subjectRow)
        {
            SubjectScoreTypeTable.Clear();
            if (subjectRow == null)
                return;

            var subjectScoreTypes = (await GenericDataService<SubjectScoreType>.Instance.GetManyAsync(sst => sst.SubjectId == subjectRow.SubjectId,
                                                                                                      query => query.Include(sst => sst.ScoreType))).ToList();

            foreach (var subjectScoreType in subjectScoreTypes)
            {
                SubjectScoreTypeTableRow newRow = new SubjectScoreTypeTableRow
                {
                    SubjectScoreTypeId = subjectScoreType.Id,
                    SubjectId = subjectRow.SubjectId,
                    ScoreTypeId = subjectScoreType.ScoreTypeId,
                    ScoreTypeName = subjectScoreType.ScoreType.Name,
                    Coefficient = subjectScoreType.ScoreType.Coefficient,
                    Amount = subjectScoreType.Amount
                };
                newRow.PropertyChanged += OnSubjectScoreTypeRowPropertyChanged;

                SubjectScoreTypeTable.Add(newRow);
            }
        }

        private async void OnSubjectScoreTypeRowPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == null)
                return;

            var row = (SubjectScoreTypeTableRow)sender;
            SubjectScoreType subjectScoreType = new SubjectScoreType
            {
                Id = row.SubjectScoreTypeId,
                Amount = row.Amount,
                ScoreTypeId = row.ScoreTypeId,
                SubjectId = row.SubjectId
            };

            await GenericDataService<SubjectScoreType>.Instance.UpdateOneAsync(subjectScoreType, sst => sst.Id == subjectScoreType.Id);
            ToastMessageViewModel.ShowSuccessToast("Chỉnh sửa số lượng thành công");
        }

        private async void DeleteSubjectRow()
        {
            if (SelectedSubject == null)
            {
                ToastMessageViewModel.ShowErrorToast("Hãy chọn một môn học để xóa");
                return;
            }

            await GenericDataService<Subject>.Instance.DeleteOneAsync(s => s.Id == SelectedSubject.SubjectId);
            OriginalSubjectTable.Remove(SelectedSubject);
            SubjectTable.Remove(SelectedSubject);
            SelectedSubject = null;
            
            Stumana.WPF.Stores.ModalNavigationStore.Instance.Close();
        }

        private async void DeleteSubjectScoreType()
        {
            if (SelectedSubjectScoreType == null)
            {
                ToastMessageViewModel.ShowErrorToast("Hãy chọn loại điểm để xóa");
                return;
            }

            await GenericDataService<SubjectScoreType>.Instance.DeleteOneAsync(sst => sst.Id == SelectedSubjectScoreType.SubjectScoreTypeId);
            SubjectScoreTypeTable.Remove(SelectedSubjectScoreType);
            
            Stumana.WPF.Stores.ModalNavigationStore.Instance.Close();
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

        public class SubjectTableRow : INotifyPropertyChanged
        {
            public Subject MySubject { get; set; }
            public string SubjectId { get; set; }
            public string GradeId { get; set; }
            public string GradeName { get; set; }
            public string SubjectName { get; set; }
            private double _passScore;

            public double PassScore
            {
                get => _passScore;
                set
                {
                    _passScore = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class SubjectScoreTypeTableRow : INotifyPropertyChanged
        {
            public string SubjectScoreTypeId { get; set; }
            public string SubjectId { get; set; }
            public string ScoreTypeId { get; set; }
            public string ScoreTypeName { get; set; }
            public double Coefficient { get; set; }
            private int _amount;

            public int Amount
            {
                get => _amount;
                set
                {
                    _amount = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}