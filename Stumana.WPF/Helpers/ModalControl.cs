using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Stumana.WPF.Helpers
{
    public class ModalControl : ContentControl
    {
        public static readonly DependencyProperty IsOpenProperty = 
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(ModalControl), new PropertyMetadata((object)false));
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(ModalControl), new PropertyMetadata(new CornerRadius()));
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        static ModalControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModalControl), new FrameworkPropertyMetadata(typeof(ModalControl)));
            BackgroundProperty.OverrideMetadata(typeof(ModalControl), new FrameworkPropertyMetadata(CreateDefaultBackground()));
        }

        private static object CreateDefaultBackground()
        {
            SolidColorBrush defaultBackground = new SolidColorBrush(Colors.Black)
            {
                Opacity = 0.5
            };
            return defaultBackground;
        }
    }
}
