using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab_1_new.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartTime { get; set; }
        public bool IsCompleted { get; set; }
        public TimeSpan Timer { get; set; }

        public TaskState TaskState { get; set; }
        public TaskCategory TaskCategory { get; set; }
        public TaskImportance TaskImportance { get; set; }
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
        Work,
        Home,
        Personal,
        Finance,
        Health,
        None
    }

    public enum TaskImportance
    {
        Low,
        Medium,
        High,
        Critical
    }
}
