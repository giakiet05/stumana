using Stumana.WPF.Commands;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

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
        private Dictionary<string, SchoolYear> SchoolYearDic { get; set; } = new();

        private ObservableCollection<string> _schoolYearCollection = new();
        public ObservableCollection<string> SchoolYearCollection
        {
            get => _schoolYearCollection;
            set
            {
                _schoolYearCollection = value;
                OnPropertyChanged();
            }
        }

        private string _selectedSchoolYear;
        public string SelectedSchoolYear
        {
            get => _selectedSchoolYear;
            set
            {
                _selectedSchoolYear = value;
                OnPropertyChanged();
                OnSelectionReportFilterChange();
            }
        }
        private Dictionary<string, Subject> SubjectDic { get; set; } = new();

        private ObservableCollection<string> _subjectCollection = new();
        public ObservableCollection<string> SubjectCollection
        {
            get => _subjectCollection;
            set
            {
                _subjectCollection = value;
                OnPropertyChanged();
            }
        }

        private string _selectedSubject;
        public string SelectedSubject
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
            SchoolYearCollection.Clear();

            List<SchoolYear> schoolYears = (await GenericDataService<SchoolYear>.Instance.GetAllAsync()).ToList();

            if (!schoolYears.Any())
                return;

            foreach (SchoolYear schoolYear in schoolYears)
            {
                string schoolyearName = schoolYear.StartYear + " - " + schoolYear.EndYear;
                SchoolYearCollection.Add(schoolyearName);
                SchoolYearDic.Add(schoolyearName, schoolYear);
            }

            if (SchoolYearCollection.Any())
            {
                SelectedSchoolYear = SchoolYearCollection[0];
            }
        }

        private async Task LoadSubjectsAsync()
        {
           
            SubjectCollection.Clear();
            var subjects = await GenericDataService<Subject>.Instance.GetManyAsync(
                s=>s.YearId == SchoolYearDic[SelectedSchoolYear].Id, 
                qr =>qr.Include(s=> s.Grade));
            if (!subjects.Any())
                return;
            foreach (var subject in subjects)
            {
                string subjectName = subject.Name + " " + subject.Grade.Name;
                SubjectCollection.Add(subjectName);
                SubjectDic.Add(subjectName, subject);
            }
            if (SubjectCollection.Any())
            {
                SelectedSubject = SubjectCollection[0];
            }
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

            int semester = SelectedSemester == "Học kỳ 1" ? 1 : 2;
            Subject subject = SubjectDic[SelectedSubject];
            var classrooms = await GenericDataService<Classroom>.Instance.GetManyAsync(
                c => c.YearId == SchoolYearDic[SelectedSchoolYear].Id &&
                c.GradeId == subject.GradeId
            );
            int stt = 1;
            foreach (var classroom in classrooms)
            {
               var studentAssignments = await GenericDataService<StudentAssignment>.Instance.GetManyAsync(
                    sa => sa.ClassroomId == classroom.Id && sa.Semester == semester
                );
                int totalCount = studentAssignments.Count();
                if (totalCount == 0)
                    continue;
                int passedCount = 0;
                foreach (var sa in studentAssignments)
                {
                    var subjectScoreTypes = await GenericDataService<SubjectScoreType>.Instance.GetManyAsync(
                            sst => sst.SubjectId == subject.Id,
                            qr => qr.Include(sst => sst.ScoreType)
                        );
                    double tempScore = 0;
                    double tempCoefficient = 0;
                    foreach (var subjectScoreType in subjectScoreTypes)
                    {
                        var scores = await _scoreService.GetManyAsync(
                            sc => sc.StudentAssignmentId == sa.Id && sc.SubjectScoreTypeId == subjectScoreType.Id
                        );
                        if (scores.Any())
                        {
                            foreach (var score in scores)
                            {
                                tempScore += score.Value * subjectScoreType.ScoreType.Coefficient;
                                tempCoefficient += subjectScoreType.ScoreType.Coefficient;
                            }
                        }
                    }
                    if (tempCoefficient > 0)
                    {
                        float totalScore = (float)(tempScore / tempCoefficient);
                        if ( totalScore >= subject.ScoreToPass)
                        {
                            passedCount++;
                        }
                    }
                }
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
