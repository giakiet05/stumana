using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels;

public class AddStudentToClassViewModel : BaseViewModel
{
    public ObservableCollection<StudentChoiceInfo> StudentChoiceTableView { get; set; }

    public Classroom CurClassroom { get; set; }

    public ICommand CancelCommand { get; set; }

    public ICommand SaveChangeCommand { get; set; }

    public AddStudentToClassViewModel(Classroom classroom)
    {
        CurClassroom = classroom;

        SaveChangeCommand = new RelayCommand(SaveChange);
        CancelCommand = new RelayCommand(() => ModalNavigationStore.Instance.Close());

        LoadData();
    }

    public async void LoadData()
    {
        StudentChoiceTableView = new ObservableCollection<StudentChoiceInfo>();
        var studentAssignment = (await GenericDataService<StudentAssignment>.Instance.GetAllAsync()).Select(sa => sa.StudentId).Distinct().ToList();
        var unassignStudent = (await GenericDataService<Student>.Instance.GetManyAsync(s => !studentAssignment.Contains(s.Id))).ToList();

        foreach (Student student in unassignStudent)
        {
            StudentChoiceInfo choice = new StudentChoiceInfo
            {
                IsSelected = false,
                StudentID = student.Id,
                Name = student.Name,
                Birthday = student.Birthday,
                Email = student.Email,
                PhoneNumber = student.Phone
            };

            StudentChoiceTableView.Add(choice);
        }
    }

    private async void SaveChange()
    {
        //int maxStudentNum = (await GenericDataService<Asset>.Instance.GetOneAsync(a => a.YearId == CurClassroom.YearId)).
        int studentNum = (await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.ClassroomId == CurClassroom.Id)).Count();
        var studentAdded = StudentChoiceTableView.Where(s => s.IsSelected).ToList();

        if (studentAdded.Count + studentNum > 40)
        {
            ToastMessageViewModel.ShowErrorToast("Số lượng học sinh quá sĩ số tối đa");
            return;
        }

        foreach (var student in studentAdded)
        {
            List<StudentAssignment> newStudentAssignments = new();
            newStudentAssignments.Add(new StudentAssignment
            {
                Semester = 1,
                Conduct = String.Empty,
                Absence = 0,
                StudentId = student.StudentID,
                ClassroomId = CurClassroom.Id
            });
            newStudentAssignments.Add(new StudentAssignment
            {
                Semester = 2,
                Conduct = String.Empty,
                Absence = 0,
                StudentId = student.StudentID,
                ClassroomId = CurClassroom.Id
            });

            await GenericDataService<StudentAssignment>.Instance.CreateManyAsync(newStudentAssignments);
        }

        ToastMessageViewModel.ShowSuccessToast("Thêm học sinh thành công");
        ModalNavigationStore.Instance.Close();
    }
}

public class StudentChoiceInfo : INotifyPropertyChanged
{
    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
                //OnSelectedChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    //public event EventHandler? OnSelectedChanged;

    public string StudentID { get; set; }
    public string Name { get; set; }
    public DateTime Birthday { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}