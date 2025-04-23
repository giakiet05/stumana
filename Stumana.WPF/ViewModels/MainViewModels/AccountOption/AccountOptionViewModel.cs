
using System.Windows.Input;

namespace Stumana.WPF.ViewModels.MainViewModels.AccountOption
{
    public class AccountOptionViewModel : BaseViewModel 
    {
        public ICommand editCommand { get; }
        public ICommand changePasswordCommand { get; }
        public AccountOptionViewModel()
        {
            
        }
        // Example properties
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
        private string _id;

        public string Id
        {
            get =>_id; 
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
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

        private string _password;

        public string Password
        {
            get => _password;
            set 
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
    }
}
