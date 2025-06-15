using System.Text.RegularExpressions;
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels
{
    public class CreateAccountViewModel : BaseViewModel
    {
        #region Properties

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private bool _invalidEmail = false;
        public bool InvalidEmail 
        { 
            get => _invalidEmail;
            set
            {
                _invalidEmail = value;
                OnPropertyChanged();
            }
        }

        private string _errorEmailText = "Email không hợp lệ";
        public string ErrorEmailText
        {
            get => _errorEmailText;
            set
            {
                _errorEmailText = value;
                OnPropertyChanged();
            }
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private bool _invalidUsername = false;
        public bool InvalidUsername 
        { 
            get => _invalidUsername;
            set
            {
                _invalidUsername = value;
                OnPropertyChanged();
            }
        }

        private string _errorUsernameText = "Username không hợp lệ";
        public string ErrorUsernameText
        {
            get => _errorUsernameText;
            set
            {
                _errorUsernameText = value;
                OnPropertyChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private bool _invalidPassword = false;
        public bool InvalidPassword 
        { 
            get => _invalidPassword;
            set
            {
                _invalidPassword = value;
                OnPropertyChanged();
            }
        }

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }

        private bool _invalidConfirmPassword = false;
        public bool InvalidConfirmPassword 
        { 
            get => _invalidConfirmPassword;
            set
            {
                _invalidConfirmPassword = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Commands
        public ICommand CreateAccountCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion Commands

        public CreateAccountViewModel()
        {
            CreateAccountCommand = new RelayCommand(CreateAccount);
            CancelCommand = new RelayCommand(ModalNavigationStore.Instance.Close);
        }

        private async void CreateAccount()
        {
            try
            {
                if (await CheckError())
                    throw new Exception();

                User user = new User(Username, Password, Email);
                await GenericDataService<User>.Instance.CreateOneAsync(user);
                ToastMessageViewModel.ShowSuccessToast("Tạo tài khoản thành công");
                ModalNavigationStore.Instance.Close();
            }
            catch (Exception e)
            {
                // Error handling - properties will update automatically due to binding
            }
        }

        private async Task<bool> CheckError()
        {
            InvalidUsername = false;
            InvalidEmail = false;
            InvalidPassword = false;
            InvalidConfirmPassword = false;
            ErrorUsernameText = "Username không hợp lệ";
            ErrorEmailText = "Email không hợp lệ";

            if (string.IsNullOrWhiteSpace(Username))
                InvalidUsername = true;
            else
            {
                User? userUN = await GenericDataService<User>.Instance.GetOneAsync(u => u.Username == Username);
                if (userUN != null)
                {
                    InvalidUsername = true;
                    ErrorUsernameText = "Username đã được sử dụng";
                }
            }

            if (string.IsNullOrWhiteSpace(Email))
                InvalidEmail = true;
            else
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                Regex regex = new Regex(pattern);
                if (!regex.IsMatch(Email))
                    InvalidEmail = true;
                else
                {
                    User? userEm = await GenericDataService<User>.Instance.GetOneAsync(u => u.Email == Email);
                    if (userEm != null)
                    {
                        InvalidEmail = true;
                        ErrorEmailText = "Email đã được sử dụng";
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(Password))
                InvalidPassword = true;

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
                InvalidConfirmPassword = true;

            if (Password != ConfirmPassword)
                InvalidConfirmPassword = true;

            if (InvalidUsername || InvalidEmail || InvalidPassword || InvalidConfirmPassword)
                return true;
            return false;
        }
    }
} 