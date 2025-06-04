using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows.Input;
using static System.Formats.Asn1.AsnWriter;

namespace Stumana.WPF.ViewModels.MainViewModels.StudentOption
{
    public class StudentInfoViewModel : BaseViewModel
    {
        public Student? CurStudent { get; set; }
        public List<StudentAssignment> StudentAssignments { get; set; }
        private Dictionary<string, double> ScoreTypeCoefficientDic { get; set; } = new();

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

        private string _studentId;


        public string StudentId
        {
            get => _studentId;
            set
            {
                _studentId = value;
                OnPropertyChanged();
            }
        }

        private string _studentName;

        public string StudentName
        {
            get => _studentName;
            set
            {
                _studentName = value;
                OnPropertyChanged();
            }
        }


        public Dictionary<string, SchoolYear> SchoolYearDic { get; set; } = new();
        private ObservableCollection<string> _schoolyearFilter = new();

        public ObservableCollection<string> SchoolYearFilter
        {
            get => _schoolyearFilter;
            set
            {
                _schoolyearFilter = value;
                OnPropertyChanged();
            }
        }

        private string? _selectedSchoolYear;

        public string? SelectedSchoolYear
        {
            get => _selectedSchoolYear;
            set
            {
                if (_selectedSchoolYear != value)
                {
                    _selectedSchoolYear = value;
                    OnPropertyChanged();
                    LoadSemesterFilter();
                }
            }
        }

        public Dictionary<string, int> SemesterDic { get; set; } = new();
        private ObservableCollection<string> _semesterFilter = new();

        public ObservableCollection<string> SemesterFilter
        {
            get => _semesterFilter;
            set
            {
                _semesterFilter = value;
                OnPropertyChanged();
            }
        }

        private string? _selectedSemester;

        public string? SelectedSemester
        {
            get => _selectedSemester;
            set
            {
                if (_selectedSemester != value)
                {
                    _selectedSemester = value;
                    OnPropertyChanged();
                    LoadTable();
                    LoadClassroom();
                    CaculateGrading();
                }
            }
        }

        private string? _classroomName;

        public string? ClassroomName
        {
            get => _classroomName;
            set
            {
                _classroomName = value;
                OnPropertyChanged();
            }
        }

        private string? _absence;

        public string? Absence
        {
            get => _absence;
            set
            {
                _absence = value;
                OnPropertyChanged();
            }
        }


        private string? _grading;

