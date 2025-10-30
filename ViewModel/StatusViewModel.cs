using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace lab_2_graphic_editor.ViewModel
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        private string _coordinates = "X: 0, Y: 0";
        private string _currentTool = "Кисть";
        private Color _currentColor = Colors.Black;
        public string Coordinates
        {
            get => _coordinates;
            set { _coordinates = value; OnPropertyChanged(); }
        }

        public string CurrentTool
        {
            get => _currentTool;
            set { _currentTool = value; OnPropertyChanged(); }
        }

        public Color CurrentColor
        {
            get => _currentColor;
            set
            {
                _currentColor = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
