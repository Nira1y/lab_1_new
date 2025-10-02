using lab_1_new.Models;
using System;
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

        private string _taskTitle;
        public string TaskTitle
        {
            get => _taskTitle;
            set
            {
                _taskTitle = value;
                OnPropertyChanged(nameof(TaskTitle));
            }
        }

        private string _taskDescription;
        public string TaskDescription
        {
            get => _taskDescription;
            set
            {
                _taskDescription = value;
                OnPropertyChanged(nameof(TaskDescription));
            }
        }

        private TaskCategory _selectedCategory;
        public TaskCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        private DateTime _dueDate = DateTime.Now;
        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged(nameof(DueDate));
            }
        }

        private DateTime _dueTime = DateTime.Now;
        public DateTime DueTime
        {
            get => _dueTime;
            set
            {
                _dueTime = value;
                OnPropertyChanged(nameof(DueTime));
            }
        }

        public ObservableCollection<TaskCategory> Categories { get; set; }
        public ObservableCollection<string> ChecklistItems { get; set; }

        public ICommand AddTaskCommand { get; private set; }
        public ICommand RemoveTaskCommand { get; private set; }


        public NewTaskViewModel()
        {
            InitializeCollections();
        }


        public NewTaskViewModel(TaskItem taskToEdit) : this()
        {
            TaskTitle = taskToEdit.Title;
            TaskDescription = taskToEdit.Description;
            SelectedCategory = taskToEdit.TaskCategory;
            DueDate = taskToEdit.DueDate;
            DueTime = taskToEdit.DueDate;
            foreach (var item in taskToEdit.ChecklistItems)
            {
                ChecklistItems.Add(item);
            }
        }

        private void InitializeCollections()
        {
            ChecklistItems = new ObservableCollection<string>();
            Categories = new ObservableCollection<TaskCategory>
            {
                TaskCategory.Работа,
                TaskCategory.Дом,
                TaskCategory.Личное,
                TaskCategory.Финансы,
                TaskCategory.Здоровье,
                TaskCategory.Другое
            };
            SelectedCategory = Categories[0];

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

        public TaskItem CreateTaskItem()
        {
            return new TaskItem
            {
                Title = TaskTitle,
                Description = TaskDescription,
                TaskCategory = SelectedCategory,
                DueDate = new DateTime(DueDate.Year, DueDate.Month, DueDate.Day, DueTime.Hour, DueTime.Minute, 0),
                StartTime = DateTime.Now,
                IsCompleted = false,
                Timer = TimeSpan.Zero,
                TaskState = TaskState.NotStarted,
                ChecklistItems = new ObservableCollection<string>(ChecklistItems)
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}