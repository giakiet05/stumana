
using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;
using Stumana.WPF.ViewModels.AuthencationViewModels;
using Stumana.WPF.ViewModels.PopupModels;

namespace Stumana.WPF.ViewModels.MainViewModels.AccountOption
{
    public class AccountOptionViewModel : BaseViewModel 
    {
        public ICommand NavigateSignUpCommand { get; set; }
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

        public EventHandler? OnAccountDataChanged { get; set; }

        public AccountOptionViewModel()
        {
            OnAccountDataChanged += UpdateAccountData;
            NavigateSignUpCommand = new NavigateViewCommand(() => new SignUpViewModel());
            ChangePasswordCommand = new NavigateModalCommand(() => new EditPasswordViewModel());
            EditCommand = new NavigateModalCommand(() => new EditUsernameAndEmailViewModel(OnAccountDataChanged));

            LoadData();
        }
        private void UpdateAccountData(object? sender, EventArgs e)
        {
            LoadData();
        }

        private async void LoadData()
        {
            var user = AccountStore.Instance.CurrentUser;
            if (user != null)
            {
                user = await GenericDataService<User>.Instance.GetOneAsync(u => u.Id == user.Id);
                AccountStore.Instance.CurrentUser = user;
                Username = user.Username;
                Email = user.Email;
            }
        }
    }
}
