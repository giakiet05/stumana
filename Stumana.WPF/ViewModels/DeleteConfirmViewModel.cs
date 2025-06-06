
using Stumana.WPF.Commands;
using System;
using System.Windows.Input;

namespace Stumana.WPF.ViewModels
{
    public class DeleteConfirmViewModel : BaseViewModel
    {
        public RelayCommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }
        public string Message { get; set; } = "Bạn có chắc chắn muốn xóa không? Điều này không thể hoàn tác";

        // Constructor that accepts an Action for DeleteCommand and keeps CancelCommand as is
        public DeleteConfirmViewModel(Action deleteAction, string m ="")
        {
            // Create the RelayCommand for the DeleteCommand with the given Action
            DeleteCommand = new RelayCommand(deleteAction);
            CancelCommand = new CancelCommand();
            if (!string.IsNullOrEmpty(m))
            {
                Message = m;
            }
        }
    }
}
