using System.Text.RegularExpressions;
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels;

class EditUsernameAndEmailViewModel : BaseViewModel
{
    public ICommand ConfirmCommand { get; set; }
    public ICommand CancelCommand { get; set; }

    private User CurUser { get; set; }

    private string _username;
    private string OldUsername { get; set; }
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
        }
    }

    private bool _isUsernameInvalid;

    public bool IsUsernameInvalid
    {
        get => _isUsernameInvalid;
        set
        {
            _isUsernameInvalid = value;
            OnPropertyChanged();
        }
    }

    private string _email;
    private string OldEmail { get; set; }
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }

    private string _errorEmailText;

    public string ErrorEmailText
    {
        get => _errorEmailText;
        set
        {
            _errorEmailText = value;
            OnPropertyChanged();
        }
    }

    private EventHandler? _onChangeAccountData;

    public EditUsernameAndEmailViewModel(EventHandler? onAccountDataChanged)
    {
        _onChangeAccountData = onAccountDataChanged;
        CurUser = AccountStore.Instance.CurrentUser;

        CancelCommand = new RelayCommand(ModalNavigationStore.Instance.Close);
        ConfirmCommand = new RelayCommand(ChangeUsernameOrEmail);

        Load();
    }

    private void Load()
    {
        Username = CurUser.Username;
        Email = CurUser.Email;

        OldUsername = Username;
        OldEmail = Email;
    }

    private async void ChangeUsernameOrEmail()
    {
        CheckError();
        if (IsUsernameInvalid || ErrorEmailText != "")
            return;

        CurUser.Username = Username;
        CurUser.Email = Email;
        await GenericDataService<User>.Instance.UpdateOneAsync(CurUser, u => u.Id == CurUser.Id);
        ToastMessageViewModel.ShowSuccessToast("Thay đổi username và email thành công");
        _onChangeAccountData?.Invoke(this, EventArgs.Empty);
        ModalNavigationStore.Instance.Close();
    }

    private async void CheckError()
    {
        IsUsernameInvalid = false;
        ErrorEmailText = "";

        if (string.IsNullOrEmpty(Username))
            IsUsernameInvalid = true;

        if (string.IsNullOrEmpty(Email))
        {
            ErrorEmailText = "⚠ Email không hợp lệ";
            return;
        }
        else
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            Regex regex = new Regex(pattern);
            if (!regex.IsMatch(Email))
            {
                ErrorEmailText = "⚠ Email không hợp lệ";
                return;
            }

            User? user = await GenericDataService<User>.Instance.GetOneAsync(u => u.Email == Email);
            if (user != null && user.Id != CurUser.Id)
            {
                ErrorEmailText = "⚠ Email đã được sử dụng";
                return;
            }
        }
    }
}