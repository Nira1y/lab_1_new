using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Models.Texts
{
    public class TextElement
    {
        protected readonly ColorService _colorService;

        private string _text = "Текст";
        private string _fontFamily = "Arial";
        private double _fontSize = 12;
        private FontWeight _fontWeight = FontWeights.Normal;
        private FontStyle _fontStyle = FontStyles.Normal;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                TextChanged?.Invoke(value);
            }
        }

        public string FontFamily
        {
            get => _fontFamily;
            set
            {
                _fontFamily = value;
                FontPropertiesChanged?.Invoke();
            }
        }

        public double FontSize
        {
            get => _fontSize;
            set
            {
                if (value > 0 && value <= 200)
                {
                    _fontSize = value;
                    FontPropertiesChanged?.Invoke();
                }
            }
        }

        public FontWeight FontWeight
        {
            get => _fontWeight;
            set
            {
                _fontWeight = value;
                FontPropertiesChanged?.Invoke();
            }
        }

        public FontStyle FontStyle
        {
            get => _fontStyle;
            set
            {
                _fontStyle = value;
                FontPropertiesChanged?.Invoke();
            }
        }

        public Brush Foreground => _colorService.CurrentColor;

        public event System.Action<string> TextChanged;
        public event System.Action FontPropertiesChanged;

        public TextElement(ColorService colorService)
        {
            _colorService = colorService;
        }

        public TextBox CreateTextBox(Point position)
        {
            return new TextBox
            {
                Text = Text,
                Foreground = Foreground,
                FontFamily = new FontFamily(FontFamily),
                FontSize = FontSize,
                FontWeight = FontWeight,
                FontStyle = FontStyle,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(2),
                MinWidth = 50,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
        }

        public virtual void UpdateTextBox(TextBox textBox)
        {
            textBox.Text = Text;
            textBox.FontFamily = new FontFamily(FontFamily);
            textBox.FontSize = FontSize;
            textBox.FontWeight = FontWeight;
            textBox.FontStyle = FontStyle;
            textBox.Foreground = Foreground;
        }

        public virtual void SetColor(Color color)
        {
            _colorService.SetColor(color);
        }

        public void ApplyToTextBox(TextBox textBox)
        {
            if (textBox != null)
            {
                UpdateTextBox(textBox);
            }
        }
    }
}