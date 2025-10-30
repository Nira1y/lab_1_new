using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Services;
using lab_2_graphic_editor.Tools;
using lab_2_graphic_editor.Utilities;
using lab_2_graphic_editor.ViewModel;
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

        public MainWindow()
        {
            InitializeComponent();

            _statusVM = (StatusViewModel)DataContext;
            _colorService = new ColorService();

            _statusVM.ColorChanged += OnColorChanged;

            _toolManager = new ToolManager();
            _cursorTool = new CursorTool(_colorService);
            _toolManager.CurrentTool = new BrushTool(_colorService);

            DrawingArea.DrawingCanvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            DrawingArea.DrawingCanvas.MouseMove += Canvas_MouseMove;
            DrawingArea.DrawingCanvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            DrawingArea.DrawingCanvas.MouseLeave += Canvas_MouseLeave;

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
                    cursorTool.ChangeStrokeColor(color);
                }
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && _toolManager.CurrentTool is CursorTool cursorTool)
            {
                cursorTool.DeleteSelectedShape(DrawingArea.DrawingCanvas);
                e.Handled = true;
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(DrawingArea.DrawingCanvas);
            _toolManager.HandleMouseDown(position, DrawingArea.DrawingCanvas);
            _statusVM.Coordinates = $"X: {position.X:0} Y: {position.Y:0}";
            DrawingArea.DrawingCanvas.CaptureMouse();
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

    }
}