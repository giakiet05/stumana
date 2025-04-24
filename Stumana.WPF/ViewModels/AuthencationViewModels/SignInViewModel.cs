using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;
using Stumana.WPF.ViewModels.MainViewModels;

namespace Stumana.WPF.ViewModels.AuthencationViewModels;

public class SignInViewModel : BaseViewModel
{
    #region Command
    public ICommand LoginCommand { get; set; }
    public ICommand NavigateSignUpCommand { get; set; }
    #endregion Command

    #region Properties

    private string _username;
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
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    private string _errormessage;
    public string ErrorMessage
    {
        get => _errormessage;
        set
        {
            _errormessage = value;
            OnPropertyChanged();
        }
    }

    private bool _isLoading;
    public string LoginText { get; set; } = "Login";
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            LoginText = _isLoading == true ? "Loading" : "Login";
            OnPropertyChanged();
            OnPropertyChanged(LoginText);
        }
    }
    #endregion Properties


    public SignInViewModel()
    {
        NavigateSignUpCommand = new NavigateViewCommand(() => new SignUpViewModel());
        LoginCommand = new RelayCommand(ExecuteLogin);
    }

    private async void ExecuteLogin()
    {
        IsLoading = true;
        try
        {
            if (string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password))
            {
                throw new Exception("User not found");
                return;
            }

            User user = await GenericDataService<User>.Instance.GetOneAsync(u => u.Username == Username || u.Email == Username);
            if (user == null)
                throw new Exception("User not found");

            if (!user.VerifyPassword(Password))
                throw new Exception("User not found");
            else
            {
                AccountStore.Instance.CurrentUser = user;
                NavigationStore.Instance.CurrentViewModel = new SidebarViewModel();
                ToastMessageViewModel.ShowSuccessToast("Đăng nhập thành công");
            }
        }
        catch (Exception e)
        {
            ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}