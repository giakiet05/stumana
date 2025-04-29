using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;

namespace Stumana.WPF.ViewModels.MainViewModels.ScoreOption;

public class ScoreSubjectViewModel : BaseViewModel
{
    #region Properties

    public Dictionary<DataRow, Dictionary<string, Tuple<double, double?>>> ChangedData = new();
    public Dictionary<string, SubjectScoreType> ScoreTypeDetailDic { get; set; } = new();

    private DataTable ScoreDataTable { get; set; } = new DataTable();
    private DataView _tableView;

    public DataView TableView
    {
        get => _tableView;
        set
        {
            _tableView = value;
            OnPropertyChanged();
        }
    }

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

    public Dictionary<string, SchoolYear> SchoolYearDic { get; set; } = new();
    public Dictionary<string, Subject> SubjectDic { get; set; } = new();
    public Dictionary<string, Grade> GradeDic { get; set; } = new();
    public Dictionary<string, Classroom> ClassroomDic { get; set; }


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

    private ObservableCollection<FilterItem> _schoolyearFilter = new();

    public ObservableCollection<FilterItem> SchoolYearFilter
    {
        get => _schoolyearFilter;
        set
        {
            _schoolyearFilter = value;
            OnPropertyChanged();
        }
    }


    private ObservableCollection<FilterItem> _subjectFilter = new();

    public ObservableCollection<FilterItem> SubjectFilter
    {
        get => _subjectFilter;
        set
        {
            _subjectFilter = value;
            OnPropertyChanged();
        }
    }

    #endregion Properties

    #region Commands

    public ICommand SaveChangeCommand { get; set; }
    public ICommand DiscardChangeCommand { get; set; }
    public ICommand FilterSchoolYearCommand { get; set; }
    public ICommand FilterGradeCommand { get; set; }
    public ICommand FilterSubjectCommand { get; set; }

    #endregion Commands

    public ScoreSubjectViewModel()
    {
        SaveChangeCommand = new RelayCommand(SaveChange);
        DiscardChangeCommand = new RelayCommand(DiscardChange);

        //Filter Command
        FilterGradeCommand = new RelayCommand(FilterGrade);

        LoadTable();
        LoadGradeFilter();
    }

    private async void LoadTable()
    {
        //Create table
        await LoadTableColumn();

        //Testing
        ScoreDataTable.Rows.Add("HS0033333333333331", "Nguyễn Văn An");
        ScoreDataTable.Rows.Add("HS0011111111111", "Nguyễn Văn Aaaaaaaaaaaaaa");
        ScoreDataTable.Rows.Add("HS0012222222222222222", "Nguyễn Văn Affffffffffff");
        for (int i = 1; i <= 20; i++)
        {
            ScoreDataTable.Rows.Add("HS0012222222222222222", "Nguyễn Văn Affffffffffff");
        }

        //Event
        ScoreDataTable.ColumnChanged += OnColumnChanged;
    }

    private async Task LoadTableColumn()
    {
        ScoreDataTable = new DataTable();
        ScoreDataTable.Columns.Add("Mã học sinh", typeof(string));
        ScoreDataTable.Columns.Add("Tên học sinh", typeof(string));
        ScoreDataTable.Columns["Mã học sinh"].ReadOnly = true;
        ScoreDataTable.Columns["Tên học sinh"].ReadOnly = true;
        TableView = ScoreDataTable.DefaultView;

        if (curSubject == null)
            return;

        var scoreSheet = await GenericDataService<SubjectScoreType>.Instance.GetManyAsync(sc => sc.SubjectId == curSubject.Id,
                                                                                          score => score.Include(sc => sc.ScoreType));
        if (!scoreSheet.Any())
            return;

        foreach (SubjectScoreType scoreTypeDetail in scoreSheet)
        {
            string header = scoreTypeDetail.ScoreType.Name;

            for (int i = 1; i <= scoreTypeDetail.Amount; i++)
            {
                string columnName = $"{header} {i}";
                ScoreTypeDetailDic[columnName] = scoreTypeDetail;
                ScoreDataTable.Columns.Add(columnName, typeof(double));
            }
        }

        ScoreDataTable.Columns.Add("Điểm TB", typeof(double));
        ScoreDataTable.Columns["Điểm TB"].ReadOnly = true;

        TableView = ScoreDataTable.DefaultView;
    }

