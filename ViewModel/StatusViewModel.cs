using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace lab_2_graphic_editor.ViewModel
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        private string _coordinates = "X: -, Y: -";
        private string _currentTool = "Инструмент: Кисть";
        private Color _currentColor = Colors.Black;

        public string Coordinates
        {
            get => _coordinates;
            set
            {
                _coordinates = value;
                OnPropertyChanged();
            }
        }

        public string CurrentTool
        {
            get => _currentTool;
            set
            {
                _currentTool = value;
                OnPropertyChanged();
            }
        }

        public Color CurrentColor
        {
            get => _currentColor;
            set
            {
                _currentColor = value;
                OnPropertyChanged();
                ColorChanged?.Invoke(value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event System.Action<Color> ColorChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}