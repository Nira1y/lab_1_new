using lab_1_new.Models;
using lab_1_new.View;
using lab_1_new.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab_1_new
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void TaskListItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TaskListItem taskListItem)
            {
                var viewModel = DataContext as MainWindiowViewModel;
                if (viewModel != null && taskListItem.DataContext is TaskItem task)
                {
                    viewModel.SelectedTask = task;
                }

            }
        }
    }
}