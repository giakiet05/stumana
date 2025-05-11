using System.Collections.ObjectModel;

namespace Stumana.WPF.ViewModels.MainViewModels.ReportOption
{
    class ReportMainViewModel : BaseViewModel
    {
        public ObservableCollection<TabItemViewModel> TabItems { get; set; }

        public ReportMainViewModel()
        {
            InitTabs();
        }

        private void InitTabs()
        {
            TabItems = new ObservableCollection<TabItemViewModel>();

            TabItems.Add(new TabItemViewModel
            {
                Header = "Báo cáo tổng kết học kỳ",
                ContentViewModel = new ReportSemesterViewModel()
            });

            TabItems.Add(new TabItemViewModel
            {
                Header = "Báo cáo tổng kết môn học",
                ContentViewModel = new ReportSubjectViewModel()
            });
        }
    }

    public class TabItemViewModel
    {
        public string Header { get; set; }
        public BaseViewModel ContentViewModel { get; set; }
    }
}