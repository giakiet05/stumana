using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;

namespace Stumana.WPF.ViewModels.MainViewModels.ScoreOption
{
    public class ScoreSubjectViewModel : BaseViewModel
    {
        #region Properties

        //private Dictionary<string, int> ScoreTypeDic = new Dictionary<string, int>();
        public Dictionary<DataRow, Dictionary<string, Tuple<double, double?>>> ChangedData = new();
        public Dictionary<string, SubjectScoreType> ScoreTypeDetailDic { get; set; } = new();

        public Subject? curSubject { get; set; } = null;

        private DataTable dataTable = new DataTable();
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
            }
        }

        public Dictionary<string, Subject> SubjectDic { get; set; } = new();
        private ObservableCollection<string> _subjectCollection;

        public ObservableCollection<string> SubjectCollection
        {
            get => _subjectCollection;
            set
            {
                _subjectCollection = value;
                OnPropertyChanged();
            }
        }

        private string _selectedsubject;

        public string SelectedSubject
        {
            get => _selectedsubject;
            set
            {
                _selectedsubject = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<string, Grade> GradeDic { get; set; } = new();
        private ObservableCollection<string> _gradeCollection;

        public ObservableCollection<string> GradeCollection
        {
            get => _gradeCollection;
            set
            {
                _gradeCollection = value;
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
            }
        }

        private ObservableCollection<string> _semesterCollection;

        public ObservableCollection<string> SemesterCollection
        {
            get => _semesterCollection;
            set
            {
                _semesterCollection = value;
                OnPropertyChanged();
            }
        }

        private string _selectedSemester;

        public string SelectedSemester
        {
            get => _selectedSemester;
            set
            {
                _selectedSemester = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<string, Classroom> ClassroomDic { get; set; }
        private ObservableCollection<string> _classroomCollection;

        public ObservableCollection<string> ClassroomCollection
        {
            get => _classroomCollection;
            set
            {
                _classroomCollection = value;
                OnPropertyChanged();
            }
        }

        private string _selectedclassroom;

        public string SelectedClassroom
        {
            get => _selectedclassroom;
            set
            {
                _selectedclassroom = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Commands

        public ICommand SaveChangeCommand { get; set; }
        public ICommand DiscardChangeCommand { get; set; }

        #endregion Commands

        public ScoreSubjectViewModel()
        {
            SaveChangeCommand = new RelayCommand(SaveChange);
            DiscardChangeCommand = new RelayCommand(DiscardChange);

            LoadTable();
            LoadCombobox();
        }

        private async void LoadTable()
        {
            //Create table
            await LoadTableColumn();
            dataTable.Rows.Add("HS0033333333333331", "Nguyễn Văn An");
            dataTable.Rows.Add("HS0011111111111", "Nguyễn Văn Aaaaaaaaaaaaaa");
            dataTable.Rows.Add("HS0012222222222222222", "Nguyễn Văn Affffffffffff");
            for (int i = 1; i <= 20; i++)
            {
                dataTable.Rows.Add("HS0012222222222222222", "Nguyễn Văn Affffffffffff");
            }

            //Load data into table
            await LoadData();

            //Event
            dataTable.ColumnChanged += OnColumnChanged;
        }

        private async Task LoadTableColumn()
        {
            dataTable = new DataTable();
            dataTable.Columns.Add("Mã học sinh", typeof(string));
            dataTable.Columns.Add("Tên học sinh", typeof(string));
            dataTable.Columns["Mã học sinh"].ReadOnly = true;
            dataTable.Columns["Tên học sinh"].ReadOnly = true;
            TableView = dataTable.DefaultView;

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
                    dataTable.Columns.Add(columnName, typeof(double));
                }
            }

            dataTable.Columns.Add("Điểm TB", typeof(double));
            dataTable.Columns["Điểm TB"].ReadOnly = true;

            TableView = dataTable.DefaultView;
        }

        private async Task LoadData()
        {
            if (dataTable.Columns.Count == 0 || curSubject == null)
                return;

            var scoreSheet = await GenericDataService<SubjectScoreType>.Instance.GetManyAsync(sc => sc.SubjectId == curSubject.Id,
                                                                                              score => score.Include(sc => sc.ScoreType));
            var scoreTypeIds = scoreSheet.Select(ss => ss.Id).ToList();
            var scoreDetails = await GenericDataService<Score>.Instance.GetManyAsync(score => scoreTypeIds.Contains(score.SubjectScoreTypeId),
                                                                                     s => s.Include(s => s.StudentAssignment).ThenInclude(sa => sa.Student));

            var studentIDList = scoreDetails.Select(score => score.StudentAssignment.StudentId).Distinct().ToList();

            foreach (string studentId in studentIDList)
            {
                DataRow dataRow = dataTable.NewRow();

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
                            sumScore += scores[i-1].Value * subjectScoreType.ScoreType.Coefficient;
                            sumCoefficient += subjectScoreType.ScoreType.Coefficient;
                        }
                        else
                            dataRow[columnName] = DBNull.Value;
                    }
                }

                dataRow["Điểm TB"] = sumScore / sumCoefficient;

                dataTable.Rows.Add(dataRow);
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

        private async void LoadCombobox()
        {
            await LoadSchoolYear();
            await LoadGrade();
            await LoadSemester();
        }

        private async Task LoadSchoolYear()
        {
            SchoolYearCollection = new ObservableCollection<string>();
            IEnumerable<SchoolYear> sy = await GenericDataService<SchoolYear>.Instance.GetAllAsync();
            var schoolYears = sy.ToList();

            if (!schoolYears.Any())
                return;

            foreach (SchoolYear schoolYear in schoolYears)
            {
                string schoolyearstr = schoolYear.StartYear + "-" + schoolYear.EndYear;
                SchoolYearCollection.Add(schoolyearstr);
                SchoolYearDic.Add(schoolyearstr, schoolYear);
            }
        }

        private async Task LoadGrade()
        {
            GradeCollection = new ObservableCollection<string>();
            var grades = (await GenericDataService<Grade>.Instance.GetAllAsync()).ToList();

            if (!grades.Any())
                return;

            foreach (Grade grade in grades)
            {
                string gradeName = $"Khối {grade.Level}";
                GradeCollection.Add(gradeName);
                GradeDic.Add(gradeName, grade);
            }

            GradeCollection.Add("Tất cả các khối");
        }

        private async Task LoadSubject(SchoolYear schoolYear, IEnumerable<Grade> grades)
        {
            if (schoolYear == null)
                return;

            SubjectCollection = new ObservableCollection<string>();
            var subjects = (await GenericDataService<Subject>.Instance.GetManyAsync(s => s.YearId == schoolYear.Id)).ToList();

            if (!subjects.Any() || grades == null || !grades.Any())
                return;

            var gradeId = grades.Select(g => g.Id);
            subjects = subjects.Where(s => gradeId.Contains(s.GradeId)).ToList();

            foreach (var subject in subjects)
            {
                Grade curGrade = grades.FirstOrDefault(g => g.Id == subject.GradeId);
                string subjectName = $"{subject.Name} khối {curGrade.Level}";
                SubjectCollection.Add(subjectName);
                SubjectDic.Add(subjectName, subject);
            }

            SubjectCollection.Add("Tất cả các môn");
        }

        private async Task LoadSemester()
        {
            SemesterCollection = new ObservableCollection<string>();
            SemesterCollection.Add("Kỳ 1");
            SemesterCollection.Add("Kỳ 2");
        }

        private async Task LoadClassroom(SchoolYear schoolYear, IEnumerable<Grade> grades)
        {
            ClassroomCollection = new ObservableCollection<string>();
            var classrooms = (await GenericDataService<Classroom>.Instance.GetManyAsync(c => c.YearId == schoolYear.Id)).ToList();

            if (!classrooms.Any() || grades == null || !grades.Any())
                return;

            var gradeId = grades.Select(g => g.Id);
            classrooms = classrooms.Where(c => gradeId.Contains(c.GradeId)).ToList();

            foreach (var classroom in classrooms)
            {
                string classroomName = $"Lớp {classroom.Name}";
                ClassroomCollection.Add(classroomName);
                ClassroomDic.Add(classroomName, classroom);
            }

            ClassroomCollection.Add("Tất cả các lớp");
        }

        public async void OnFilterChangedLoad()
        {
            if (string.IsNullOrEmpty(SelectedSchoolYear) || string.IsNullOrEmpty(SelectedGrade))
                return;

            SchoolYear schoolYear = SchoolYearDic[SelectedSchoolYear];
            List<Grade> grades = new List<Grade>();

            if (SelectedGrade != "Tất cả các khối")
                grades.Add(GradeDic[SelectedGrade]);
            else
            {
                foreach (var gradePair in GradeDic)
                {
                    grades.Add(gradePair.Value);
                }
            }

            await LoadSubject(schoolYear, grades);
            await LoadClassroom(schoolYear, grades);
        }

        public async void OnFilterChange()
        {
            if (string.IsNullOrEmpty(SelectedSchoolYear) || string.IsNullOrEmpty(SelectedGrade) || string.IsNullOrEmpty(SelectedSubject))
                return;

            SchoolYear schoolYear = SchoolYearDic[SelectedSchoolYear];

            List<Grade> grades = new List<Grade>();
            if (SelectedGrade != "Tất cả các khối")
                grades.Add(GradeDic[SelectedGrade]);
            else
            {
                foreach (var gradePair in GradeDic)
                {
                    grades.Add(gradePair.Value);
                }
            }

            List<Subject> subjects = new List<Subject>();
            if (SelectedSubject != "Tất cả các môn") 
                subjects.Add(SubjectDic[SelectedSubject]);
            else
            {
                foreach (var subjectPair in SubjectDic)
                {
                    subjects.Add(subjectPair.Value);
                }
            }

        }

        public void OnSearchTextChange()
        {
            TableView = SearchAllColumns(SearchText, false, false);
        }

        public DataView SearchAllColumns(string searchText, bool exactMatch = false, bool caseSensitive = false)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return dataTable.DefaultView;
            }

            var culture = new CultureInfo("vi-VN");
            var compareInfo = culture.CompareInfo;
            var compareOptions = caseSensitive ? CompareOptions.None : CompareOptions.IgnoreCase;

            var filteredRows = dataTable.AsEnumerable()
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

            return filteredRows.Any() ? filteredRows.CopyToDataTable().DefaultView : new DataView(dataTable.Clone());
        }
    }
}