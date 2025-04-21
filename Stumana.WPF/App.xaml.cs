using Stumana.WPF.Stores;
using System.Windows;
using Stumana.WPF.ViewModels;
using Stumana.WPF.ViewModels.AuthencationViewModels;

namespace Stumana.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            NavigationStore.Instance.CurrentViewModel = new SignInViewModel();
            NavigationStore.Instance.CurrentLayoutModel = null;
            ModalNavigationStore.Instance.CurrentModalViewModel = null;

            MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
