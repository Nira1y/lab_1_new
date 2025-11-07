using lab_2_graphic_editor.Commands;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Models.Tools
{
    public class CurveTool : Tool
    {
        private List<Point> _nodes = new List<Point>();
        private Polyline _curvePolyline;
        private List<Ellipse> _nodeMarkers = new List<Ellipse>();
        private Canvas _currentCanvas;

        private int _dragIndex = -1;
        private bool _isDragging = false;
        private bool _isCurveCompleted = false;

        private Brush _strokeBrush = Brushes.Black;
        private double _strokeThickness = 2.0;
        private Brush _nodeFill = Brushes.White;
        private Brush _nodeStroke = Brushes.Red;
        private double _nodeRadius = 6.0;

        private readonly ColorService _colorService;
        private readonly CommandService _commandService;

        public CurveTool(ColorService colorService, CommandService commandService)
        {
            Name = "Кривая";
            _colorService = colorService;
            _commandService = commandService;

            _strokeBrush = _colorService.CurrentColor;

            _colorService.ColorChanged += OnColorChanged;
        }

        public override void OnMouseDown(Point position, Canvas canvas)
        {
            _currentCanvas = canvas;

            if (_isCurveCompleted)
            {
                StartNewCurve();
            }

            int hitIndex = HitTestNode(position);
            if (hitIndex >= 0)
            {
                _dragIndex = hitIndex;
                _isDragging = true;
                canvas.CaptureMouse();
            }
            else
            {
                _nodes.Add(position);
                var marker = CreateNodeMarker(position);
                _nodeMarkers.Add(marker);
                canvas.Children.Add(marker);

                UpdateCurve(canvas);
            }
        }

        public override void OnMouseMove(Point position, Canvas canvas)
        {
            if (_isDragging && _dragIndex >= 0 && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                _nodes[_dragIndex] = position;
                UpdateNodeMarker(_dragIndex, position);
                UpdateCurve(canvas);
            }
        }

        public override void OnMouseUp(Point position, Canvas canvas)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _dragIndex = -1;
                canvas.ReleaseMouseCapture();
            }
        }

        public void CompleteCurve()
        {
            if (_currentCanvas == null || _nodes.Count < 2 || _curvePolyline == null)
            {
                CancelCurve();
                return;
            }

            var completedCurve = _curvePolyline;

            var curvePoints = new PointCollection();
            foreach (var point in _curvePolyline.Points)
            {
                curvePoints.Add(point);
            }

            var finalCurve = new Polyline
            {
                Stroke = _strokeBrush,
                StrokeThickness = _strokeThickness,
                StrokeLineJoin = PenLineJoin.Round,
                Points = curvePoints
            };

            CleanupTemporaryElements();
            _isCurveCompleted = true;

            _commandService.ExecuteAddElement(finalCurve, _currentCanvas);

            _curvePolyline = null;
        }

        public void FinishCurve()
        {
            CompleteCurve();
        }

        public void CancelCurve()
        {
            if (_currentCanvas == null) return;

            CleanupTemporaryElements();
            _isCurveCompleted = false;
        }

        public void StartNewCurve()
        {
            CancelCurve();
            _nodes.Clear();
            _nodeMarkers.Clear();
            _curvePolyline = null;
            _isCurveCompleted = false;
        }

        private void CleanupTemporaryElements()
        {
            if (_currentCanvas == null) return;

            foreach (var marker in _nodeMarkers)
            {
                if (_currentCanvas.Children.Contains(marker))
                {
                    _currentCanvas.Children.Remove(marker);
                }
            }

            if (_curvePolyline != null && _currentCanvas.Children.Contains(_curvePolyline) && !_isCurveCompleted)
            {
                _currentCanvas.Children.Remove(_curvePolyline);
                _curvePolyline = null;
            }

            _nodeMarkers.Clear();
            _dragIndex = -1;
            _isDragging = false;
        }

        private int HitTestNode(Point point)
        {
            const double hitRadius = 10.0;
            for (int i = 0; i < _nodes.Count; i++)
            {
                if ((point - _nodes[i]).Length <= hitRadius)
                    return i;
            }
            return -1;
        }

        private Ellipse CreateNodeMarker(Point position)
        {
            var ellipse = new Ellipse
            {
                Width = _nodeRadius * 2,
                Height = _nodeRadius * 2,
                Fill = _nodeFill,
                Stroke = _nodeStroke,
                StrokeThickness = 1.5,
                Cursor = Cursors.Hand
            };

            Canvas.SetLeft(ellipse, position.X - _nodeRadius);
            Canvas.SetTop(ellipse, position.Y - _nodeRadius);

            return ellipse;
        }

        private void UpdateNodeMarker(int index, Point position)
        {
            if (index >= 0 && index < _nodeMarkers.Count)
            {
                var marker = _nodeMarkers[index];
                Canvas.SetLeft(marker, position.X - _nodeRadius);
                Canvas.SetTop(marker, position.Y - _nodeRadius);
            }
        }

        private void UpdateCurve(Canvas canvas)
        {
            if (_nodes.Count < 2)
            {
                if (_curvePolyline != null)
                {
                    canvas.Children.Remove(_curvePolyline);
                    _curvePolyline = null;
                }
                return;
            }

            if (_curvePolyline == null)
            {
                _curvePolyline = new Polyline
                {
                    Stroke = _strokeBrush,
                    StrokeThickness = _strokeThickness,
                    StrokeLineJoin = PenLineJoin.Round,
                };
                canvas.Children.Add(_curvePolyline);
            }

            var curvePoints = BuildCatmullRom(_nodes, 20);
            _curvePolyline.Points = new PointCollection(curvePoints);
        }

        private List<Point> BuildCatmullRom(List<Point> points, int samplesPerSegment)
        {
            var result = new List<Point>();
            if (points.Count < 2) return result;

            for (int i = 0; i < points.Count - 1; i++)
            {
                Point p0 = i == 0 ? points[i] : points[i - 1];
                Point p1 = points[i];
                Point p2 = points[i + 1];
                Point p3 = (i + 2 < points.Count) ? points[i + 2] : points[i + 1];

                for (int j = 0; j < samplesPerSegment; j++)
                {
                    double t = j / (double)samplesPerSegment;
                    var point = CatmullRomInterpolate(p0, p1, p2, p3, t);
                    result.Add(point);
                }
            }

            result.Add(points[points.Count - 1]);
            return result;
        }

        private static Point CatmullRomInterpolate(Point p0, Point p1, Point p2, Point p3, double t)
        {
            double t2 = t * t;
            double t3 = t2 * t;

            double x = 0.5 * ((2 * p1.X) +
                            (-p0.X + p2.X) * t +
                            (2 * p0.X - 5 * p1.X + 4 * p2.X - p3.X) * t2 +
                            (-p0.X + 3 * p1.X - 3 * p2.X + p3.X) * t3);

            double y = 0.5 * ((2 * p1.Y) +
                            (-p0.Y + p2.Y) * t +
                            (2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * t2 +
                            (-p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y) * t3);

            return new Point(x, y);
        }

        private void OnColorChanged(Brush newColor)
        {
            _strokeBrush = newColor;
            if (_curvePolyline != null)
                _curvePolyline.Stroke = newColor;
        }

        public void SetStrokeBrush(Brush brush)
        {
            _strokeBrush = brush;
            if (_curvePolyline != null)
                _curvePolyline.Stroke = brush;
        }

        public void SetStrokeThickness(double thickness)
        {
            _strokeThickness = thickness;
            if (_curvePolyline != null)
                _curvePolyline.StrokeThickness = thickness;
        }

        public void SetNodeAppearance(Brush fill, Brush stroke, double radius)
        {
            _nodeFill = fill;
            _nodeStroke = stroke;
            _nodeRadius = radius;
            foreach (var marker in _nodeMarkers)
            {
                marker.Fill = fill;
                marker.Stroke = stroke;
                marker.Width = radius * 2;
                marker.Height = radius * 2;
            }
        }

        public void ForceCompleteCurve()
        {
            if (_nodes.Count >= 2 && _curvePolyline != null)
            {
                CompleteCurve();
            }
            else
            {
                CancelCurve();
            }
        }

        ~CurveTool()
        {
            _colorService.ColorChanged -= OnColorChanged;
        }
    }
}