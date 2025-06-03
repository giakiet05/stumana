using Stumana.WPF.Commands;
using Stumana.WPF.Stores;
using Stumana.WPF.ViewModels.AuthencationViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stumana.WPF.ViewModels
{
    public class LogoutConfirmViewModel : BaseViewModel
    {
        public RelayCommand LogoutCommand { get; }
        public ICommand CancelCommand { get; }

        // Constructor that accepts an Action for DeleteCommand and keeps CancelCommand as is
        public LogoutConfirmViewModel()
        {
            // Create the RelayCommand for the DeleteCommand with the given Action
            LogoutCommand = new RelayCommand(LogOut);
            CancelCommand = new CancelCommand();
        }
        public void LogOut()
        {
            NavigationStore.Instance.CurrentViewModel = new SignInViewModel();
            ModalNavigationStore.Instance.CurrentModalViewModel = null;
            AccountStore.Instance.CurrentUser = null;
            ToastMessageViewModel.ShowSuccessToast("Đăng xuất thành công");
        }
    }
}
