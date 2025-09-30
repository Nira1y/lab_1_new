using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab_1_new.DataService;

namespace lab_1_new.ViewModel
{
    internal class TaskViewModel : INotifyPropertyChanged
    {
        private readonly TaskDataService _taskDataService;

        public event PropertyChangedEventHandler PropertyChanged;

        TaskViewModel()
        {
            _taskDataService = new TaskDataService();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
