using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels
{
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

        public EditUsernameAndEmailViewModel()
        {
            CurUser = AccountStore.Instance.CurrentUser;

            CancelCommand = new RelayCommand(() => ModalNavigationStore.Instance.Close());
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
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Email))
            {
                ToastMessageViewModel.ShowErrorToast("Username hoặc email không hợp lệ");
                Username = OldUsername;
                Email = OldEmail;
                return;
            }

            CurUser.Username = Username;
            CurUser.Email = Email;
            await GenericDataService<User>.Instance.UpdateOneAsync(CurUser, u => u.Id == CurUser.Id);
            ToastMessageViewModel.ShowSuccessToast("Thay đỏi username và email thành công");
            AccountStore.Instance.CurrentUser = CurUser;
            ModalNavigationStore.Instance.Close();
        }
    }
}
