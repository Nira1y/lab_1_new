using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Tools
{
    public class BrushTool : Tool
    {
        private Point previousPoint;
        private bool isDrawing = false;
        private double brushSize = 3;
        private readonly ColorService _colorService;

        public BrushTool(ColorService colorService)
        {
            Name = "Кисть";
            IconPath = "/Resources/brush_icon.png";
            _colorService = colorService;
        }

        public override void OnMouseDown(Point position, Canvas canvas)
        {
            isDrawing = true;
            previousPoint = position;
            DrawDot(position, canvas);
        }

        public override void OnMouseMove(Point position, Canvas canvas)
        {
            if (isDrawing)
            {
                DrawLine(previousPoint, position, canvas);
                previousPoint = position;
            }
        }

        public override void OnMouseUp(Point position, Canvas canvas)
        {
            isDrawing = false;
        }

        private void DrawLine(Point start, Point end, Canvas canvas)
        {
            Line line = new Line
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
                Stroke = _colorService.CurrentColor,
                StrokeThickness = brushSize,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round
            };

            canvas.Children.Add(line);
        }

        private void DrawDot(Point point, Canvas canvas)
        {
            Ellipse dot = new Ellipse
            {
                Width = brushSize,
                Height = brushSize,
                Fill = _colorService.CurrentColor,
                Stroke = _colorService.CurrentColor
            };

            Canvas.SetLeft(dot, point.X - brushSize / 2);
            Canvas.SetTop(dot, point.Y - brushSize / 2);
            canvas.Children.Add(dot);
        }

        public void SetBrushSize(double size)
        {
            brushSize = size;
        }
    }
}