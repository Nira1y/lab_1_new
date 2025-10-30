using System;
using System.Windows.Media;

namespace lab_2_graphic_editor.Services
{
    public class ColorService
    {
        private Brush _currentColor = Brushes.Black;

        public Brush CurrentColor
        {
            get => _currentColor;
            set
            {
                _currentColor = value;
                ColorChanged?.Invoke(value);
            }
        }

        public event Action<Brush> ColorChanged;

        public void SetColor(Color color)
        {
            CurrentColor = new SolidColorBrush(color);
        }

        public Color GetColor()
        {
            return (CurrentColor as SolidColorBrush)?.Color ?? Colors.Black;
        }
    }
}