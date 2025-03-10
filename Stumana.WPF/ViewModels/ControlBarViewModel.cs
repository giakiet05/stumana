
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Stumana.WPF.Commands;

namespace Stumana.WPF.ViewModels
{
    public class ControlBarViewModel : BaseViewModel
    {
        public ICommand CloseCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand MouseMoveCommand { get; }
        public ControlBarViewModel()
        {
            CloseCommand = new RelayCommand(ExecuteClose);
            MinimizeCommand = new RelayCommand(ExecuteMinimize);
            MaximizeCommand = new RelayCommand(ExecuteMaximize);
            MouseMoveCommand = new RelayCommand(ExecuteMouseMove);
        }

        private void ExecuteMaximize(object parameter)
        {
            FrameworkElement parent = GetParent(parameter as UserControl);
            var w = parent as Window;
            if (w != null)
                w.WindowState = w.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void ExecuteClose(object parameter)
        {
            FrameworkElement parent = GetParent(parameter as UserControl);
            var w = parent as Window;
            if (w != null)
                w.Close();
        }

        private void ExecuteMinimize(object parameter)
        {
            FrameworkElement parent = GetParent(parameter as UserControl);
            var w = parent as Window;
            if (w != null)
                w.WindowState = WindowState.Minimized;
        }

        private void ExecuteMouseMove(object parameter)
        {
            FrameworkElement parent = GetParent(parameter as UserControl);
            var w = parent as Window;
            if (w != null)
                w.DragMove();
        }

        FrameworkElement GetParent(UserControl p)
        {
            FrameworkElement parent = p;
            while (parent.Parent != null)
            {
                parent = parent.Parent as FrameworkElement;
            }
            return parent;
        }
    }
}
