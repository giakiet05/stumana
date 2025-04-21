using System.Text.RegularExpressions;
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;

namespace Stumana.WPF.ViewModels.AuthencationViewModels;

public class SignUpViewModel : BaseViewModel
{
    #region Properties

    private string _email;
    public bool InvalidEmail { get; set; } = false;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }

    private string _username;
    public bool InvalidUsername { get; set; } = false;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
        }
    }

    private string _password;
    public bool InvalidPassword { get; set; } = false;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    private string _confirmpassword;
    public bool InvalidConfirmPassword { get; set; } = false;
    public string ConfirmPassword
    {
        get => _confirmpassword;
        set
        {
            _confirmpassword = value;
            OnPropertyChanged();
        }
    }
    #endregion Properties

    #region Commands
    public ICommand CreateAccountCommand { get; set; }
    public ICommand NavigateSignInCommand { get; set; }

    #endregion Commands

    public SignUpViewModel()
    {
        OnPropertyChanged(nameof(InvalidEmail));
        OnPropertyChanged(nameof(InvalidUsername));
        OnPropertyChanged(nameof(InvalidPassword));
        OnPropertyChanged(nameof(InvalidConfirmPassword));

        CreateAccountCommand = new RelayCommand(CreateAccount);
        NavigateSignInCommand = new NavigateViewCommand(() => new SignInViewModel());
    }

    private async void CreateAccount()
    {
        try
        {
            CheckError();
            User user = new User(Username, Password, Email);
            await GenericDataService<User>.Instance.CreateOneAsync(user);
            ToastMessageViewModel.ShowInfoToast("Tạo tài khoản thành công");
            NavigateSignInCommand.Execute(null);
        }
        catch (Exception e)
        {
            OnPropertyChanged(nameof(InvalidEmail));
            OnPropertyChanged(nameof(InvalidUsername));
            OnPropertyChanged(nameof(InvalidPassword));
            OnPropertyChanged(nameof(InvalidConfirmPassword));
        }
    }

    private async void CheckError()
    {
        InvalidUsername = false;
        InvalidEmail = false;
        InvalidPassword = false;
        InvalidConfirmPassword = false;

        if (string.IsNullOrWhiteSpace(Username))
            InvalidUsername = true;

        if (string.IsNullOrWhiteSpace(Email))
            InvalidEmail = true;

        if (string.IsNullOrWhiteSpace(Password))
            InvalidPassword = true;

        if (string.IsNullOrWhiteSpace(ConfirmPassword))
            InvalidConfirmPassword = true;

        if (Password != ConfirmPassword)
            InvalidConfirmPassword = true;

        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        Regex regex = new Regex(pattern);
        if (!regex.IsMatch(Email))
            InvalidEmail = true;

        if (InvalidUsername || InvalidEmail || InvalidPassword || InvalidConfirmPassword)
            throw new Exception();
    }
}