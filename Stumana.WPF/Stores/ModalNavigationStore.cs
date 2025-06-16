using Stumana.WPF.ViewModels;

namespace Stumana.WPF.Stores
{
    public class ModalNavigationStore
    {
        private static ModalNavigationStore _instance;

        ModalNavigationStore()
        {
            _instance = this;
        }

        public static ModalNavigationStore Instance
        {
            get
            {
                if (_instance == null) _instance = new ModalNavigationStore();
                return _instance;
            }
        }


        public event Action CurrentModalViewModelChanged;
        private BaseViewModel? _currentModalViewModel;

        public BaseViewModel? CurrentModalViewModel
        {
            get => _currentModalViewModel;
            set
            {
                _currentModalViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        public bool IsOpen => CurrentModalViewModel != null;

        private void OnCurrentViewModelChanged()
        {
            CurrentModalViewModelChanged?.Invoke();
        }

        public void NavigateCurrentModelViewModel<TViewModel>(Func<TViewModel> createModalViewModel) where TViewModel : BaseViewModel
        {
            CurrentModalViewModel = createModalViewModel();
        }

        public void Close()
        {
            CurrentModalViewModel = null;

        }
    }
}