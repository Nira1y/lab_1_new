using lab_2_graphic_editor.Models;
using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Services;
using lab_2_graphic_editor.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace lab_2_graphic_editor.Models.Texts
{
    public class TextTool : Tool
    {
        private TextBox _currentTextBox;
        private TextElement _textElement;
        private readonly ColorService _colorService;
        private readonly TextViewModel _textViewModel;

        public TextTool(ColorService colorService, TextViewModel textViewModel)
        {
            Name = "Текст";
            _colorService = colorService;
            _textViewModel = textViewModel;

            _textElement = new TextElement(colorService);
            _textViewModel.ApplyToTextElement(_textElement);

            _textElement.FontPropertiesChanged += OnTextElementPropertiesChanged;
            _textElement.TextChanged += OnTextElementTextChanged;
        }

        public override void OnMouseDown(Point position, Canvas canvas)
        {
            if (_currentTextBox != null)
            {
                ExitEditMode();
            }

            _currentTextBox = _textElement.CreateTextBox(position);
            Canvas.SetLeft(_currentTextBox, position.X);
            Canvas.SetTop(_currentTextBox, position.Y);

            MakeTextBoxNonEditable(_currentTextBox);

            canvas.Children.Add(_currentTextBox);
        }

        public override void OnMouseMove(Point position, Canvas canvas)
        {
        }

        public override void OnMouseUp(Point position, Canvas canvas)
        {
        }

        private void MakeTextBoxNonEditable(TextBox textBox)
        {
            if (textBox == null) return;

            textBox.IsHitTestVisible = false;
            textBox.Focusable = false;
            textBox.Cursor = Cursors.Arrow;
            textBox.Background = Brushes.Transparent;
            textBox.BorderThickness = new Thickness(0);
        }

        private void MakeTextBoxEditable(TextBox textBox)
        {
            if (textBox == null) return;

            textBox.IsHitTestVisible = true;
            textBox.Focusable = true;
            textBox.Cursor = Cursors.IBeam;
            textBox.Background = new SolidColorBrush(Color.FromArgb(30, 173, 216, 230));
            textBox.BorderThickness = new Thickness(1);
            textBox.BorderBrush = new SolidColorBrush(Colors.LightBlue);
        }

        private void EnterEditMode(TextBox textBox)
        {
            if (textBox == null) return;

            MakeTextBoxEditable(textBox);

            textBox.Focus();
            textBox.SelectAll();

            textBox.LostFocus += TextBox_LostFocus;
            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (e.Key == Key.Escape || (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift))
                {
                    ExitEditMode();
                    e.Handled = true;
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.LostFocus -= TextBox_LostFocus;
                textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
                ExitEditMode();
            }
        }

        private void ExitEditMode()
        {
            if (_currentTextBox != null)
            {
                _textElement.Text = _currentTextBox.Text;
                MakeTextBoxNonEditable(_currentTextBox);
                _currentTextBox = null;
            }
        }
        public void StartTextEditing(TextBox textBox)
        {
            if (textBox != null)
            {
                _currentTextBox = textBox;
                EnterEditMode(textBox);
            }
        }

        public void UpdateTextProperties(string fontFamily, double fontSize, FontWeight fontWeight, FontStyle fontStyle)
        {
            _textElement.FontFamily = fontFamily;
            _textElement.FontSize = fontSize;
            _textElement.FontWeight = fontWeight;
            _textElement.FontStyle = fontStyle;
        }

        public void UpdateTextColor(Color color)
        {
            _textElement.SetColor(color);
        }

        private void OnTextElementPropertiesChanged()
        {
            if (_currentTextBox != null)
            {
                _textElement.ApplyToTextBox(_currentTextBox);
            }
        }

        private void OnTextElementTextChanged(string newText)
        {
            if (_currentTextBox != null)
            {
                _currentTextBox.Text = newText;
            }
        }
    }
}