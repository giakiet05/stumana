using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using static MaterialDesignThemes.Wpf.Theme;

namespace Stumana.WPF.ViewModels.MainViewModels.ScoreOption
{
    public class ScoreSubjectViewModel : BaseViewModel
    {
        #region Properties

        //private Dictionary<string, int> ScoreTypeDic = new Dictionary<string, int>();
        public Dictionary<DataRow, Dictionary<string, Tuple<double, double?>>> ChangedData = new();

        public Subject curSubject { get; set; }

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

        #endregion Properties

        #region Commands

        public ICommand AddScoreCommand { get; set; }
        public ICommand DeleteScoreCommand { get; set; }
        public ICommand EditScoreCommand { get; set; }
        public ICommand FilterCommand { get; set; }

        #endregion Commands

        public ScoreSubjectViewModel()
        {
            EditScoreCommand = new NavigateModalCommand(() => new EditScoreInputViewModel());

            LoadTableColumn();

            dataTable.Rows.Add("HS0033333333333331", "Nguyễn Văn An");
            dataTable.Rows.Add("HS0011111111111", "Nguyễn Văn Aaaaaaaaaaaaaa");
            dataTable.Rows.Add("HS0012222222222222222", "Nguyễn Văn Affffffffffff");
            for (int i = 0; i < 20; i++)
            {
                dataTable.Rows.Add("HS0012222222222222222", "Nguyễn Văn Affffffffffff");
            }

            LoadData();
            dataTable.ColumnChanged += OnColumnChanged;
        }


        private async Task LoadTableColumn()
        {
            //if (curSubject == null)
            //    return;

            dataTable = new DataTable();
            dataTable.Columns.Add("Mã học sinh", typeof(string));
            dataTable.Columns.Add("Tên học sinh", typeof(string));
            dataTable.Columns["Mã học sinh"].ReadOnly = true;
            dataTable.Columns["Tên học sinh"].ReadOnly = false;
            TableView = dataTable.DefaultView;

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
                    dataTable.Columns.Add(columnName, typeof(double));
                }
            }

            TableView = dataTable.DefaultView;
        }

        private async void LoadData()
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
                            dataRow[columnName] = scores[i - 1].Value;
                        else
                            dataRow[columnName] = DBNull.Value;
                    }
                }

                dataTable.Rows.Add(dataRow);
            }
        }

        public void OnColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            string columnName = e.Column.ColumnName;

            object originalObj = e.Row[columnName, DataRowVersion.Original];
            double originalValue = originalObj == DBNull.Value ? -1 : (double)originalObj;

            object newObj = e.ProposedValue;
            double? newValue = newObj == DBNull.Value ? null : (double)newObj;

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