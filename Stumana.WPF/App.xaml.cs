using Stumana.WPF.Stores;
using System.Windows;
using Stumana.WPF.ViewModels;
using Stumana.WPF.ViewModels.AuthencationViewModels;
using Stumana.DataAcess.Data;
using Microsoft.EntityFrameworkCore;
using Stumana.WPF.ViewModels.MainViewModels;

namespace Stumana.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            using (var context = AppDbContextFactory.Instance.CreateDbContext())
            {
                context.Database.Migrate();
                NavigationStore.Instance.CurrentViewModel = new SignInViewModel();
                
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
}
