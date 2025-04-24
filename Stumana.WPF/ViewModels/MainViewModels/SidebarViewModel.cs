using Stumana.WPF.Stores;
using Stumana.WPF.ViewModels.MainViewModels.AccountOption;
using Stumana.WPF.ViewModels.MainViewModels.ClassOption;
using System.Windows.Input;
using Stumana.WPF.Commands;
using Stumana.WPF.ViewModels.MainViewModels.ReportOption;
using Stumana.WPF.ViewModels.MainViewModels.ScoreOption;
using Stumana.WPF.ViewModels.MainViewModels.SettleOption;
using Stumana.WPF.ViewModels.MainViewModels.StudentOption;
using Stumana.WPF.ViewModels.MainViewModels.SubjectOption;

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

        #endregion Commands

        public SidebarViewModel()
        {
            NavigationStore.Instance.CurrentLayoutModelChanged += OnCurrentLayoutModelChanged;
            NavigationStore.Instance.CurrentLayoutModel = new ClassListViewModel();

            AccountNavigateCommand = new NavigateLayoutCommand(() => new AccountOptionViewModel());
            SubjectNavigateCommand = new NavigateLayoutCommand(() => new ObjectViewModel());
            StudentNavigateCommand = new NavigateLayoutCommand(() => new StudentViewModel());
            SemesterNavigateCommand = new NavigateLayoutCommand(() => new SemesterViewModel());
            ClassNavigateCommand = new NavigateLayoutCommand(() => new ClassListViewModel());
            ScoreNavigateCommand = new NavigateLayoutCommand(() => new ScoreSubjectViewModel());
            ReportNavigateCommand = new NavigateLayoutCommand(() => new ReportSubjectViewModel());
        }

        public void OnCurrentLayoutModelChanged()
        {
            OnPropertyChanged(nameof(CurrentLayoutVM));
        }
    }
}