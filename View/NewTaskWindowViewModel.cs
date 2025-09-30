using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace lab_1_new.ViewModel
{
    public class NewTaskViewModel : INotifyPropertyChanged
    {
        private string _newTaskText;
        public string NewTaskText
        {
            get => _newTaskText;
            set
            {
                _newTaskText = value;
                OnPropertyChanged(nameof(NewTaskText));
            }
        }

        public ObservableCollection<string> ChecklistItems { get; set; }
        public ICommand AddTaskCommand { get; }
        public ICommand RemoveTaskCommand { get; }

        public NewTaskViewModel()
        {
            ChecklistItems = new ObservableCollection<string>();
            AddTaskCommand = new RelayCommand(AddTask);
            RemoveTaskCommand = new RelayCommand<string>(RemoveTask);
        }

        private void AddTask()
        {
            if (!string.IsNullOrWhiteSpace(NewTaskText))
            {
                ChecklistItems.Add(NewTaskText);
                NewTaskText = string.Empty;
            }
        }
        private void RemoveTask(string task)
        {
            if (task != null)
            {
                ChecklistItems.Remove(task);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}