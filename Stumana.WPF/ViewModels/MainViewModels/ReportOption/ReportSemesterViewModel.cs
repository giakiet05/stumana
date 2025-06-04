using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using static System.Formats.Asn1.AsnWriter;

namespace Stumana.WPF.ViewModels.MainViewModels.ReportOption
{
    public class ReportSemesterViewModel : BaseViewModel
    {
        private readonly GenericDataService<Score> _scoreService;
        private readonly GenericDataService<StudentAssignment> _studentAssignmentService;
        private readonly GenericDataService<Subject> _subjectService;
        private readonly GenericDataService<SubjectScoreType> _subjectScoreTypeService;

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

        public ReportSemesterViewModel()
        {
            _scoreService = GenericDataService<Score>.Instance;
            _studentAssignmentService = GenericDataService<StudentAssignment>.Instance;
            _subjectService = GenericDataService<Subject>.Instance;
            _subjectScoreTypeService = GenericDataService<SubjectScoreType>.Instance;
            LoadReportTableColumn();
            LoadSchoolYearsAsync();
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
        

        private void LoadSemestersAsync()
        {
            SemesterCollection = new ObservableCollection<string> { "Học kỳ 1", "Học kỳ 2" };
        }

        private async void OnSelectionReportFilterChange()
        {
            if (SelectedSchoolYear == null || string.IsNullOrEmpty(SelectedSemester))
                return;
            Asset asset = await GenericDataService<Asset>.Instance.GetOneAsync(a => a.YearId == SchoolYearDic[SelectedSchoolYear].Id);
            float scoreToPass = (float)asset.ScoreToPass;
            ReportTable.Clear();
            int semester = SelectedSemester == "Học kỳ 1" ? 1 : 2;
            var studentAssignments = (await _studentAssignmentService.GetManyAsync(
                sa => sa.Classroom.YearId == SchoolYearDic[SelectedSchoolYear].Id
                && sa.Semester ==semester ,
                cl => cl.Include(sa => sa.Classroom))).ToList();
            var studentAssignmentIds = studentAssignments.Select(sa => sa.Id).ToList();

          
            int stt = 1;
            var classrooms = studentAssignments.Select(sa => sa.Classroom).Distinct();
            foreach (var classroom in classrooms)
            {
                var subjects = await _subjectService.GetManyAsync(
                    s => s.GradeId == classroom.GradeId 
                    && s.YearId == classroom.YearId
                );
                int passedCount = 0;
                int totalCount = studentAssignments.Count();

                foreach (var sa in studentAssignments.Where(s=>s.Classroom == classroom)){
                    float totalScore = 0;
                    foreach (var subject in subjects)
                    {
                        var subjectScoreTypes = await _subjectScoreTypeService.GetManyAsync(
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
                            totalScore += (float)((tempScore / tempCoefficient) / subjects.Count());
                        }
                    }
                    if (totalScore >= scoreToPass) passedCount++;
                   
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
            // TODO: Implement export functionality
        }
    }
}
