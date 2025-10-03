using lab_1_new.View;
using lab_1_new.Models;
using lab_1_new.DataService;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace lab_1_new.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly TaskDataService _taskRepository;
        private string _searchText;
        private ObservableCollection<TaskItem> _allTasks;

        public ICommand IOpenNewWindow => new RelayCommand(OpenNewWindow);
        public ICommand EditTaskCommand => new RelayCommand(EditSelectedTask, CanEditTask);
        public ICommand DeleteTaskCommand => new RelayCommand(DeleteSelectedTask, CanEditTask);
        public ICommand CompleteTaskCommand => new RelayCommand(CompleteSelectedTask, CanEditTask);

        public MainWindowViewModel()
        {
            _taskRepository = new TaskDataService();
            Tasks = new ObservableCollection<TaskItem>();
            _allTasks = new ObservableCollection<TaskItem>();
            LoadTasks();
        }

        private void LoadTasks()
        {
            _allTasks.Clear();
            var tasks = _taskRepository.GetAllTasks();
            foreach (var task in tasks)
            {
                _allTasks.Add(task);
            }
            ApplyFilter();
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplyFilter();
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Tasks = new ObservableCollection<TaskItem>(_allTasks);
            }
            else
            {
                var searchLower = SearchText.ToLower();
                var filteredTasks = _allTasks.Where(task =>
                    (task.Title != null && task.Title.ToLower().Contains(searchLower)) ||
                    (task.Description != null && task.Description.ToLower().Contains(searchLower))||
                    (task.TaskCategory.ToString().ToLower().Contains(searchLower))
                );
                Tasks = new ObservableCollection<TaskItem>(filteredTasks);
            }
            UpdateTaskOrder();
            OnPropertyChanged(nameof(Tasks));
        }

        private void OpenNewWindow()
        {
            var newTaskWindow = new NewTaskWindow();
            if (newTaskWindow.ShowDialog() == true)
            {
                _taskRepository.AddTask(newTaskWindow.NewTask);
                _allTasks.Add(newTaskWindow.NewTask);
                ApplyFilter();
            }
        }

        private void EditSelectedTask()
        {
            if (SelectedTask != null)
            {
                var editWindow = new NewTaskWindow(SelectedTask);
                if (editWindow.ShowDialog() == true)
                {
                    _taskRepository.UpdateTask(editWindow.NewTask);
                    LoadTasks();
                }
            }
        }

        private void DeleteSelectedTask()
        {
            if (SelectedTask != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить задачу \"{SelectedTask.Title}\"?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _taskRepository.DeleteTask(SelectedTask);
                    LoadTasks();
                    SelectedTask = null;
                }
            }
        }

        private void CompleteSelectedTask()
        {
            if (SelectedTask != null)
            {
                SelectedTask.IsCompleted = true;
                SelectedTask.TaskState = TaskState.Завершено;
                var result = MessageBox.Show($"Задача \"{SelectedTask.Title}\" завершена",
                    "завершение", MessageBoxButton.YesNo, MessageBoxImage.Information);
                _taskRepository.UpdateTask(SelectedTask);

                ApplyFilter();
            }
        }

        public void UpdateTaskOrder()
        {
            var sortedTasks = Tasks
                .OrderByDescending(t => !t.IsCompleted) 
                .ThenBy(t => t.DueDate)
                .ToList();

            Tasks.Clear();
            foreach (var task in sortedTasks)
            {
                Tasks.Add(task);
            }
        }

        private bool CanEditTask()
        {
            return SelectedTask != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
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
    }
}