using lab_1_new.ViewModel;
using lab_1_new.Models;
using System.Windows;

namespace lab_1_new.View
{
    public partial class NewTaskWindow : Window
    {
        public TaskItem NewTask { get; set; }
        private TaskItem _editingTask;

        public NewTaskWindow()
        {
            InitializeComponent();
            DataContext = new NewTaskViewModel();
            Title = "Новая задача";
        }

        public NewTaskWindow(TaskItem taskToEdit)
        {
            InitializeComponent();
            _editingTask = taskToEdit;
            DataContext = new NewTaskViewModel(taskToEdit);
            Title = "Редактирование задачи";
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as NewTaskViewModel;

            if (string.IsNullOrWhiteSpace(viewModel.TaskTitle))
            {
                MessageBox.Show("Введите название задачи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewTask = viewModel.CreateTaskItem();
            if (_editingTask != null)
            {
                NewTask.Id = _editingTask.Id;
            }

            DialogResult = true;
            Close();
        }
    }
}