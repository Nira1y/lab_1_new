using lab_2_graphic_editor.Commands;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Models.Tools
{
    public class BrushTool : Tool
    {
        private Point previousPoint;
        private bool isDrawing = false;
        private double brushSize = 3;
        private readonly ColorService _colorService;
        private readonly CommandService _commandService;
        private List<UIElement> _currentStrokeElements = new List<UIElement>();

        public BrushTool(ColorService colorService, CommandService commandService)
        {
            Name = "Кисть";
            _colorService = colorService;
            _commandService = commandService;
        }

        public override void OnMouseDown(Point position, Canvas canvas)
        {
            isDrawing = true;
            previousPoint = position;
            _currentStrokeElements.Clear();
            DrawDot(position, canvas);
        }

        public override void OnMouseMove(Point position, Canvas canvas)
        {
            if (isDrawing)
            {
                var line = DrawLine(previousPoint, position, canvas);
                _currentStrokeElements.Add(line);
                previousPoint = position;
            }
        }

        public override void OnMouseUp(Point position, Canvas canvas)
        {
            if (isDrawing && _currentStrokeElements.Count > 0)
            {
                var commands = new List<ICommand>();
                foreach (var element in _currentStrokeElements)
                {
                    commands.Add(new AddElementCommand(element, canvas));
                }
                _commandService.ExecuteBatchCommand(commands);
            }

            isDrawing = false;
            _currentStrokeElements.Clear();
        }

        private Line DrawLine(Point start, Point end, Canvas canvas)
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
            return line;
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
            _currentStrokeElements.Add(dot);
        }

        public void SetBrushSize(double size)
        {
            brushSize = size;
        }
    }
}