    private async Task LoadData(Subject curSubject)
    {
        if (ScoreDataTable.Columns.Count == 0)
            return;

        var scoreSheet = await GenericDataService<SubjectScoreType>.Instance.GetManyAsync(sc => sc.SubjectId == curSubject.Id,
                                                                                          score => score.Include(sc => sc.ScoreType));
        var scoreTypeIds = scoreSheet.Select(ss => ss.Id).ToList();
        var scoreDetails = await GenericDataService<Score>.Instance.GetManyAsync(score => scoreTypeIds.Contains(score.SubjectScoreTypeId),
                                                                                 s => s.Include(s => s.StudentAssignment).ThenInclude(sa => sa.Student));

        var studentIDList = scoreDetails.Select(score => score.StudentAssignment.StudentId).Distinct().ToList();

        foreach (string studentId in studentIDList)
        {
            DataRow dataRow = ScoreDataTable.NewRow();

            double sumScore = 0;
            double sumCoefficient = 0;

            var studentScore = scoreDetails.Where(score => score.StudentAssignment.StudentId == studentId).ToList();

            dataRow["Mã học sinh"] = studentId;
            dataRow["Tên học sinh"] = studentScore[0].StudentAssignment.Student.Name;

            foreach (SubjectScoreType subjectScoreType in scoreSheet)
            {
                List<Score> scores = studentScore.Where(score => score.SubjectScoreTypeId == subjectScoreType.Id).ToList();

                for (int i = 1; i <= subjectScoreType.Amount; i++)
                {
                    string columnName = $"{subjectScoreType.ScoreType.Name} {i}";
                    if (i <= scores.Count)
                    {
                        dataRow[columnName] = scores[i - 1].Value;
                        sumScore += scores[i - 1].Value * subjectScoreType.ScoreType.Coefficient;
                        sumCoefficient += subjectScoreType.ScoreType.Coefficient;
                    }
                    else
                        dataRow[columnName] = DBNull.Value;
                }
            }

            dataRow["Điểm TB"] = sumScore / sumCoefficient;

            ScoreDataTable.Rows.Add(dataRow);
        }
    }

    public async void OnColumnChanged(object sender, DataColumnChangeEventArgs e)
    {
        string columnName = e.Column.ColumnName;

        //object originalObj = e.Row[columnName, DataRowVersion.Original];
        //double originalValue = originalObj == DBNull.Value ? -1 : (double)originalObj;

        double originalValue = 0;
        if (ChangedData.TryGetValue(e.Row, out var dictionary))
            originalValue = dictionary[columnName].Item1;
        else
            originalValue = (await GenericDataService<Score>.Instance.GetOneAsync(s => s.SubjectScoreTypeId == ScoreTypeDetailDic[columnName].Id &&
                                                                                       s.StudentAssignment.StudentId == e.Row["Mã học sinh"].ToString(),
                                                                                  query => query.Include(s => s.StudentAssignment))).Value;

        object? newObj = e.ProposedValue;
        double? newValue = null;

        //if (!string.IsNullOrEmpty(Convert.ToString(newObj)))
        if (newObj != null || newObj != DBNull.Value)
        {
            try
            {
                double parsedValue = Convert.ToDouble(newObj);

                if (parsedValue < 0 || parsedValue > 10)
                {
                    ToastMessageViewModel.ShowErrorToast("Điểm không hợp lệ");
                    e.Row[columnName] = originalValue;
                    return;
                }

                newValue = parsedValue;
            }
            catch
            {
                ToastMessageViewModel.ShowErrorToast("Điểm không hợp lệ");
                e.Row[columnName] = originalValue;
                return;
            }
        }

        if ((originalValue < 0 && newValue == null) || Equals(originalValue, newValue))
        {
            if (ChangedData.TryGetValue(e.Row, out var columnChanges))
                columnChanges.Remove(columnName);
            return;
        }

        if (!ChangedData.ContainsKey(e.Row))
            ChangedData[e.Row] = new Dictionary<string, Tuple<double, double?>>();

        ChangedData[e.Row][columnName] = Tuple.Create(originalValue, newValue);
    }

    public async void SaveChange()
    {
        await SaveChangeAsync();
        ToastMessageViewModel.ShowSuccessToast("Lưu thay đổi thành công");
    }

    public async Task SaveChangeAsync()
    {
        try
        {
            foreach (var rowEntry in ChangedData)
            {
                DataRow row = rowEntry.Key;
                Dictionary<string, Tuple<double, double?>> columnChanges = rowEntry.Value;

                string studentId = row["Mã học sinh"].ToString();

                foreach (var columnEntry in columnChanges)
                {
                    string columnName = columnEntry.Key;
                    double oldValue = columnEntry.Value.Item1;
                    double? newValue = columnEntry.Value.Item2;

                    string scoreTypeName = columnName.Substring(0, columnName.LastIndexOf(' '));

                    var studentAssignment = await GenericDataService<StudentAssignment>.Instance.GetOneAsync(sa => sa.StudentId == studentId);
                    var scoreType = await GenericDataService<SubjectScoreType>.Instance.GetOneAsync(sst => sst.ScoreType.Name == scoreTypeName,
                                                                                                    query => query.Include(sst => sst.ScoreType));
                    if (newValue == null)
                    {
                        Score oldScore = await GenericDataService<Score>.Instance.GetOneAsync(s => s.StudentAssignmentId == studentAssignment.Id &&
                                                                                                   s.SubjectScoreTypeId == scoreType.Id);

                        await GenericDataService<Score>.Instance.DeleteOneAsync(s => s.Id == oldScore.Id);
                        return;
                    }

                    if (oldValue < 0)
                    {
                        Score score = new Score
                        {
                            Id = Guid.NewGuid().ToString(),
                            Value = (double)newValue,
                            SubjectScoreTypeId = scoreType.Id,
                            StudentAssignmentId = studentAssignment.Id
                        };

                        await GenericDataService<Score>.Instance.CreateOneAsync(score);
                    }
                    else
                    {
                        Score newScore = await GenericDataService<Score>.Instance.GetOneAsync(s => s.StudentAssignmentId == studentAssignment.Id &&
                                                                                                   s.SubjectScoreTypeId == scoreType.Id);
                        newScore.Value = (double)newValue;
                        await GenericDataService<Score>.Instance.UpdateOneAsync(newScore, s => s.Id == newScore.Id);
                    }
                }
            }
        }
        catch (Exception e)
        {
            ToastMessageViewModel.ShowErrorToast("Không thể lưu. Đã có lỗi xảy ra");
        }
    }

