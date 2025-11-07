using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using lab_2_graphic_editor.Utilities;
using lab_2_graphic_editor.Models.Texts;
using lab_2_graphic_editor.Models.Tools;

namespace lab_2_graphic_editor.ViewModel
{
    public class TextViewModel : INotifyPropertyChanged
    {
        private string _fontFamily = "Arial";
        private double _fontSize = 12;
        private FontWeight _fontWeight = FontWeights.Normal;
        private FontStyle _fontStyle = FontStyles.Normal;
        private Color _textColor = Colors.Black;

        public string[] AvailableFontFamilies { get; } = new[]
        {
            "Arial", "Times New Roman", "Segoe UI", "Calibri", "Verdana", "Tahoma"
        };

        public double[] AvailableFontSizes { get; } = new[]
        {
            8.0, 9.0, 10.0, 11.0, 12.0, 14.0, 16.0, 18.0, 20.0, 24.0, 28.0, 32.0, 36.0
        };

        public string FontFamily
        {
            get => _fontFamily;
            set
            {
                if (_fontFamily != value)
                {
                    _fontFamily = value;
                    OnPropertyChanged();
                    TextPropertiesChanged?.Invoke();
                }
            }
        }

        public double FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize != value && value >= 1 && value <= 200)
                {
                    _fontSize = value;
                    OnPropertyChanged();
                    TextPropertiesChanged?.Invoke();
                }
            }
        }

        public FontWeight FontWeight
        {
            get => _fontWeight;
            set
            {
                if (_fontWeight != value)
                {
                    _fontWeight = value;
                    OnPropertyChanged();
                    TextPropertiesChanged?.Invoke();
                }
            }
        }

        public FontStyle FontStyle
        {
            get => _fontStyle;
            set
            {
                if (_fontStyle != value)
                {
                    _fontStyle = value;
                    OnPropertyChanged();
                    TextPropertiesChanged?.Invoke();
                }
            }
        }

        public Color TextColor
        {
            get => _textColor;
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    OnPropertyChanged();
                    TextColorChanged?.Invoke(value);
                }
            }
        }

        public ICommand BoldCommand { get; }
        public ICommand ItalicCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event System.Action TextPropertiesChanged;
        public event System.Action<Color> TextColorChanged;

        public TextViewModel()
        {
            BoldCommand = new RelayCommand(
                execute: () => FontWeight = FontWeight == FontWeights.Normal ? FontWeights.Bold : FontWeights.Normal,
                canExecute: () => true);

            ItalicCommand = new RelayCommand(
                execute: () => FontStyle = FontStyle == FontStyles.Normal ? FontStyles.Italic : FontStyles.Normal,
                canExecute: () => true);
        }

        public void ApplyToTextElement(TextElement textElement)
        {
            if (textElement != null)
            {
                textElement.FontFamily = FontFamily;
                textElement.FontSize = FontSize;
                textElement.FontWeight = FontWeight;
                textElement.FontStyle = FontStyle;
            }
        }

        public void ApplyToTextTool(TextTool textTool)
        {
            textTool?.UpdateTextProperties(FontFamily, FontSize, FontWeight, FontStyle);
            textTool?.UpdateTextColor(TextColor);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}