using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Tools
{
    public class CursorTool : Tool
    {
        private Point _startPoint;
        private bool _isDragging = false;
        private Shape _selectedShape;
        private readonly ColorService _colorService;
        private Point _shapeStartPosition;
        private Brush _originalStroke;
        private double _originalStrokeThickness;
        private DoubleCollection _originalStrokeDashArray;
        private bool _isAlreadySelected = false;

        private static Shape _currentlySelectedShape;
        private static CursorTool _currentInstance;

        public CursorTool(ColorService colorService)
        {
            Name = "Курсор";
            _colorService = colorService;
            _currentInstance = this;
        }

        public override void OnMouseDown(Point position, Canvas canvas)
        {
            if (_currentlySelectedShape != null)
            {
                ClearSelectionFromPrevious();
                _isAlreadySelected = false;
            }

            _selectedShape = FindShapeAtPosition(position, canvas);

            if (_selectedShape != null)
            {
                _isDragging = true;
                _startPoint = position;

                _shapeStartPosition = GetShapePosition(_selectedShape);

                if (!_isAlreadySelected || _selectedShape != _currentlySelectedShape)
                {
                    SaveOriginalProperties(_selectedShape);
                    _isAlreadySelected = true;
                }
                HighlightSelectedShape(_selectedShape);
                _currentlySelectedShape = _selectedShape;
            }
            else
            {
                ClearAllSelections();
                _isAlreadySelected = false;
            }
        }

        public override void OnMouseMove(Point position, Canvas canvas)
        {
            if (_isDragging && _selectedShape != null)
            {
                double deltaX = position.X - _startPoint.X;
                double deltaY = position.Y - _startPoint.Y;

                MoveShape(_selectedShape, _shapeStartPosition, deltaX, deltaY);
            }
        }

        public override void OnMouseUp(Point position, Canvas canvas)
        {
            _isDragging = false;
        }

        private Shape FindShapeAtPosition(Point position, Canvas canvas)
        {
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, position);

            if (hitTestResult != null)
            {
                DependencyObject current = hitTestResult.VisualHit;
                while (current != null && current != canvas)
                {
                    if (current is Shape shape)
                    {
                        return shape;
                    }
                    current = VisualTreeHelper.GetParent(current);
                }
            }

            return null;
        }

        private Point GetShapePosition(Shape shape)
        {
            if (shape is Line line)
            {
                return new Point(line.X1, line.Y1);
            }
            else if (shape is Polygon polygon)
            {
                return GetPolygonCenter(polygon);
            }
            else
            {
                double left = Canvas.GetLeft(shape);
                double top = Canvas.GetTop(shape);
                return new Point(
                    double.IsNaN(left) ? 0 : left,
                    double.IsNaN(top) ? 0 : top
                );
            }
        }

        private void MoveShape(Shape shape, Point startPosition, double deltaX, double deltaY)
        {
            if (shape is Line line)
            {
                double originalWidth = line.X2 - line.X1;
                double originalHeight = line.Y2 - line.Y1;

                line.X1 = startPosition.X + deltaX;
                line.Y1 = startPosition.Y + deltaY;

                line.X2 = line.X1 + originalWidth;
                line.Y2 = line.Y1 + originalHeight;
            }
            else if (shape is Polygon polygon)
            {
                PointCollection newPoints = new PointCollection();
                Point oldCenter = GetPolygonCenter(polygon);

                foreach (Point point in polygon.Points)
                {
                    double offsetX = point.X - oldCenter.X;
                    double offsetY = point.Y - oldCenter.Y;
                    double newX = startPosition.X + deltaX + offsetX;
                    double newY = startPosition.Y + deltaY + offsetY;
                    newPoints.Add(new Point(newX, newY));
                }

                polygon.Points = newPoints;
            }
            else
            {
                double newLeft = startPosition.X + deltaX;
                double newTop = startPosition.Y + deltaY;

                Canvas.SetLeft(shape, newLeft);
                Canvas.SetTop(shape, newTop);
            }
        }

        private Point GetPolygonCenter(Polygon polygon)
        {
            if (polygon.Points.Count == 0) return new Point(0, 0);

            double minX = polygon.Points[0].X;
            double maxX = polygon.Points[0].X;
            double minY = polygon.Points[0].Y;
            double maxY = polygon.Points[0].Y;

            foreach (Point point in polygon.Points)
            {
                if (point.X < minX) minX = point.X;
                if (point.X > maxX) maxX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.Y > maxY) maxY = point.Y;
            }

            return new Point((minX + maxX) / 2, (minY + maxY) / 2);
        }

        private void SaveOriginalProperties(Shape shape)
        {
            _originalStroke = shape.Stroke;
            _originalStrokeThickness = shape.StrokeThickness;
            _originalStrokeDashArray = shape.StrokeDashArray;
        }

        private void HighlightSelectedShape(Shape shape)
        {
            shape.Stroke = Brushes.Blue;
            shape.StrokeThickness = _originalStrokeThickness + 2;
            shape.StrokeDashArray = new DoubleCollection(new double[] { 4, 2 });

            shape.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = Colors.Blue,
                ShadowDepth = 0,
                BlurRadius = 10,
                Opacity = 0.7
            };
        }

        private void ClearSelectionFromPrevious()
        {
            if (_currentlySelectedShape != null)
            {
                _currentlySelectedShape.Stroke = _originalStroke;
                _currentlySelectedShape.StrokeThickness = _originalStrokeThickness;
                _currentlySelectedShape.StrokeDashArray = _originalStrokeDashArray;
                _currentlySelectedShape.Effect = null;

                _currentlySelectedShape = null;
                _isAlreadySelected = false;
            }
        }

        public void ClearSelection()
        {
            if (_selectedShape != null)
            {
                _selectedShape.Stroke = _originalStroke;
                _selectedShape.StrokeThickness = _originalStrokeThickness;
                _selectedShape.StrokeDashArray = _originalStrokeDashArray;
                _selectedShape.Effect = null;

                _selectedShape = null;
                _currentlySelectedShape = null;
                _isDragging = false;
                _isAlreadySelected = false;
            }
        }

        public void DeleteSelectedShape(Canvas canvas)
        {
            if (_selectedShape != null)
            {
                canvas.Children.Remove(_selectedShape);
                ClearSelection();
            }
        }

        public void ChangeStrokeColor(Color color)
        {
            if (_selectedShape != null)
            {
                _selectedShape.Stroke = new SolidColorBrush(color);
                _originalStroke = new SolidColorBrush(color);
            }
        }

        public void ChangeFillColor(Color color)
        {
            if (_selectedShape != null && _selectedShape is not Line)
            {
                _selectedShape.Fill = new SolidColorBrush(color);
            }
        }

        public bool HasSelection => _selectedShape != null;

        public static void ClearAllSelections()
        {
            if (_currentlySelectedShape != null && _currentInstance != null)
            {
                _currentInstance.ClearSelection();
            }
        }
    }
}