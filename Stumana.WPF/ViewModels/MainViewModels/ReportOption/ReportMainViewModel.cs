using System.Collections.ObjectModel;

namespace Stumana.WPF.ViewModels.MainViewModels.ReportOption
{
    class ReportMainViewModel : BaseViewModel
    {
        public ReportSemesterViewModel ReportSemesterViewModel { get; set; }
        public ReportSubjectViewModel ReportSubjectViewModel { get; set; }

        public ReportMainViewModel()
        {
            ReportSemesterViewModel = new ReportSemesterViewModel();
            ReportSubjectViewModel = new ReportSubjectViewModel();
        }
    }

    public class TabItemViewModel
    {
        public string Header { get; set; }
        public BaseViewModel ContentViewModel { get; set; }
    }
}