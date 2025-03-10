using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Stumana.WPF.Helpers
{
    public static class DataGridSelectedItemsBehavior
    {
        public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "BindableSelectedItems",
                typeof(IList),
                typeof(DataGridSelectedItemsBehavior),
                new PropertyMetadata(null, OnBindableSelectedItemsChanged));

        public static void SetBindableSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(BindableSelectedItemsProperty, value);
        }

        public static IList GetBindableSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(BindableSelectedItemsProperty);
        }

        private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
                dataGrid.SelectionChanged += DataGrid_SelectionChanged;
            }
        }

        private static void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                var bindableSelectedItems = GetBindableSelectedItems(dataGrid);
                if (bindableSelectedItems == null)
                    return;

                bindableSelectedItems.Clear();

                foreach (var item in dataGrid.SelectedItems)
                {
                    bindableSelectedItems.Add(item);
                }
            }
        }
    }

}