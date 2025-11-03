using lab_2_graphic_editor.Models;
using lab_2_graphic_editor.Models.Texts;
using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Services;
using lab_2_graphic_editor.Tools;
using lab_2_graphic_editor.Utilities;
using lab_2_graphic_editor.ViewModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor
{
    public partial class MainWindow : Window
    {
        private ToolManager _toolManager;
        private StatusViewModel _statusVM;
        private ColorService _colorService;
        private CursorTool _cursorTool;
        private FileService _fileService;
        private string _currentProjectPath;
        private TextTool _textTool;
        private TextViewModel _textViewModel;

        public MainWindow()
        {
            InitializeComponent();

            _statusVM = (StatusViewModel)DataContext;
            _colorService = new ColorService();

            _statusVM.ColorChanged += OnColorChanged;

            _toolManager = new ToolManager();
            _cursorTool = new CursorTool(_colorService);
            _toolManager.CurrentTool = new BrushTool(_colorService);
            _fileService = new FileService();

            _textViewModel = new TextViewModel();
            _textTool = new TextTool(_colorService, _textViewModel);

            _cursorTool.SetTextTool(_textTool);

            TextToolbarPanel.DataContext = _textViewModel;

            _textViewModel.TextPropertiesChanged += OnTextPropertiesChanged;
            _textViewModel.TextColorChanged += OnTextColorChanged;

            DrawingArea.DrawingCanvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            DrawingArea.DrawingCanvas.MouseMove += Canvas_MouseMove;
            DrawingArea.DrawingCanvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            DrawingArea.DrawingCanvas.MouseLeave += Canvas_MouseLeave;

            DrawingArea.DrawingCanvas.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ClickCount == 2 && _toolManager.CurrentTool is CursorTool cursorTool)
                {
                    cursorTool.StartTextEditing();
                    e.Handled = true;
                }
            };

            this.KeyDown += MainWindow_KeyDown;

            this.Loaded += (s, e) => { this.Focus(); };
        }

        private void OnColorChanged(Color color)
        {
            _colorService.SetColor(color);

            if (_toolManager.CurrentTool is CursorTool cursorTool && cursorTool.HasSelection)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    cursorTool.ChangeFillColor(color);
                }
                else
                {
                    if (cursorTool.SelectedElement is TextBox)
                    {
                        cursorTool.ChangeTextColor(color);
                    }
                    else if (cursorTool.SelectedElement is Shape)
                    {
                        cursorTool.ChangeStrokeColor(color);
                    }
                }
            }
            else if (_toolManager.CurrentTool is TextTool textTool)
            {
                textTool.UpdateTextColor(color);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && _toolManager.CurrentTool is CursorTool cursorTool)
            {
                cursorTool.DeleteSelectedShape(DrawingArea.DrawingCanvas);
                e.Handled = true;
            }

            if (e.Key == Key.Escape)
            {
                if (_toolManager.CurrentTool is CursorTool _cursorTool && _cursorTool.HasSelection)
                {
                    _cursorTool.FinishTextEditing();
                }
            }

            if (e.Key == Key.F2 && _toolManager.CurrentTool is CursorTool cursorToolEdit && cursorToolEdit.HasSelection)
            {
                cursorToolEdit.StartTextEditing();
                e.Handled = true;
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                Point position = e.GetPosition(DrawingArea.DrawingCanvas);
                _toolManager.HandleMouseDown(position, DrawingArea.DrawingCanvas);
                _statusVM.Coordinates = $"X: {position.X:0} Y: {position.Y:0}";
                DrawingArea.DrawingCanvas.CaptureMouse();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(DrawingArea.DrawingCanvas);
            _toolManager.HandleMouseMove(position, DrawingArea.DrawingCanvas);
            _statusVM.Coordinates = $"X: {position.X:0} Y: {position.Y:0}";
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(DrawingArea.DrawingCanvas);
            _toolManager.HandleMouseUp(position, DrawingArea.DrawingCanvas);
            _statusVM.Coordinates = $"X: {position.X:0} Y: {position.Y:0}";
            DrawingArea.DrawingCanvas.ReleaseMouseCapture();
        }

        private void Canvas_MouseLeave(object sender, System.EventArgs e)
        {
            _statusVM.Coordinates = "X: -, Y: -";
        }

        #region Инструменты

        private void SelectCursor_Click(object sender, RoutedEventArgs e)
        {
            _cursorTool?.ClearSelection();
            _toolManager.CurrentTool = _cursorTool;
            _statusVM.CurrentTool = "Инструмент: Курсор";
        }

        private void SelectBrush_Click(object sender, RoutedEventArgs e)
        {
            _cursorTool?.ClearSelection();
            _toolManager.CurrentTool = new BrushTool(_colorService);
            _statusVM.CurrentTool = "Инструмент: Кисть";
        }

        private void SelectLine_Click(object sender, RoutedEventArgs e)
        {
            _cursorTool?.ClearSelection();
            _toolManager.CurrentTool = new LineTool(_colorService);
            _statusVM.CurrentTool = "Инструмент: Линия";
        }

        private void SelectRectangle_Click(object sender, RoutedEventArgs e)
        {
            _cursorTool?.ClearSelection();
            _toolManager.CurrentTool = new RectangleTool(_colorService);
            _statusVM.CurrentTool = "Инструмент: Прямоугольник";
        }

        private void SelectRectangleFill_Click(object sender, RoutedEventArgs e)
        {
            _cursorTool?.ClearSelection();
            _toolManager.CurrentTool = new RectangleTool(_colorService, true);
            _statusVM.CurrentTool = "Инструмент: Прямоугольник (с заливкой)";
        }

        private void SelectEllipse_Click(object sender, RoutedEventArgs e)
        {
            _cursorTool?.ClearSelection();
            _toolManager.CurrentTool = new EllipseTool(_colorService);
            _statusVM.CurrentTool = "Инструмент: Эллипс";
        }

        private void SelectEllipseFill_Click(object sender, RoutedEventArgs e)
        {
            _cursorTool?.ClearSelection();
            _toolManager.CurrentTool = new EllipseTool(_colorService, true);
            _statusVM.CurrentTool = "Инструмент: Эллипс (с заливкой)";
        }

        private void SelectTriangle_Click(object sender, RoutedEventArgs e)
        {
            _cursorTool?.ClearSelection();
            _toolManager.CurrentTool = new TriangleTool(_colorService);
            _statusVM.CurrentTool = "Инструмент: Треугольник";
        }

        private void SelectTriangleFill_Click(object sender, RoutedEventArgs e)
        {
            _cursorTool?.ClearSelection();
            _toolManager.CurrentTool = new TriangleTool(_colorService, true);
            _statusVM.CurrentTool = "Инструмент: Треугольник (с заливкой)";
        }

        private void SelectText_Click(object sender, RoutedEventArgs e)
        {
            _cursorTool?.ClearSelection();
            _toolManager.CurrentTool = _textTool;
            _statusVM.CurrentTool = "Инструмент: Текст";

            _textViewModel.ApplyToTextTool(_textTool);
        }

        #endregion

        #region Обработчики текста

        private void OnTextPropertiesChanged()
        {
            if (_toolManager.CurrentTool is CursorTool cursorTool && cursorTool.HasSelection)
            {
                cursorTool.ChangeTextFont(_textViewModel.FontFamily, _textViewModel.FontSize,
                    _textViewModel.FontWeight, _textViewModel.FontStyle);
            }
            else
            {
                _textViewModel.ApplyToTextTool(_textTool);
            }
        }

        private void OnTextColorChanged(Color color)
        {
            if (_toolManager.CurrentTool is CursorTool cursorTool && cursorTool.HasSelection)
            {
                cursorTool.ChangeTextColor(color);
            }
            else
            {
                _textTool.UpdateTextColor(color);
            }
        }

        #endregion

        #region Файловые операции

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Создать новый проект? Несохраненные изменения будут потеряны.",
                "Новый проект", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DrawingArea.DrawingCanvas.Children.Clear();
                _currentProjectPath = null;
                _statusVM.CurrentTool = "Новый проект";
            }
        }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Graphic Editor Project (*.gep)|*.gep|JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Открыть проект"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _fileService.LoadProject(DrawingArea.DrawingCanvas, openFileDialog.FileName);
                    _currentProjectPath = openFileDialog.FileName;
                    _statusVM.CurrentTool = $"Проект: {System.IO.Path.GetFileName(_currentProjectPath)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке проекта: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentProjectPath))
            {
                SaveProjectAs_Click(sender, e);
            }
            else
            {
                _fileService.SaveProject(DrawingArea.DrawingCanvas, _currentProjectPath);
                MessageBox.Show("Проект успешно сохранен!", "Сохранение",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveProjectAs_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Graphic Editor Project (*.gep)|*.gep|JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Сохранить проект как",
                DefaultExt = ".gep"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _fileService.SaveProject(DrawingArea.DrawingCanvas, saveFileDialog.FileName);
                _currentProjectPath = saveFileDialog.FileName;
                MessageBox.Show("Проект успешно сохранен!", "Сохранение",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportToPng_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image (*.png)|*.png",
                Title = "Экспорт в PNG",
                DefaultExt = ".png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _fileService.ExportToImage(DrawingArea.DrawingCanvas, saveFileDialog.FileName, "png");
                MessageBox.Show("Изображение успешно экспортировано в PNG!", "Экспорт",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportToJpeg_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JPEG Image (*.jpg)|*.jpg",
                Title = "Экспорт в JPEG",
                DefaultExt = ".jpg"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _fileService.ExportToImage(DrawingArea.DrawingCanvas, saveFileDialog.FileName, "jpg");
                MessageBox.Show("Изображение успешно экспортировано в JPEG!", "Экспорт",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Выйти из приложения? Несохраненные изменения будут потеряны.",
                "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        #endregion
    }
}