    public void DiscardChange()
    {
        var result = MessageBox.Show(
            "Bạn có chắc chắn muốn huỷ thay đổi?",
            "Xác nhận huỷ",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );

        if (result != MessageBoxResult.Yes)
            return;

        foreach (var rowEntry in ChangedData)
        {
            DataRow row = rowEntry.Key;
            Dictionary<string, Tuple<double, double?>> columnChanges = rowEntry.Value;

            foreach (var columnEntry in columnChanges)
            {
                string columnName = columnEntry.Key;
                double oldValue = columnEntry.Value.Item1;

                row[columnName] = oldValue >= 0 ? oldValue : DBNull.Value;
            }
        }

        ChangedData.Clear();
    }

    public void OnSearchTextChange()
    {
        TableView = SearchAllColumns(SearchText, false, false);
    }

    public DataView SearchAllColumns(string searchText, bool exactMatch = false, bool caseSensitive = false)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            return ScoreDataTable.DefaultView;
        }

        var culture = new CultureInfo("vi-VN");
        var compareInfo = culture.CompareInfo;
        var compareOptions = caseSensitive ? CompareOptions.None : CompareOptions.IgnoreCase;

        var filteredRows = ScoreDataTable.AsEnumerable()
            .Where(row => row.ItemArray.Any(cell =>
            {
                if (cell == null)
                    return false;

                string? cellValue = cell.ToString();
                if (string.IsNullOrEmpty(cellValue))
                    return false;

                if (exactMatch)
                {
                    return compareInfo.Compare(cellValue, searchText, compareOptions) == 0;
                }
                else
                {
                    return compareInfo.IndexOf(cellValue, searchText, compareOptions) >= 0;
                }
            }));

        return filteredRows.Any() ? filteredRows.CopyToDataTable().DefaultView : new DataView(ScoreDataTable.Clone());
    }

    private async Task LoadSchoolYear()
    {
        List<SchoolYear> schoolYears = (await GenericDataService<SchoolYear>.Instance.GetAllAsync()).ToList();

        if (!schoolYears.Any())
            return;

        SchoolYearFilter.Add(new FilterItem("All", false));
        foreach (SchoolYear schoolYear in schoolYears)
        {
            string schoolyearName = schoolYear.StartYear + " - " + schoolYear.EndYear;
            SchoolYearFilter.Add(new FilterItem(schoolyearName, false));
            SchoolYearDic.Add(schoolyearName, schoolYear);
        }
    }

    public async void LoadGradeFilter()
    {
        GradeFilter.Clear();

        //Test
        //GradeFilter.Add(new FilterItem("All", false));
        //GradeFilter.Add(new FilterItem("Khối 10", false));
        //GradeFilter.Add(new FilterItem("Khối 11", false));
        //GradeFilter.Add(new FilterItem("Khối 12", false));

        var grades = (await GenericDataService<Grade>.Instance.GetAllAsync()).ToList();
        if (!grades.Any())
            return;

        GradeFilter.Add(new FilterItem("All", false));

        foreach (Grade grade in grades)
        {
            string gradeName = $"Khối {grade.Level}";
            GradeFilter.Add(new FilterItem(gradeName, false));
            GradeDic.Add(gradeName, grade);
        }
    }

    public async void LoadSubjectFilter(IEnumerable<SchoolYear> schoolYears, IEnumerable<Grade> grades)
    {

    }

    private void FilterGrade(object param)
    {
        FilterItem filterItem = (FilterItem)param;
        ProcessFilterItemSelection(filterItem, GradeFilter);
        return;
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
}

public class FilterItem : INotifyPropertyChanged
{
    private string _name;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    private bool _isChecked;

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked != value)
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }
    }

    private FilterItem() { }

    public FilterItem(string name, bool isChecked)
    {
        Name = name;
        IsChecked = isChecked;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}