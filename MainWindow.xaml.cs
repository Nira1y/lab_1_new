using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Services;
using lab_2_graphic_editor.Tools;
using lab_2_graphic_editor.Utilities;
using lab_2_graphic_editor.ViewModel;
using System.Net.NetworkInformation;
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
        private Color CurrentColor = Constants.DEFAULT_STROKE_COLOR;
        private ColorService _colorService;

        public MainWindow()
        {
            InitializeComponent();

            _statusVM = (StatusViewModel)DataContext;
            _colorService = new ColorService();

            _statusVM.ColorChanged += (color) => _colorService.SetColor(color);

            _toolManager = new ToolManager();
            _toolManager.CurrentTool = new BrushTool(_colorService);

            DrawingArea.DrawingCanvas.MouseDown += Canvas_MouseDown;
            DrawingArea.DrawingCanvas.MouseMove += Canvas_MouseMove;
            DrawingArea.DrawingCanvas.MouseUp += Canvas_MouseUp;
            DrawingArea.DrawingCanvas.MouseLeave += Canvas_MouseLeave;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(DrawingArea.DrawingCanvas);
            _toolManager.HandleMouseDown(position, DrawingArea.DrawingCanvas);
            _statusVM.Coordinates = $"X: {position.X:0} Y: {position.Y:0}";
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(DrawingArea.DrawingCanvas);
            _toolManager.HandleMouseMove(position, DrawingArea.DrawingCanvas);
            _statusVM.Coordinates = $"X: {position.X:0} Y: {position.Y:0}";
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(DrawingArea.DrawingCanvas);
            _toolManager.HandleMouseUp(position, DrawingArea.DrawingCanvas);
            _statusVM.Coordinates = $"X: {position.X:0} Y: {position.Y:0}";
        }

        private void Canvas_MouseLeave(object sender, EventArgs e)
        {
            _statusVM.Coordinates = "X: -, Y: -";
        }
        private void SelectBrush_Click(object sender, RoutedEventArgs e)
        {
            _toolManager.CurrentTool = new BrushTool(_colorService);
            _statusVM.CurrentTool = "Инструмент: Кисть";
        }

        private void SelectLine_Click(object sender, RoutedEventArgs e)
        {
            _toolManager.CurrentTool = new LineTool(_colorService);
            _statusVM.CurrentTool = "Инструмент: Линия";
        }
        private void SelectRectangle_Click(object sender, RoutedEventArgs e)
        {
            _toolManager.CurrentTool = new RectangleTool(_colorService);
            _statusVM.CurrentTool = "Инструмент: Прямоугольник";
        }
        private void SelectRectangleFill_Click(object sender, RoutedEventArgs e)
        {
            _toolManager.CurrentTool = new RectangleTool(_colorService, true);
            _statusVM.CurrentTool = "Инструмент: Прямоугольник (с заливкой)";
        }
        private void SelectEllipse_Click(object sender, RoutedEventArgs e)
        {
            _toolManager.CurrentTool = new EllipseTool(_colorService);
            _statusVM.CurrentTool = "Инструмент: Эллипс";
        }
        private void SelectEllipseFill_Click(object sender, RoutedEventArgs e)
        {
            _toolManager.CurrentTool = new EllipseTool(_colorService, true);
            _statusVM.CurrentTool = "Инструмент: Эллипс (с заливкой)";
        }
        private void SelectTriangle_Click(object sender, RoutedEventArgs e)
        {
            _toolManager.CurrentTool = new TriangleTool(_colorService);
            _statusVM.CurrentTool = "Инструмент: Треугольник";
        }
        private void SelectTriangleFill_Click(object sender, RoutedEventArgs e)
        {
            _toolManager.CurrentTool = new TriangleTool(_colorService,true);
            _statusVM.CurrentTool = "Инструмент: Треугольник (с заливкой)";
        }
       
        }

    }
