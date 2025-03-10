using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stumana.WPF.ViewModels;

namespace Stumana.WPF.Stores
{
    public class NavigationStore
    {
        private static NavigationStore _instance;
        NavigationStore() { _instance = this; }

        public static NavigationStore Instance
        {
            get { if (_instance == null) _instance = new NavigationStore(); return _instance; }
        }
        public event Action CurrentViewModelChanged;
        public event Action CurrentLayoutModelChanged;

        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }
        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
        private BaseViewModel _currentLayoutModel;
        public BaseViewModel CurrentLayoutModel
        {
            get => _currentLayoutModel;
            set
            {
                _currentLayoutModel = value;
                OnCurrentLayoutModelChanged();
            }
        }
        private void OnCurrentLayoutModelChanged()
        {
            CurrentLayoutModelChanged?.Invoke();
        }
        public void NavigateCurrentViewModel<TViewModel> (Func<TViewModel> createViewModel) where TViewModel : BaseViewModel
        {
            CurrentViewModel = createViewModel();
        }
        public void NavigateCurrentLayoutModel<TViewModel>(Func<TViewModel> createViewModel) where TViewModel : BaseViewModel
        {
            CurrentLayoutModel = createViewModel();
        }
    }
}
