using System.Text.RegularExpressions;
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels
{
    public class AddStudentViewModel : BaseViewModel
    {
        private string _studentID = String.Empty;

        public string StudentID
        {
            get => _studentID;
            set
            {
                _studentID = value;
                OnPropertyChanged();
            }
        }

        private string _name = String.Empty;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private bool _isNameInvalid;

        public bool IsNameInvalid
        {
            get => _isNameInvalid;
            set
            {
                _isNameInvalid = value;
                OnPropertyChanged();
            }
        }

        private string _email = String.Empty;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private bool _isEmailInvalid;

        public bool IsEmailInvalid
        {
            get => _isEmailInvalid;
            set
            {
                _isEmailInvalid = value;
                OnPropertyChanged();
            }
        }

        private string _selectedGender;

        public string SelectedGender
        {
            get => _selectedGender;
            set
            {
                _selectedGender = value;
                OnPropertyChanged();
            }
        }

        private DateTime _dateOfBirth = DateTime.Now;

        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged();
            }
        }

        private string _address = String.Empty;

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        private string _phoneNumber = String.Empty;

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        private bool _isPhoneInvalid;

        public bool IsPhoneNumberInvalid
        {
            get => _isPhoneInvalid;
            set
            {
                _isPhoneInvalid = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddStudentCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public AddStudentViewModel()
        {
            AddStudentCommand = new RelayCommand(AddStudent);
            CancelCommand = new RelayCommand(() => ModalNavigationStore.Instance.Close());

            GenerateStudentID();
        }

        private async void GenerateStudentID()
        {
            string idPrefix = "STU";

            Student? lastStudent = null;
            lastStudent = (await GenericDataService<Student>.Instance.GetAllAsync()).OrderByDescending(s => int.Parse(s.Id.Substring(idPrefix.Length))).FirstOrDefault();
            int idCount = 1;
            if (lastStudent != null)
            {
                idCount = int.Parse(lastStudent.Id.Substring(idPrefix.Length));
                idCount++;
            }

            StudentID = idPrefix + idCount.ToString("D5");
        }

        private bool CheckEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        private void CheckError()
        {
            IsNameInvalid = false;
            IsEmailInvalid = false;
            IsPhoneNumberInvalid = false;

            if (string.IsNullOrEmpty(Name) || Name.Length > 120)
                IsNameInvalid = true;

            if (string.IsNullOrEmpty(Email) || !CheckEmail(Email))
                IsEmailInvalid = true;

            if (string.IsNullOrEmpty(PhoneNumber) || PhoneNumber.Length != 10)
                IsPhoneNumberInvalid = true;
        }

        private async void AddStudent()
        {
            try
            {
                CheckError();
                if (IsEmailInvalid || IsPhoneNumberInvalid || IsNameInvalid)
                    throw new Exception("Invalid Input");

                Student student = new Student
                {
                    Id = this.StudentID,
                    Name = this.Name,
                    Email = this.Email,
                    Phone = this.PhoneNumber,
                    Gender = this.SelectedGender,
                    Birthday = this.DateOfBirth,
                    Address = this.Address,
                };

                await GenericDataService<Student>.Instance.CreateOneAsync(student);
                ToastMessageViewModel.ShowSuccessToast("Thêm học sinh thành công");
                ModalNavigationStore.Instance.Close();
            }
            catch (Exception e)
            {
                ToastMessageViewModel.ShowErrorToast($"Không thể thêm học sinh. Đã có lỗi xảy ra. {e.Message}");
            }
        }
    }
}