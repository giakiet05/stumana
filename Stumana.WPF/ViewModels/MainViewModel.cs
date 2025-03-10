using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Stumana.WPF.Stores;
using Stumana.WPF.Views;

namespace Stumana.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public BaseViewModel CurrentViewModel => NavigationStore.Instance.CurrentViewModel;
        public ObservableCollection<ToastMessageView> Toasts => ToastMessageViewModel.Toasts;
        public BaseViewModel CurrentModelViewModel => ModalNavigationStore.Instance.CurrentModalViewModel;
        public bool IsOpen => ModalNavigationStore.Instance.IsOpen;
        public MainViewModel()
        {
            NavigationStore.Instance.CurrentViewModelChanged += OnCurrentViewModelChanged;
            ModalNavigationStore.Instance.CurrentModalViewModelChanged += OnCurrentModelViewModelChanged;
        }

        private void OnCurrentModelViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentModelViewModel));
            OnPropertyChanged(nameof(IsOpen));
        }

        private void OnCurrentViewModelChanged()
        {
          OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
