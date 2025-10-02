using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab_1_new.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartTime { get; set; }
        public bool IsCompleted { get; set; }
        public TimeSpan Timer { get; set; }
        public ObservableCollection<string> ChecklistItems { get; set; }

        public TaskState TaskState { get; set; }
        public TaskCategory TaskCategory { get; set; }

        public static implicit operator Task(TaskItem v)
        {
            throw new NotImplementedException();
        }
    }

    public enum TaskState
    {
        InProgress,
        Completed,
        NotStarted,
        Late,
        Archived,
        Deleted
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
