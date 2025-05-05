using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels
{
    public class EditPasswordViewModel : BaseViewModel
    {
        public ICommand ConfirmCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private string _oldPassword = string.Empty;

        public string OldPassword
        {
            get => _oldPassword;
            set
            {
                _oldPassword = value;
                OnPropertyChanged();
            }
        }

        private string _errorOldPassword;

        public string ErrorOldPassword
        {
            get => _errorOldPassword;
            set
            {
                _errorOldPassword = value;
                OnPropertyChanged();
            }
        }

        private string _newPassword = string.Empty;

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged();
            }
        }

        private string _errorNewPassword;

        public string ErrorNewPassword
        {
            get => _errorNewPassword;
            set
            {
                _errorNewPassword = value;
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

        private string _errorConfirmPassword;

        public string ErrorConfirmPassword
        {
            get => _errorConfirmPassword;
            set
            {
                _errorConfirmPassword = value;
                OnPropertyChanged();
            }
        }

        public EditPasswordViewModel()
        {
            CancelCommand = new RelayCommand(() => ModalNavigationStore.Instance.Close());
            ConfirmCommand = new RelayCommand(ChangePassword);
        }

        private async void ChangePassword()
        {
            try
            {
                bool haveError = false;
                User user = AccountStore.Instance.CurrentUser;

                if (user.VerifyPassword(OldPassword))
                {
                    ErrorOldPassword = "Sai mật khẩu cũ";
                    haveError = true;
                }

                if (string.IsNullOrEmpty(NewPassword))
                {
                    ErrorNewPassword = "Mật khẩu mới không hợp lệ";
                    haveError = true;
                }
                else if (OldPassword == NewPassword)
                {
                    ErrorNewPassword = "Mật khẩu mới không thể trùng với mật khẩu cũ";
                    haveError = true;
                }

                if (NewPassword != ConfirmPassword)
                {
                    ErrorConfirmPassword = "Xác nhận lại mật khẩu không đúng";
                    haveError = true;
                }

                if (!haveError)
                    throw new Exception();

                user.ChangePassword(NewPassword);
                await GenericDataService<User>.Instance.UpdateOneAsync(user, u => u.Id == user.Id);
                ToastMessageViewModel.ShowSuccessToast("Thay đổi mật khẩu thành công");
                ModalNavigationStore.Instance.Close();
            }
            catch (Exception e)
            {
                ToastMessageViewModel.ShowErrorToast("Thay đổi mật khẩu không thành công");
            }
        }
    }
}