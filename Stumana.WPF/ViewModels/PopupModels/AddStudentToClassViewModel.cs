﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels;

public class AddStudentToClassViewModel : BaseViewModel
{
    private List<StudentChoiceInfo> UnassignStudents { get; set; } = new();
    public ObservableCollection<StudentChoiceInfo> StudentChoiceTable { get; set; } = new();
    public int UnassignStudentCount => UnassignStudents.Count;

    public Classroom CurClassroom { get; set; }

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

    public ICommand CancelCommand { get; set; }
    public ICommand SaveChangeCommand { get; set; }
    public ICommand RowClickCommand { get; set; }

    private readonly EventHandler? OnUpdateData;

    public AddStudentToClassViewModel(Classroom classroom, EventHandler? onStudentDataChanged)
    {
        CurClassroom = classroom;
        OnUpdateData = onStudentDataChanged;

        SaveChangeCommand = new RelayCommand(SaveChange);
        CancelCommand = new RelayCommand(ModalNavigationStore.Instance.Close);
        RowClickCommand = new RelayCommand(obj =>
        {
            if (obj is StudentChoiceInfo studentChoiceInfo)
            {
                studentChoiceInfo.IsSelected = !studentChoiceInfo.IsSelected;
                OnPropertyChanged(nameof(studentChoiceInfo));
            }
        });

        LoadData();
    }

    public async void LoadData()
    {
        var studentAssignments = await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.Classroom.YearId == CurClassroom.YearId ,
                                                                                                   query => query.Include(sa => sa.Classroom));

        var studentWithClassIds = studentAssignments.Select(sa => sa.StudentId).Distinct().ToList();
        var unassignStudent = (await GenericDataService<Student>.Instance.GetManyAsync(s => !studentWithClassIds.Contains(s.Id))).ToList();
       var asset = await GenericDataService<Asset>.Instance.GetOneAsync(a => a.YearId == CurClassroom.YearId, s=>s.Include(a=>a.Year));

        foreach (Student student in unassignStudent)
        {
           int birthYear = student.Birthday.Year;
           int age = asset.Year.StartYear - birthYear;
            // Check if the student's age is within the allowed range for the asset
            if (age >= asset.MinAge && age <= asset.MaxAge)
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

                StudentChoiceTable.Add(choice);
                UnassignStudents.Add(choice);
            }
        }
       
    }

    private async void SaveChange()
    {
        int maxStudentNum = (await GenericDataService<Asset>.Instance.GetOneAsync(a => a.YearId == CurClassroom.YearId)).MaxCapacity;
        int studentNum = (await GenericDataService<StudentAssignment>.Instance.GetManyAsync(sa => sa.ClassroomId == CurClassroom.Id)).Count();
        var studentAdded = StudentChoiceTable.Where(s => s.IsSelected).ToList();

        if (studentAdded.Count + studentNum > maxStudentNum)
        {
            ToastMessageViewModel.ShowErrorToast("Số lượng học sinh quá sĩ số tối đa");
            return;
        }

        foreach (var student in studentAdded)
        {
            List<StudentAssignment> newStudentAssignments = new();
            newStudentAssignments.Add(new StudentAssignment
            {
                Id = Guid.NewGuid().ToString(),
                Semester = 1,
                Conduct = string.Empty,
                Absence = 0,
                Comment = string.Empty,
                StudentId = student.StudentID,
                ClassroomId = CurClassroom.Id
            });
            newStudentAssignments.Add(new StudentAssignment
            {
                Id = Guid.NewGuid().ToString(),
                Semester = 2,
                Conduct = string.Empty,
                Absence = 0,
                Comment = string.Empty,
                StudentId = student.StudentID,
                ClassroomId = CurClassroom.Id
            });

            await GenericDataService<StudentAssignment>.Instance.CreateManyAsync(newStudentAssignments);
        }

        ToastMessageViewModel.ShowSuccessToast("Thêm học sinh thành công");
        OnUpdateData?.Invoke(this, EventArgs.Empty);
        ModalNavigationStore.Instance.Close();
    }

    public void OnSearchTextChange()
    {
        SearchAllColumns(UnassignStudents, StudentChoiceTable, SearchText, false, false);
    }

    public void SearchAllColumns<T>(List<T> originalList, ObservableCollection<T> table, string searchText, bool exactMatch = false, bool caseSensitive = false)
    {
        if (originalList == null)
            return;

        var culture = new CultureInfo("vi-VN");
        var compareInfo = culture.CompareInfo;
        var compareOptions = caseSensitive ? CompareOptions.None : CompareOptions.IgnoreCase;

        IEnumerable<T> filtered;

        if (string.IsNullOrEmpty(searchText))
            filtered = originalList;
        else
        {
            filtered = originalList.Where(s =>
            {
                bool Match(string? value) => !string.IsNullOrEmpty(value) && (exactMatch ? compareInfo.Compare(value, searchText, compareOptions) == 0 : compareInfo.IndexOf(value, searchText, compareOptions) >= 0);
                PropertyInfo[] properties = typeof(T).GetProperties();
                bool IsMatch = false;

                foreach (var property in properties)
                {
                    var rawValue = property.GetValue(s);
                    if (rawValue == null)
                        continue;

                    string? stringValue = rawValue is DateTime dt ? dt.ToString("dd/MM/yyyy", culture) : rawValue.ToString();
                    IsMatch = IsMatch || Match(stringValue);
                }

                return IsMatch;
            });
        }

        table.Clear();
        foreach (var row in filtered)
        {
            table.Add(row);
        }
    }

    public sealed class StudentChoiceInfo : INotifyPropertyChanged
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public string StudentID { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}