using Stumana.WPF.Commands;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace Stumana.WPF.ViewModels.MainViewModels.ReportOption
{
    public class ReportSubjectViewModel : BaseViewModel
    {
        private readonly GenericDataService<SchoolYear> _schoolYearService;
        private readonly GenericDataService<Subject> _subjectService;
        private readonly GenericDataService<Score> _scoreService;
        private readonly GenericDataService<StudentAssignment> _studentAssignmentService;

        private ICommand _exportCommand;
        public ICommand ExportCommand => _exportCommand ??= new RelayCommand(ExecuteExport);

        private ObservableCollection<SchoolYear> _schoolYearCollection;
        public ObservableCollection<SchoolYear> SchoolYearCollection
        {
            get => _schoolYearCollection;
            set
            {
                _schoolYearCollection = value;
                OnPropertyChanged();
            }
        }

        private SchoolYear _selectedSchoolYear;
        public SchoolYear SelectedSchoolYear
        {
            get => _selectedSchoolYear;
            set
            {
                _selectedSchoolYear = value;
                OnPropertyChanged();
                OnSelectionReportFilterChange();
            }
        }

        private ObservableCollection<Subject> _subjectCollection;
        public ObservableCollection<Subject> SubjectCollection
        {
            get => _subjectCollection;
            set
            {
                _subjectCollection = value;
                OnPropertyChanged();
            }
        }

        private Subject _selectedSubject;
        public Subject SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged();
                OnSelectionReportFilterChange();
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
                OnSelectionReportFilterChange();
            }
        }

        private DataTable _reportTable;
        public DataTable ReportTable
        {
            get => _reportTable;
            set
            {
                _reportTable = value;
                OnPropertyChanged();
            }
        }

        private DataView _reportTableView;
        public DataView ReportTableView
        {
            get => _reportTableView;
            set
            {
                _reportTableView = value;
                OnPropertyChanged();
            }
        }

        public ReportSubjectViewModel()
        {
            _schoolYearService = GenericDataService<SchoolYear>.Instance;
            _subjectService = GenericDataService<Subject>.Instance;
            _scoreService = GenericDataService<Score>.Instance;
            _studentAssignmentService = GenericDataService<StudentAssignment>.Instance;

            LoadReportTableColumn();
            LoadSchoolYearsAsync();
            LoadSubjectsAsync();
            LoadSemestersAsync();
        }

        private void LoadReportTableColumn()
        {
            ReportTable = new DataTable();
            ReportTable.Columns.Add("STT", typeof(int));
            ReportTable.Columns.Add("Lớp", typeof(string));
            ReportTable.Columns.Add("Số lượng đạt", typeof(int));
            ReportTable.Columns.Add("Tỉ lệ", typeof(string));
            ReportTableView = ReportTable.DefaultView;
        }

        private async Task LoadSchoolYearsAsync()
        {
            var schoolYears = await _schoolYearService.GetAllAsync();
            SchoolYearCollection = new ObservableCollection<SchoolYear>(schoolYears);
        }

        private async Task LoadSubjectsAsync()
        {
            var subjects = await _subjectService.GetAllAsync();
            SubjectCollection = new ObservableCollection<Subject>(subjects);
        }

        private void LoadSemestersAsync()
        {
            SemesterCollection = new ObservableCollection<string> { "Học kỳ 1", "Học kỳ 2" };
        }

        private async void OnSelectionReportFilterChange()
        {
            if (SelectedSchoolYear == null || SelectedSubject == null || string.IsNullOrEmpty(SelectedSemester))
                return;

            ReportTable.Clear();

            var studentAssignments = await _studentAssignmentService.GetManyAsync(
                sa => sa.Classroom.YearId == SelectedSchoolYear.Id
            );

            var scores = await _scoreService.GetManyAsync(
                s => s.SubjectScoreType.SubjectId == SelectedSubject.Id
            );

            int stt = 1;
            foreach (var classroom in studentAssignments.Select(sa => sa.Classroom).Distinct())
            {
                var classScores = scores.Where(s =>
                    studentAssignments.Any(sa =>
                        sa.ClassroomId == classroom.Id &&
                        sa.Id == s.StudentAssignmentId
                    )
                );

                int passedCount = classScores.Count(s => s.Value >= 5.0);
                int totalCount = classScores.Count();
                double passRate = totalCount > 0 ? (double)passedCount / totalCount * 100 : 0;

                ReportTable.Rows.Add(
                    stt++,
                    classroom.Name,
                    passedCount,
                    $"{passRate:F1}%"
                );
            }
        }

        private void ExecuteExport(object parameter)
        {

        }
    }
}
