
using System.Windows.Input;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;
using Stumana.WPF.ViewModels.PopupModels;

namespace Stumana.WPF.ViewModels.MainViewModels.AccountOption
{
    public class AccountOptionViewModel : BaseViewModel 
    {
        public ICommand EditCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _email;

        public string Email
        {
            get => _email;
            set 
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public AccountOptionViewModel()
        {
            ChangePasswordCommand = new NavigateModalCommand(() => new EditPasswordViewModel());
            EditCommand = new NavigateModalCommand(() => new EditUsernameAndEmailViewModel());

            LoadData();
        }

        private void LoadData()
        {
            User user = AccountStore.Instance.CurrentUser;
            if (user != null)
            {
                Username = user.Username;
                Email = user.Email;
            }
        }
    }
}
