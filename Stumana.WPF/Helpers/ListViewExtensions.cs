using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Stumana.WPF.Helpers
{
    public static class ListViewExtensions
    {
        public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "BindableSelectedItems",
                typeof(IList),
                typeof(ListViewExtensions),
                new PropertyMetadata(null, OnBindableSelectedItemsChanged));

        public static IList GetBindableSelectedItems(DependencyObject obj) =>
            (IList)obj.GetValue(BindableSelectedItemsProperty);

        public static void SetBindableSelectedItems(DependencyObject obj, IList value) =>
            obj.SetValue(BindableSelectedItemsProperty, value);

        private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView listView)
            {
                listView.SelectionChanged -= ListView_SelectionChanged;
                if (e.NewValue is IList boundList)
                {
                    listView.SelectionChanged += ListView_SelectionChanged;
                    SynchronizeSelectedItems(listView, boundList);
                }
            }
        }

        private static void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView && GetBindableSelectedItems(listView) is IList boundList)
            {
                foreach (var item in e.RemovedItems)
                {
                    boundList.Remove(item);
                }

                foreach (var item in e.AddedItems)
                {
                    boundList.Add(item);
                }
            }
        }

        private static void SynchronizeSelectedItems(ListView listView, IList boundList)
        {
            boundList.Clear();
            foreach (var item in listView.SelectedItems)
            {
                boundList.Add(item);
            }
        }
    }
}