        public string? Grading
        {
            get => _grading;
            set
            {
                _grading = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ConductCollection { get; set; }
        private string? _selectedConduct;

        public string? SelectedConduct
        {
            get => _selectedConduct;
            set
            {
                _selectedConduct = value;
                OnPropertyChanged();
                if (value != null) CaculateGrading();
            }
        }

        private string _evaluation;

        public string Evaluation
        {
            get => _evaluation;
            set
            {
                _evaluation = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseModalCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public StudentInfoViewModel(Student? student)
        {
            CurStudent = student;
            LoadStudentAssignment(student);

            StudentId = student.Id;
            StudentName = student.Name;

            CloseModalCommand = new RelayCommand(ModalNavigationStore.Instance.Close);
            SaveCommand = new RelayCommand(Save);
            LoadSchoolYearFilter();

            ConductCollection = new ObservableCollection<string>
            {
                "Tốt",
                "Khá",
                "Trung bình",
                "Yếu",
                "Kém"
            };
        }
        public async void Save()
        {
            if (SelectedSchoolYear == null || SelectedSemester == null)
                return;

            StudentAssignment? studentAssignment = StudentAssignments.FirstOrDefault(sa => sa.Classroom.YearId == SchoolYearDic[SelectedSchoolYear].Id
                                                                                           && sa.Semester == SemesterDic[SelectedSemester]);

            if (studentAssignment == null)
                return;

            if (!string.IsNullOrEmpty(Absence))
                studentAssignment.Absence = int.Parse(Absence);

           
            if (SelectedConduct != null)
                studentAssignment.Conduct = SelectedConduct;

            if (!string.IsNullOrEmpty(Evaluation))
                studentAssignment.Comment = Evaluation;

            await GenericDataService<StudentAssignment>.Instance.UpdateOneAsync(studentAssignment, sa => sa.Id == studentAssignment.Id);

            ToastMessageViewModel.ShowSuccessToast("Cập nhật thành công");
        }
        private async void LoadStudentAssignment(Student? student)
        {
            if (student == null)
                return;

            StudentAssignments = (await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.StudentId == student.Id,
                                                                                                    query => query.Include(sa => sa.Classroom))).ToList();
        }

        private async void LoadSchoolYearFilter()
        {
            SchoolYearFilter.Clear();
            SchoolYearDic.Clear();
            if (CurStudent == null)
                return;


            List<string> yearStrList = new List<string>();
            var yearIds = StudentAssignments.Select(sa => sa.Classroom.YearId).Distinct();
            var schoolYears = (await GenericDataService<SchoolYear>.Instance.GetManyAsync(sy => yearIds.Contains(sy.Id))).ToList();
            foreach (var schoolYear in schoolYears)
            {
                string yearStr = $"{schoolYear.StartYear} - {schoolYear.EndYear}";
                yearStrList.Add(yearStr);
                SchoolYearDic.Add(yearStr, schoolYear);
            }

            if (yearStrList.Any())
            {
                yearStrList.Sort((a, b) =>
                {
                    int startA = int.Parse(a.Split(" - ")[0]);
                    int startB = int.Parse(b.Split(" - ")[0]);
                    return startA.CompareTo(startB);
                });

                foreach (var year in yearStrList)
                {
                    SchoolYearFilter.Add(year);
                }

                SelectedSchoolYear = SchoolYearFilter[0];
            }
        }

        private async void LoadSemesterFilter()
        {
            SemesterFilter.Clear();
            SemesterDic.Clear();
            if (SelectedSchoolYear == null)
                return;

            var semesters = StudentAssignments.Where(sa => sa.Classroom.YearId == SchoolYearDic[SelectedSchoolYear].Id).Select(sa => sa.Semester).ToList();
            semesters.Sort();
            foreach (var semester in semesters)
            {
                string semesterStr = $"Kỳ {semester}";
                SemesterFilter.Add(semesterStr);
                SemesterDic.Add(semesterStr, semester);
            }

            if (SemesterFilter.Any())
                SelectedSemester = SemesterFilter[0];
        }

        private async void LoadClassroom()
        {
            ClassroomName = string.Empty;

            if (SelectedSchoolYear == null || SelectedSemester == null)
                return;

            StudentAssignment? studentAssignment = StudentAssignments.FirstOrDefault(sa => sa.Classroom.YearId == SchoolYearDic[SelectedSchoolYear].Id
                                                                                           && sa.Semester == SemesterDic[SelectedSemester]);

            if (studentAssignment == null)
                return;

            ClassroomName = studentAssignment.Classroom.Name;
            Evaluation = studentAssignment.Comment;
            Absence = studentAssignment.Absence.ToString();
            if (!string.IsNullOrEmpty(studentAssignment.Conduct))
            {
                SelectedConduct = studentAssignment.Conduct;
            }
            else
            {
                SelectedConduct = null;
            }
        }
        private async void CaculateGrading()
        {
            if (SelectedSchoolYear == null || SelectedSemester == null)
                return;
            if (String.IsNullOrEmpty(SelectedConduct))
            {
                Grading = "Chưa đánh giá";
                return;
            }
            float _totalScore = 0;
            foreach (DataRow row in ScoreDataTable.Rows)
            {
                if (row["Điểm TB"] is DBNull)
                    continue;
                _totalScore += Convert.ToSingle(row["Điểm TB"]);
            }
            _totalScore /= ScoreDataTable.Rows.Count;
            
          
            if (_totalScore >= 8.0 && SelectedConduct == "Tốt")
                Grading = "Giỏi";
            else if (_totalScore >= 6.5 && (SelectedConduct == "Tốt" || SelectedConduct == "Khá"))
                Grading = "Khá";
            else if (_totalScore >= 5.0 && (SelectedConduct == "Tốt" || SelectedConduct == "Khá" || SelectedConduct == "Trung bình"))
                Grading = "Trung bình";
            else
                Grading = "Yếu";
        }


        private async Task LoadTable()
        {
            if (SelectedSchoolYear == null || SelectedSemester == null)
            {
                ScoreDataTable = new DataTable();
                TableView = ScoreDataTable.DefaultView;
                return;
            }

            StudentAssignment? studentAssignment = StudentAssignments.FirstOrDefault(sa => sa.Classroom.YearId == SchoolYearDic[SelectedSchoolYear].Id
                                                                                           && sa.Semester == SemesterDic[SelectedSemester]);
            if (studentAssignment == null)
                return;

            //Load table column
            ScoreDataTable = new DataTable();
            ScoreDataTable.Columns.Add("Môn học", typeof(string));

            List<Score> scores = (await GenericDataService<Score>.Instance.GetManyAsync(s => s.StudentAssignmentId == studentAssignment.Id,
                                                                                        query => query.Include(s => s.SubjectScoreType))).ToList();

            var scoreTypeIds = scores.Select(s => s.SubjectScoreType.ScoreTypeId).Distinct();
            var scoreTypes = (await GenericDataService<ScoreType>.Instance.GetManyAsync(st => scoreTypeIds.Contains(st.Id))).ToList();
            scoreTypes.Sort((a, b) => a.Coefficient.CompareTo(b.Coefficient));

            foreach (var scoreType in scoreTypes)
            {
                ScoreDataTable.Columns.Add(scoreType.Name, typeof(string));
                ScoreTypeCoefficientDic[scoreType.Name] = scoreType.Coefficient;
            }

            ScoreDataTable.Columns.Add("Điểm TB", typeof(double));
            TableView = ScoreDataTable.DefaultView;

            //Load data
            var subjectIds = scores.Select(s => s.SubjectScoreType.SubjectId).Distinct();
            List<Subject> subjects = (await GenericDataService<Subject>.Instance.GetManyAsync(sb => subjectIds.Contains(sb.Id))).ToList();
            subjects.Sort((a, b) => string.Compare(a.Name, b.Name, CultureInfo.CurrentCulture, CompareOptions.None));

            ScoreDataTable.Clear();
            foreach (var subject in subjects)
            {
                DataRow dataRow = ScoreDataTable.NewRow();
                dataRow["Môn học"] = subject.Name;
                double sumScore = 0;
                double sumCoefficient = 0;

                var curSubjectScores = scores.Where(s => s.SubjectScoreType.SubjectId == subject.Id).ToList();
                var curSubjectScoreTypeIds = curSubjectScores.Select(s => s.SubjectScoreType.ScoreTypeId).Distinct();
                foreach (var curSubjectScoreTypeId in curSubjectScoreTypeIds)
                {
                    ScoreType scoreType = await GenericDataService<ScoreType>.Instance.GetOneAsync(st => st.Id == curSubjectScoreTypeId);
                    List<Score> extractedScores = curSubjectScores.Where(s => s.SubjectScoreType.ScoreTypeId == curSubjectScoreTypeId).ToList();

                    dataRow[scoreType.Name] = string.Join(", ", extractedScores.Select(score => score.Value.ToString("F1")));

                    foreach (var extractedScore in extractedScores)
                    {
                        sumScore += extractedScore.Value * scoreType.Coefficient;
                        sumCoefficient += scoreType.Coefficient;
                    }
                }

                dataRow["Điểm TB"] = sumCoefficient > 0 ? Math.Round(sumScore / sumCoefficient, 2) : DBNull.Value;
                ScoreDataTable.Rows.Add(dataRow);

            }
        }

        
    }
}