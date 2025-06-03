using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Stumana.WPF.Helpers;

public static class DataGridRowClickBehavior
{
    public static readonly DependencyProperty RowClickCommandProperty =
        DependencyProperty.RegisterAttached(
            "RowClickCommand",
            typeof(ICommand),
            typeof(DataGridRowClickBehavior),
            new PropertyMetadata(null, OnRowClickCommandChanged));

    public static void SetRowClickCommand(DependencyObject element, ICommand value)
    {
        element.SetValue(RowClickCommandProperty, value);
    }

    public static ICommand GetRowClickCommand(DependencyObject element)
    {
        return (ICommand)element.GetValue(RowClickCommandProperty);
    }

    private static void OnRowClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DataGrid dataGrid)
        {
            dataGrid.PreviewMouseLeftButtonUp -= DataGrid_PreviewMouseLeftButtonUp;
            dataGrid.PreviewMouseLeftButtonUp += DataGrid_PreviewMouseLeftButtonUp;
        }
    }

    private static void DataGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGrid dataGrid)
        {
            var point = e.GetPosition(dataGrid);
            var element = dataGrid.InputHitTest(point) as DependencyObject;

            while (element != null && !(element is DataGridRow))
                element = VisualTreeHelper.GetParent(element);

            if (element is DataGridRow row && row.Item != null)
            {
                var command = GetRowClickCommand(dataGrid);
                if (command != null && command.CanExecute(row.Item))
                    command.Execute(row.Item);
            }
        }
    }
}