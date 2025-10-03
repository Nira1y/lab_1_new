using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace lab_1_new.Models
{
    public class TaskItem : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        public TaskState _taskState;
        public TaskCategory TaskCategory { get; set; }

        [Ignore]
        public ObservableCollection<string> ChecklistItems { get; set; }

        public string ChecklistJson { get; set; }

        public static implicit operator Task(TaskItem v)

        {
            throw new NotImplementedException();
        }
        public TaskState TaskState
        {
            get => _taskState;
            set
            {
                _taskState = value;
                OnPropertyChanged(nameof(TaskState));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum TaskState
    {
        Выполняется,
        Завершено
    }

    public enum TaskCategory
    {
        Работа,
        Дом,
        Личное,
        Финансы,
        Здоровье,
        Другое
    }

}
