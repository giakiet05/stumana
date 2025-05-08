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

        StudentChoiceInfo student = new StudentChoiceInfo
        {
            IsSelected = true,
            StudentID = "STU00001",
            Name = "An Nam",
            Birthday = DateTime.Now,
            PhoneNumber = "0901295343",
            Email = "admin@gmail.com"
        };

        StudentChoiceTableView.Add(student);
    }

    private async void SaveChange()
    {
        //int maxStudentNum = (await GenericDataService<Asset>.Instance.GetOneAsync(a => a.YearId == CurClassroom.YearId)).
        int studentNum = (await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.ClassroomId == CurClassroom.Id)).Count();
        var studentAdded = StudentChoiceTableView.Where(s => s.IsSelected).ToList();

        //if (studentAdded.Count + studentNum >)
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