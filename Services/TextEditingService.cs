using lab_2_graphic_editor.Models.Tools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace lab_2_graphic_editor.Services
{
    public class TextEditingService
    {
        private TextTool _textTool;

        public void SetTextTool(TextTool textTool)
        {
            _textTool = textTool;
        }

        public void MakeTextBoxNonEditable(TextBox textBox)
        {
            textBox.IsHitTestVisible = false;
            textBox.Focusable = false;
            textBox.Cursor = Cursors.SizeAll;
        }

        public void StartTextEditing(TextBox textBox)
        {
            _textTool?.StartTextEditing(textBox);
        }

        public void FinishTextEditing(TextBox textBox)
        {
            if (textBox != null)
            {
                MakeTextBoxNonEditable(textBox);
                textBox.Background = Brushes.Transparent;
                textBox.BorderBrush = Brushes.Blue;

                textBox.LostFocus -= TextBox_LostFocus;
                textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            }
        }

        public void SetupTextEditingEvents(TextBox textBox)
        {
            textBox.LostFocus += TextBox_LostFocus;
            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                FinishTextEditing(textBox);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (e.Key == Key.Escape || (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift))
                {
                    FinishTextEditing(textBox);
                    e.Handled = true;
                }
            }
        }

        public void ChangeTextFont(TextBox textBox, string fontFamily, double fontSize, FontWeight fontWeight, FontStyle fontStyle)
        {
            if (textBox != null)
            {
                textBox.FontFamily = new FontFamily(fontFamily);
                textBox.FontSize = fontSize;
                textBox.FontWeight = fontWeight;
                textBox.FontStyle = fontStyle;
            }
        }
    }
}