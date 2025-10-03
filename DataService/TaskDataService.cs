using lab_1_new.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Text.Json;

namespace lab_1_new.DataService
{
    public class TaskDataService
    {
        private SQLiteConnection _database;
        private string _databasePath;

        public TaskDataService()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            string databaseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TaskManager");
            Directory.CreateDirectory(databaseFolder);
            _databasePath = Path.Combine(databaseFolder, "tasks.db3");
            _database = new SQLiteConnection(_databasePath);
            _database.CreateTable<TaskItem>();
   
        }

        public List<TaskItem> GetAllTasks()
        {
            var tasks = _database.Table<TaskItem>().ToList();

            foreach (var task in tasks)
            {
                if (!string.IsNullOrEmpty(task.ChecklistJson))
                {
                  
                        var checklistItems = JsonSerializer.Deserialize<List<string>>(task.ChecklistJson);
                        task.ChecklistItems = new ObservableCollection<string>(checklistItems);    
                }
                else
                {
                    task.ChecklistItems = new ObservableCollection<string>();
                }
            }
            return tasks;
        }

        public void AddTask(TaskItem task)
        {
           
                if (task.ChecklistItems != null && task.ChecklistItems.Any())
                {
                    task.ChecklistJson = JsonSerializer.Serialize(task.ChecklistItems.ToList());
                }
                else
                {
                    task.ChecklistJson = null;
                }

                _database.Insert(task);  
        }

        public void UpdateTask(TaskItem task)
        {
                if (task.ChecklistItems != null && task.ChecklistItems.Any())
                {
                    task.ChecklistJson = JsonSerializer.Serialize(task.ChecklistItems.ToList());
                }
                else
                {
                    task.ChecklistJson = null;
                }

                _database.Update(task);
        }


        public void DeleteTask(int taskId)
        {

             _database.Delete<TaskItem>(taskId);
          
        }

        public void DeleteTask(TaskItem task)
        {
            _database.Delete(task);  
        }
    }
}
