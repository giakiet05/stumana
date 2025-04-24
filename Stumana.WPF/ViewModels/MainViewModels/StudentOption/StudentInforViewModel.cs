using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.WPF.ViewModels.MainViewModels.StudentOption
{
    public class StudentInforViewModel : BaseViewModel
    {
        public string _authorizedleaveText;
        public string authorizedleaveText
        {
            get => _authorizedleaveText;
            set
            {
                _authorizedleaveText = value;
                OnPropertyChanged(nameof(authorizedleaveText));
            }
        }
        public string _unauthorizedleaveText;
        public string unauthorizedleaveText
        {
            get => _unauthorizedleaveText;
            set
            {
                _unauthorizedleaveText = value;
                OnPropertyChanged(nameof(unauthorizedleaveText));
            }
        }
        public string _performanceText;
        public string performanceText
        {
            get => _performanceText;
            set
            {
                _performanceText = value;
                OnPropertyChanged(nameof(performanceText));
            }
        }
        public string _conductText;
        public string conductText
        {
            get => _conductText;
            set
            {
                _conductText = value;
                OnPropertyChanged(nameof(conductText));
            }
        }
        public string _commentText;
        public string commentText
        {
            get => _commentText;
            set
            {
                _commentText = value;
                OnPropertyChanged(nameof(commentText));
            }
        }
        public string _semesterText;
        public string semesterText
        {
            get => _semesterText;
            set
            {
                _semesterText = value;
                OnPropertyChanged(nameof(semesterText));
            }
        }
     
    }
}
