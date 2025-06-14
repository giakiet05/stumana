using Stumana.WPF.Stores;
using Stumana.WPF.ViewModels.MainViewModels.AccountOption;
using Stumana.WPF.ViewModels.MainViewModels.ClassOption;
using System.Windows.Input;
using Stumana.WPF.Commands;
using Stumana.WPF.ViewModels.MainViewModels.ReportOption;
using Stumana.WPF.ViewModels.MainViewModels.ScoreOption;
using Stumana.WPF.ViewModels.MainViewModels.StudentOption;
using Stumana.WPF.ViewModels.MainViewModels.SubjectOption;
using Stumana.WPF.ViewModels.MainViewModels.YearOption;

namespace Stumana.WPF.ViewModels.MainViewModels
{
    public class SidebarViewModel : BaseViewModel
    {
        public BaseViewModel CurrentLayoutVM => NavigationStore.Instance.CurrentLayoutModel;

        #region Commands

        public ICommand AccountNavigateCommand { get; set; }
        public ICommand SubjectNavigateCommand { get; set; }
        public ICommand StudentNavigateCommand { get; set; }
        public ICommand SemesterNavigateCommand { get; set; }
        public ICommand ClassNavigateCommand { get; set; }
        public ICommand ScoreNavigateCommand { get; set; }
        public ICommand ReportNavigateCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        #endregion Commands

        public SidebarViewModel()
        {
            NavigationStore.Instance.CurrentLayoutModelChanged += OnCurrentLayoutModelChanged;
            NavigationStore.Instance.CurrentLayoutModel = new AccountOptionViewModel();

            AccountNavigateCommand = new NavigateLayoutCommand(() => new AccountOptionViewModel());
            SubjectNavigateCommand = new NavigateLayoutCommand(() => new SubjectViewModel());
            StudentNavigateCommand = new NavigateLayoutCommand(() => new StudentViewModel());
            SemesterNavigateCommand = new NavigateLayoutCommand(() => new YearViewModel());
            ClassNavigateCommand = new NavigateLayoutCommand(() => new ClassListViewModel());
            ScoreNavigateCommand = new NavigateLayoutCommand(() => new ScoreSubjectViewModel());
            ReportNavigateCommand = new NavigateLayoutCommand(() => new ReportMainViewModel());
            LogoutCommand = new RelayCommand(() =>
            {
                //AccountStore.Instance.CurrentUser = null;
                //NavigationStore.Instance.CurrentViewModel = new SignInViewModel();
                //ToastMessageViewModel.ShowSuccessToast("Đăng xuất thành công");
                ModalNavigationStore.Instance.CurrentModalViewModel = new LogoutConfirmViewModel();
            });
        }
        

        public void OnCurrentLayoutModelChanged()
        {
            OnPropertyChanged(nameof(CurrentLayoutVM));
        }
    }
}