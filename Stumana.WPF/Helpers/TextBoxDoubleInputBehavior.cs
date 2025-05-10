using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace Stumana.WPF.Helpers
{
    public static class TextBoxDoubleInputBehavior
    {
        public static readonly DependencyProperty IsNumericProperty =
            DependencyProperty.RegisterAttached(
                "IsNumeric",
                typeof(bool),
                typeof(TextBoxDoubleInputBehavior),
                new UIPropertyMetadata(false, OnIsNumericChanged));

        public static bool GetIsNumeric(DependencyObject obj) => (bool)obj.GetValue(IsNumericProperty);

        public static void SetIsNumeric(DependencyObject obj, bool value) => obj.SetValue(IsNumericProperty, value);

        private static void OnIsNumericChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    DataObject.AddPastingHandler(textBox, OnPaste);
                }
                else
                {
                    textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                    DataObject.RemovePastingHandler(textBox, OnPaste);
                }
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string fullText = GetProposedText(textBox, e.Text);

                // Only allow digits and a single dot
                e.Handled = !Regex.IsMatch(fullText, @"^\d*\.?\d*$");
            }
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (e.DataObject.GetDataPresent(DataFormats.Text))
                {
                    string pastedText = (string)e.DataObject.GetData(DataFormats.Text);
                    string fullText = GetProposedText(textBox, pastedText);
                    if (!Regex.IsMatch(fullText, @"^\d*\.?\d*$"))
                    {
                        e.CancelCommand();
                    }
                }
            }
        }

        private static string GetProposedText(TextBox textBox, string newText)
        {
            string currentText = textBox.Text;
            var selectionStart = textBox.SelectionStart;
            var selectionLength = textBox.SelectionLength;

            return currentText.Remove(selectionStart, selectionLength).Insert(selectionStart, newText);
        }
    }
}