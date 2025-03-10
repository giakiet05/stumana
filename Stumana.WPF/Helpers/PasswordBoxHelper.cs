using System.Windows;
using System.Windows.Controls;

namespace Stumana.WPF.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject dp) => (string)dp.GetValue(BoundPasswordProperty);
        public static void SetBoundPassword(DependencyObject dp, string value) => dp.SetValue(BoundPasswordProperty, value);

        private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox && !passwordBox.Password.Equals(e.NewValue))
            {
                passwordBox.Password = (string)e.NewValue;
            }
        }

        public static readonly DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, OnBindPasswordChanged));

        public static bool GetBindPassword(DependencyObject dp) => (bool)dp.GetValue(BindPasswordProperty);
        public static void SetBindPassword(DependencyObject dp, bool value) => dp.SetValue(BindPasswordProperty, value);

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox)
            {
                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
                else
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetBoundPassword(passwordBox, passwordBox.Password);
            }
        }
    }
}
