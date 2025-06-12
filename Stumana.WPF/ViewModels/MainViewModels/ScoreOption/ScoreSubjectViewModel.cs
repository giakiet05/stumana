using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Helpers;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.MainViewModels.ScoreOption;

public class ScoreSubjectViewModel : BaseViewModel
{
    #region Properties

    public Dictionary<DataRow, Dictionary<string, Tuple<double, double?>>> ChangedData { get; set; } = new();
    public Dictionary<string, SubjectScoreType> SubjectScoreTypeDic { get; set; } = new();

    private DataTable _scoreDataTable = new DataTable();

    public DataTable ScoreDataTable
    {
        get => _scoreDataTable;
        set
        {
            _scoreDataTable = value;
            OnPropertyChanged();
        }
    }

    private DataView _tableView;

    public DataView TableView
    {
        get => _tableView;
        set
        {
            _tableView = value;
            OnPropertyChanged(nameof(TableView));
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

    private Dictionary<string, Subject> SubjectDic { get; set; } = new();
    private Dictionary<string, SchoolYear> SchoolYearDic { get; set; } = new();
    private Dictionary<string, Grade> GradeDic { get; set; } = new();
    private Dictionary<string, Classroom> ClassroomDic { get; set; } = new();
    private Dictionary<string, int> SemesterDic { get; set; } = new();
    private bool IsProcessingFilter { get; set; } = false;

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

    private ObservableCollection<string> _schoolyearFilter = new();

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

    public ObservableCollection<string> SchoolYearFilter
    {
        get => _schoolyearFilter;
        set
        {
            _schoolyearFilter = value;
            OnPropertyChanged();
        }
    }

    private string _selectedSchoolYear;

    public string SelectedSchoolYear
    {
        get => _selectedSchoolYear;
        set
        {
            if (_selectedSchoolYear != value)
            {
                _selectedSchoolYear = value;
                OnPropertyChanged();
                LoadDependentFilter();
                OnFilterChange();
            }
        }
    }

    private ObservableCollection<string> _subjectFilter = new();

    public ObservableCollection<string> SubjectFilter
    {
        get => _subjectFilter;
        set
        {
            _subjectFilter = value;
            OnPropertyChanged();
        }
    }

    private string _selectedSubject;

    public string SelectedSubject
    {
        get => _selectedSubject;
        set
        {
            if (_selectedSubject != value)
            {
                _selectedSubject = value;
                OnPropertyChanged();
                OnFilterChange();
            }
        }
    }

    private ObservableCollection<FilterItem> _classFilter = new();

    public ObservableCollection<FilterItem> ClassFilter
    {
        get => _classFilter;
        set
        {
            _classFilter = value;
            OnPropertyChanged();
        }
    }

    private string _displayClassFilterText = String.Empty;

    public string DisplayClassFilterText
    {
        get => _displayClassFilterText;
        set
        {
            _displayClassFilterText = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<FilterItem> _semesterFilter = new();

    public ObservableCollection<FilterItem> SemesterFilter
    {
        get => _semesterFilter;
        set
        {
            _semesterFilter = value;
            OnPropertyChanged();
        }
    }

    private string _displaySemesterFilterText;

    public string DisplaySemesterFilterText
    {
        get => _displaySemesterFilterText;
        set
        {
            _displaySemesterFilterText = value;
            OnPropertyChanged();
        }
    }

    #endregion Properties

    #region Commands

    public ICommand SaveChangeCommand { get; set; }
    public ICommand DiscardChangeCommand { get; set; }
    public ICommand FilterGradeCommand { get; set; }
    public ICommand FilterClassroomCommand { get; set; }
    public ICommand FilterSemesterCommand { get; set; }

    #endregion Commands

    public ScoreSubjectViewModel()
    {
        SaveChangeCommand = new RelayCommand(SaveChange);
        DiscardChangeCommand = new RelayCommand(DiscardChange);

        //Filter Command
        FilterGradeCommand = new RelayCommand(FilterGrade);
        FilterClassroomCommand = new RelayCommand(FilterClassRoom);
        FilterSemesterCommand = new RelayCommand(FilterSemester);

        LoadInitFilter();
    }

    private async void LoadInitFilter()
    {
        await LoadSchoolYearFilter();
        await LoadGradeFilter();
        DisplayGradeFilterText = ProcessDisplayText(GradeFilter);

        LoadDependentFilter();
        DisplayClassFilterText = ProcessDisplayText(ClassFilter);
        DisplaySemesterFilterText = ProcessDisplayText(SemesterFilter);
    }

    private async Task LoadSchoolYearFilter()
    {
        SchoolYearFilter.Clear();

        List<SchoolYear> schoolYears = (await GenericDataService<SchoolYear>.Instance.GetAllAsync()).ToList();

        if (!schoolYears.Any())
            return;

        foreach (SchoolYear schoolYear in schoolYears)
        {
            string schoolyearName = schoolYear.StartYear + " - " + schoolYear.EndYear;
            SchoolYearFilter.Add(schoolyearName);
            SchoolYearDic.Add(schoolyearName, schoolYear);
        }

        if (SchoolYearFilter.Any())
            SelectedSchoolYear = SchoolYearFilter[0];
    }

    public async Task LoadGradeFilter()
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

        DisplayGradeFilterText = ProcessDisplayText(GradeFilter);
    }

    public async Task LoadSubjectFilter(SchoolYear? schoolYear, List<Grade> grades)
    {
        SubjectFilter.Clear();
        if (schoolYear == null)
            return;

        var gradeId = grades.Select(g => g.Id);
        List<Subject> subjects = (await GenericDataService<Subject>.Instance.GetManyAsync(s => s.YearId == schoolYear.Id && gradeId.Contains(s.GradeId),
                                                                                          query => query.Include(s => s.Grade))).ToList();

        if (!subjects.Any())
            return;

        SubjectDic.Clear();
        foreach (Subject subject in subjects)
        {
            string subjectName = $"{subject.Name} {subject.Grade.Name}";
            SubjectDic.Add(subjectName, subject);
            SubjectFilter.Add(subjectName);
        }

        if (SubjectFilter.Any())
            SelectedSubject = SubjectFilter[0];
    }

    public async Task LoadClassroomFilter(SchoolYear? schoolYear, List<Grade> grades)
    {
        ClassFilter.Clear();
        if (schoolYear == null)
            return;

        var gradeId = grades.Select(g => g.Id);
        List<Classroom> classrooms = (await GenericDataService<Classroom>.Instance.GetManyAsync(c => c.YearId == schoolYear.Id && gradeId.Contains(c.GradeId))).ToList();

        if (!classrooms.Any())
            return;

        ClassroomDic.Clear();
        ClassFilter.Add(new FilterItem("All", true));
        foreach (Classroom classroom in classrooms)
        {
            string className = $"{classroom.Name}";
            ClassFilter.Add(new FilterItem(className, true));
            ClassroomDic[className] = classroom;
        }
    }

    public async Task LoadSemesterFilter(SchoolYear? schoolYear, List<Grade> grades)
    {
        SemesterFilter.Clear();
        if (schoolYear == null)
            return;

        var gradeId = grades.Select(g => g.Id);
        List<int> semesters = (await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.Classroom.YearId == schoolYear.Id && gradeId.Contains(sa.Classroom.GradeId),
                                                                                                 query => query.Include(sa => sa.Classroom))).Select(sa => sa.Semester).Distinct().ToList();

        if (!semesters.Any())
            return;

        SemesterDic.Clear();
        SemesterFilter.Add(new FilterItem("All", true));
        foreach (int semester in semesters)
        {
            string semesterName = semester.ToString();
            SemesterFilter.Add(new FilterItem(semesterName, true));
            SemesterDic[semesterName] = semester;
        }
    }

    public async void LoadDependentFilter()
    {
        if (SelectedSchoolYear == null)
            return;

        SchoolYear schoolYear = SchoolYearDic[SelectedSchoolYear];

        List<Grade> grades = new List<Grade>();
        foreach (var filterItem in GradeFilter)
        {
            if (filterItem.IsChecked && filterItem.Name != "All")
                grades.Add(GradeDic[filterItem.Name]);
        }

        await LoadClassroomFilter(schoolYear, grades);
        await LoadSubjectFilter(schoolYear, grades);
        await LoadSemesterFilter(schoolYear, grades);
        DisplayClassFilterText = ProcessDisplayText(ClassFilter);
        DisplaySemesterFilterText = ProcessDisplayText(SemesterFilter);

        OnFilterChange();
    }

    private async void OnFilterChange()
    {
        if (string.IsNullOrEmpty(SelectedSchoolYear) || GradeFilter.Count(i => i.IsChecked) == 0 || string.IsNullOrEmpty(SelectedSubject))
        {
            TableView = null;
            return;
        }

        Subject subject = SubjectDic[SelectedSubject];
        await LoadTableColumn(subject);

        List<Classroom> classrooms = new List<Classroom>();
        foreach (var filterItem in ClassFilter)
        {
            if (filterItem.IsChecked && filterItem.Name != "All")
                classrooms.Add(ClassroomDic[filterItem.Name]);
        }

        List<int> semesters = new List<int>();
        foreach (var filterItem in SemesterFilter)
        {
            if (filterItem.IsChecked && filterItem.Name != "All")
                semesters.Add(SemesterDic[filterItem.Name]);
        }

        await LoadData(subject, classrooms, semesters);
        ScoreDataTable.ColumnChanged += OnColumnChanged;
    }

    private async Task LoadTableColumn(Subject? curSubject)
    {
        ScoreDataTable = new DataTable();
        SubjectScoreTypeDic.Clear();
        ScoreDataTable.Columns.Add("Học kì", typeof(string));
        ScoreDataTable.Columns.Add("Môn học", typeof(string));
        ScoreDataTable.Columns.Add("Lớp", typeof(string));
        ScoreDataTable.Columns.Add("Mã học sinh", typeof(string));
        ScoreDataTable.Columns.Add("Tên học sinh", typeof(string));
        ScoreDataTable.Columns["Học kì"].ReadOnly = true;
        ScoreDataTable.Columns["Môn học"].ReadOnly = true;
        ScoreDataTable.Columns["Lớp"].ReadOnly = true;
        ScoreDataTable.Columns["Mã học sinh"].ReadOnly = true;
        ScoreDataTable.Columns["Tên học sinh"].ReadOnly = true;

        if (curSubject == null)
            return;

        var subjectScoreTypes = await GenericDataService<SubjectScoreType>.Instance.GetManyAsync(sc => sc.SubjectId == curSubject.Id,
                                                                                                 score => score.Include(sc => sc.ScoreType));

        foreach (SubjectScoreType subjectScoreType in subjectScoreTypes)
        {
            string header = subjectScoreType.ScoreType.Name;

            if (subjectScoreType.Amount == 1)
            {
                SubjectScoreTypeDic[header] = subjectScoreType;
                ScoreDataTable.Columns.Add(header, typeof(double));
                continue;
            }

            for (int i = 1; i <= subjectScoreType.Amount; i++)
            {
                string columnName = $"{header} lần {i}";
                SubjectScoreTypeDic[columnName] = subjectScoreType;
                ScoreDataTable.Columns.Add(columnName, typeof(double));
            }
        }

        ScoreDataTable.Columns.Add("Điểm TB", typeof(double));
        ScoreDataTable.Columns["Điểm TB"].ReadOnly = true;

        TableView = ScoreDataTable.DefaultView;
    }

    private async Task LoadData(Subject curSubject, List<Classroom>? classrooms = null, List<int>? semesters = null)
    {
        if (ScoreDataTable.Columns.Count == 0)
            return;

        var subjectScoreTypes = (await GenericDataService<SubjectScoreType>.Instance.GetManyAsync(sc => sc.SubjectId == curSubject.Id,
                                                                                                  score => score.Include(sc => sc.ScoreType))).ToList();

        if (subjectScoreTypes.Count == 0)
            return;

        List<StudentAssignment> studentAssignments = new List<StudentAssignment>();
        if (classrooms != null && classrooms.Count > 0)
        {
            var classroomId = classrooms.Select(c => c.Id);
            studentAssignments = (await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => classroomId.Contains(sa.ClassroomId),
                                                                                                    query => query.Include(sa => sa.Student)
                                                                                                        .Include(sa => sa.Classroom))).ToList();
        }

        if (semesters != null)
            studentAssignments = studentAssignments.Where(sa => semesters.Contains(sa.Semester)).ToList();

        ScoreDataTable.Clear();
        foreach (var studentAssignment in studentAssignments)
        {
            if (studentAssignment.Classroom.GradeId != curSubject.GradeId)
                continue;

            DataRow dataRow = ScoreDataTable.NewRow();
            dataRow["Học kì"] = studentAssignment.Semester;
            dataRow["Môn học"] = $"{curSubject.Name} {curSubject.Grade.Name}";
            dataRow["Lớp"] = studentAssignment.Classroom.Name;
            dataRow["Mã học sinh"] = studentAssignment.StudentId;
            dataRow["Tên học sinh"] = studentAssignment.Student.Name;

            double sumScore = 0;
            double sumCoefficient = 0;
            double maxSumCoefficient = 0;

            foreach (var subjectScoreType in subjectScoreTypes)
            {
                maxSumCoefficient += subjectScoreType.Amount * subjectScoreType.ScoreType.Coefficient;

                var studentScores = (await GenericDataService<Score>.Instance.GetManyAsync(sc => sc.StudentAssignmentId == studentAssignment.Id
                                                                                                 && sc.SubjectScoreTypeId == subjectScoreType.Id)).ToList();
                studentScores.Sort((a, b) => a.Attempt.CompareTo(b.Attempt));

                if (subjectScoreType.Amount <= 1)
                {
                    string columnName = subjectScoreType.ScoreType.Name;
                    if (studentScores.Count > 0)
                    {
                        dataRow[columnName] = studentScores[0].Value;
                        sumScore += studentScores[0].Value * subjectScoreType.ScoreType.Coefficient;
                        sumCoefficient += subjectScoreType.ScoreType.Coefficient;
                    }
                    else
                        dataRow[columnName] = DBNull.Value;

                    continue;
                }

                foreach (var score in studentScores)
                {
                    string columnName = $"{subjectScoreType.ScoreType.Name} lần {score.Attempt}";
                    dataRow[columnName] = score.Value;
                    sumScore += score.Value * subjectScoreType.ScoreType.Coefficient;
                    sumCoefficient += subjectScoreType.ScoreType.Coefficient;
                }
            }

            if (sumCoefficient != 0 && sumCoefficient >= maxSumCoefficient)
                dataRow["Điểm TB"] = double.Round(sumScore / sumCoefficient, 2);
            else
                dataRow["Điểm TB"] = DBNull.Value;

            ScoreDataTable.Rows.Add(dataRow);
        }
    }

    private void FilterGrade(object param)
    {
        if (IsProcessingFilter)
            return;

        try
        {
            IsProcessingFilter = true;
            FilterItem filterItem = (FilterItem)param;
            ProcessFilterItemSelection(filterItem, GradeFilter);
            DisplayGradeFilterText = ProcessDisplayText(GradeFilter);
            LoadDependentFilter();
        }
        finally
        {
            IsProcessingFilter = false;
        }
    }

    private void FilterClassRoom(object param)
    {
        if (IsProcessingFilter)
            return;

        try
        {
            IsProcessingFilter = true;
            FilterItem filterItem = (FilterItem)param;
            ProcessFilterItemSelection(filterItem, ClassFilter);
            DisplayClassFilterText = ProcessDisplayText(ClassFilter);
            OnFilterChange();
        }
        finally
        {
            IsProcessingFilter = false;
        }
    }

    private void FilterSemester(object param)
    {
        if (IsProcessingFilter)
            return;

        try
        {
            IsProcessingFilter = true;
            FilterItem filterItem = (FilterItem)param;
            ProcessFilterItemSelection(filterItem, SemesterFilter);
            DisplaySemesterFilterText = ProcessDisplayText(SemesterFilter);
            OnFilterChange();
        }
        finally
        {
            IsProcessingFilter = false;
        }
    }

    private string ProcessDisplayText(ObservableCollection<FilterItem> filterItems, string defaultText = "")
    {
        int selectionCount = filterItems.Count(i => i.IsChecked);
        if (selectionCount == 0)
            return defaultText;

        string displayText;
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

    private bool _isUpdating = false;

    public async void OnColumnChanged(object sender, DataColumnChangeEventArgs e)
    {
        if (_isUpdating)
            return;
        _isUpdating = true;

        string columnName = e.Column.ColumnName;

        double originalValue = 0;
        if (ChangedData.TryGetValue(e.Row, out var dictionary) && dictionary.TryGetValue(columnName, out var myTuple))
            originalValue = myTuple.Item1;
        else
        {
            var subjectScoreTypeId = SubjectScoreTypeDic[columnName].Id;
            var studentId = e.Row["Mã học sinh"].ToString();
            var semester = int.Parse(e.Row["Học kì"].ToString());
            var classroomId = ClassroomDic[e.Row["Lớp"].ToString()].Id;

            var attemptStr = columnName.Split(' ').Last();
            int attempt = int.TryParse(attemptStr, out var parsed) ? parsed : 1;

            var scores = (await GenericDataService<Score>.Instance.GetManyAsync(s => s.SubjectScoreTypeId == subjectScoreTypeId &&
                                                                                     s.StudentAssignment.StudentId == studentId &&
                                                                                     s.StudentAssignment.Semester == semester &&
                                                                                     s.StudentAssignment.ClassroomId == classroomId,
                                                                                query => query.Include(s => s.StudentAssignment))).ToList();
            var score = scores.FirstOrDefault(s => s.Attempt == attempt);
            if (score == null)
                originalValue = -1;
            else
                originalValue = score.Value;
        }

        object? newObj = e.ProposedValue;
        double? newValue = null;

        if (newObj != null && newObj != DBNull.Value)
        {
            try
            {
                double parsedValue = Convert.ToDouble(newObj);

                if (parsedValue < 0 || parsedValue > 10)
                    throw new Exception();

                newValue = parsedValue;
            }
            catch
            {
                ToastMessageViewModel.ShowErrorToast("Điểm không hợp lệ");
                e.Row[columnName] = originalValue < 0 ? DBNull.Value : originalValue;
                _isUpdating = false;
                return;
            }
        }

        if ((originalValue < 0 && newValue == null) || Equals(originalValue, newValue))
        {
            if (ChangedData.TryGetValue(e.Row, out var columnChanges))
                columnChanges.Remove(columnName);
            _isUpdating = false;
            return;
        }

        if (!ChangedData.ContainsKey(e.Row))
            ChangedData[e.Row] = new Dictionary<string, Tuple<double, double?>>();

        ChangedData[e.Row][columnName] = Tuple.Create(originalValue, newValue);
        _isUpdating = false;
    }

    public async void SaveChange()
    {
        try
        {
            foreach (var rowEntry in ChangedData)
            {
                DataRow row = rowEntry.Key;
                Dictionary<string, Tuple<double, double?>> columnChanges = rowEntry.Value;

                string studentId = row["Mã học sinh"].ToString();
                int semester = SemesterDic[row["Học kì"].ToString()];
                string classroomId = ClassroomDic[row["Lớp"].ToString()].Id;

                foreach (var columnEntry in columnChanges)
                {
                    string columnName = columnEntry.Key;
                    double oldValue = columnEntry.Value.Item1;
                    double? newValue = columnEntry.Value.Item2;

                    var attemptStr = columnName.Split(' ').Last();
                    int attempt = int.TryParse(attemptStr, out var parsed) ? parsed : 1;

                    string scoreTypeId = SubjectScoreTypeDic[columnName].ScoreTypeId;
                    var subjectId = SubjectDic[SelectedSubject].Id;

                    var studentAssignment = await GenericDataService<StudentAssignment>.Instance.GetOneAsync(sa => sa.StudentId == studentId &&
                                                                                                                   sa.Semester == semester &&
                                                                                                                   sa.ClassroomId == classroomId);

                    var subjectScoreType = await GenericDataService<SubjectScoreType>.Instance.GetOneAsync(sst => sst.ScoreType.Id == scoreTypeId && sst.SubjectId == subjectId);
                    if (newValue == null)
                    {
                        Score oldScore = await GenericDataService<Score>.Instance.GetOneAsync(s => s.StudentAssignmentId == studentAssignment.Id &&
                                                                                                   s.SubjectScoreTypeId == subjectScoreType.Id &&
                                                                                                   s.Value == oldValue);

                        await GenericDataService<Score>.Instance.DeleteOneAsync(s => s.Id == oldScore.Id);
                        continue;
                    }

                    if (oldValue < 0) //old score not exist
                    {
                        Score score = new Score
                        {
                            Id = Guid.NewGuid().ToString(),
                            Value = (double)newValue,
                            Attempt = attempt,
                            SubjectScoreTypeId = subjectScoreType.Id,
                            StudentAssignmentId = studentAssignment.Id
                        };

                        await GenericDataService<Score>.Instance.CreateOneAsync(score);
                    }
                    else
                    {
                        Score newScore = await GenericDataService<Score>.Instance.GetOneAsync(s => s.StudentAssignmentId == studentAssignment.Id &&
                                                                                                   s.SubjectScoreTypeId == subjectScoreType.Id &&
                                                                                                   s.Attempt == attempt);
                        newScore.Value = (double)newValue;
                        await GenericDataService<Score>.Instance.UpdateOneAsync(newScore, s => s.Id == newScore.Id);
                    }
                }
            }

            ChangedData.Clear();
            ToastMessageViewModel.ShowSuccessToast("Lưu thay đổi thành công");
            OnFilterChange();
        }
        catch (Exception e)
        {
            ToastMessageViewModel.ShowErrorToast("Không thể lưu. Đã có lỗi xảy ra");
        }
    }

    public void DiscardChange()
    {
        ModalNavigationStore.Instance.CurrentModalViewModel = new DeleteConfirmViewModel(
            () =>
            {
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
                ModalNavigationStore.Instance.Close();
            },
            "Bạn có chắc chắn muốn huỷ thay đổi?\nĐiều này không thể hoàn tác"
                );
       
       

        
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
}