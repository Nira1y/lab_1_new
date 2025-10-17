using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace lab_2_graphic_editor.ViewModel
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        private string _coordinates = "X: 0, Y: 0";
        private string _currentTool = "Кисть";

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
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
