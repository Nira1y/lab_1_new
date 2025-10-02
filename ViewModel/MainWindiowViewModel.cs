using lab_1_new.View;
using lab_1_new.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace lab_1_new.ViewModel
{
    public class MainWindiowViewModel : INotifyPropertyChanged
    {
        public ICommand IOpenNewWindow => new RelayCommand(OpenNewWindow);
        public ICommand EditTaskCommand => new RelayCommand(EditSelectedTask, CanEditTask);
        public ICommand DeleteTaskCommand => new RelayCommand(DeleteSelectedTask, CanEditTask);

        private void OpenNewWindow()
        {
            var newTaskWindow = new NewTaskWindow();
            if (newTaskWindow.ShowDialog() == true)
            {
                Tasks.Add(newTaskWindow.NewTask);
            }
        }

        private void EditSelectedTask()
        {
            if (SelectedTask != null)
            {
                var editWindow = new NewTaskWindow(SelectedTask);
                if (editWindow.ShowDialog() == true)
                {
                    int index = Tasks.IndexOf(SelectedTask);
                    if (index != -1)
                    {
                        Tasks[index] = editWindow.NewTask;
                        SelectedTask = editWindow.NewTask;
                    }
                }
            }
        }

        private void DeleteSelectedTask()
        {
            if (SelectedTask != null)
            {
                Tasks.Remove(SelectedTask);
                SelectedTask = null;
            }
        }

        private bool CanEditTask()
        {
            return SelectedTask != null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<TaskItem> _tasks;
        public ObservableCollection<TaskItem> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }

        private TaskItem _selectedTask;
        public TaskItem SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
                OnPropertyChanged(nameof(IsTaskSelected));
            }
        }

        public bool IsTaskSelected => SelectedTask != null;

        public MainWindiowViewModel()
        {
            Tasks = new ObservableCollection<TaskItem>();
        }
    }
}