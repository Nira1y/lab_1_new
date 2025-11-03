using lab_2_graphic_editor.Models.Texts;
using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Tools
{
    public class CursorTool : Tool
    {
        private Point _startPoint;
        private bool _isDragging = false;
        private UIElement _selectedElement;
        private readonly ColorService _colorService;
        private Point _elementStartPosition;
        private Brush _originalStroke;
        private double _originalStrokeThickness;
        private DoubleCollection _originalStrokeDashArray;
        private bool _isAlreadySelected = false;

        private static UIElement _currentlySelectedElement;
        private static CursorTool _currentInstance;
        public event Action<TextBox> TextEditRequested;
        private TextTool _textTool;

        public CursorTool(ColorService colorService)
        {
            Name = "Курсор";
            _colorService = colorService;
            _currentInstance = this;
        }

        public UIElement SelectedElement => _selectedElement;
        public bool HasSelection => _selectedElement != null;

        public override void OnMouseDown(Point position, Canvas canvas)
        {
            if (_currentlySelectedElement != null)
            {
                ClearSelectionFromPrevious();
                _isAlreadySelected = false;
            }

            _selectedElement = FindElementAtPosition(position, canvas);

            if (_selectedElement != null)
            {
                _isDragging = true;
                _startPoint = position;
                _elementStartPosition = GetElementPosition(_selectedElement);

                if (!_isAlreadySelected || _selectedElement != _currentlySelectedElement)
                {
                    SaveOriginalProperties(_selectedElement);
                    _isAlreadySelected = true;
                }

                HighlightSelectedElement(_selectedElement);
                _currentlySelectedElement = _selectedElement;

                if (_selectedElement is TextBox textBox)
                {
                    MakeTextBoxNonEditable(textBox);
                }
            }
            else
            {
                ClearAllSelections();
                _isAlreadySelected = false;
            }
        }

        public override void OnMouseMove(Point position, Canvas canvas)
        {
            if (_isDragging && _selectedElement != null)
            {
                double deltaX = position.X - _startPoint.X;
                double deltaY = position.Y - _startPoint.Y;

                MoveElement(_selectedElement, _elementStartPosition, deltaX, deltaY);
            }
        }

        public override void OnMouseUp(Point position, Canvas canvas)
        {
            _isDragging = false;
        }

        private UIElement FindElementAtPosition(Point position, Canvas canvas)
        {
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, position);

            if (hitTestResult != null)
            {
                DependencyObject current = hitTestResult.VisualHit;
                while (current != null && current != canvas)
                {
                    if (current is Shape shape || current is TextBox textBox)
                    {
                        return current as UIElement;
                    }
                    current = VisualTreeHelper.GetParent(current);
                }
            }

            return null;
        }

        private Point GetElementPosition(UIElement element)
        {
            if (element is Line line)
            {
                return new Point(line.X1, line.Y1);
            }
            else if (element is Polygon polygon)
            {
                return GetPolygonCenter(polygon);
            }
            else if (element is TextBox textBox)
            {
                return new Point(Canvas.GetLeft(textBox), Canvas.GetTop(textBox));
            }
            else
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                return new Point(
                    double.IsNaN(left) ? 0 : left,
                    double.IsNaN(top) ? 0 : top
                );
            }
        }

        private void MoveElement(UIElement element, Point startPosition, double deltaX, double deltaY)
        {
            if (element is Line line)
            {
                double originalWidth = line.X2 - line.X1;
                double originalHeight = line.Y2 - line.Y1;

                line.X1 = startPosition.X + deltaX;
                line.Y1 = startPosition.Y + deltaY;

                line.X2 = line.X1 + originalWidth;
                line.Y2 = line.Y1 + originalHeight;
            }
            else if (element is Polygon polygon)
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
            else if (element is TextBox textBox)
            {
                // TextBox перемещаем всегда
                double newLeft = startPosition.X + deltaX;
                double newTop = startPosition.Y + deltaY;

                Canvas.SetLeft(textBox, newLeft);
                Canvas.SetTop(textBox, newTop);
            }
            else
            {
                double newLeft = startPosition.X + deltaX;
                double newTop = startPosition.Y + deltaY;

                Canvas.SetLeft(element, newLeft);
                Canvas.SetTop(element, newTop);
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

        private void SaveOriginalProperties(UIElement element)
        {
            if (element is Shape shape)
            {
                _originalStroke = shape.Stroke;
                _originalStrokeThickness = shape.StrokeThickness;
                _originalStrokeDashArray = shape.StrokeDashArray;
            }
        }

        private void HighlightSelectedElement(UIElement element)
        {
            if (element is Shape shape)
            {
                shape.Stroke = Brushes.Blue;
                shape.StrokeThickness = _originalStrokeThickness + 2;
                shape.StrokeDashArray = new DoubleCollection(new double[] { 4, 2 });
            }
            else if (element is TextBox textBox)
            {
                textBox.BorderThickness = new Thickness(2);
                textBox.BorderBrush = Brushes.Blue;
            }
        }

        private void MakeTextBoxNonEditable(TextBox textBox)
        {
            textBox.IsHitTestVisible = false;
            textBox.Focusable = false;
            textBox.Cursor = Cursors.SizeAll;
        }

        private static void MakeTextBoxEditable(TextBox textBox)
        {
            textBox.IsHitTestVisible = true;
            textBox.Focusable = true;
            textBox.Cursor = Cursors.IBeam;
        }

        private void ClearSelectionFromPrevious()
        {
            if (_currentlySelectedElement != null)
            {
                if (_currentlySelectedElement is Shape shape)
                {
                    shape.Stroke = _originalStroke;
                    shape.StrokeThickness = _originalStrokeThickness;
                    shape.StrokeDashArray = _originalStrokeDashArray;
                }
                else if (_currentlySelectedElement is TextBox textBox)
                {
                    textBox.BorderThickness = new Thickness(0);
                    MakeTextBoxNonEditable(textBox);
                }

                _currentlySelectedElement = null;
                _isAlreadySelected = false;
            }
        }

        public void ClearSelection()
        {
            if (_selectedElement != null)
            {
                if (_selectedElement is Shape shape)
                {
                    shape.Stroke = _originalStroke;
                    shape.StrokeThickness = _originalStrokeThickness;
                    shape.StrokeDashArray = _originalStrokeDashArray;
                }
                else if (_selectedElement is TextBox textBox)
                {
                    textBox.BorderThickness = new Thickness(0);
                    MakeTextBoxNonEditable(textBox);
                }

                _selectedElement = null;
                _currentlySelectedElement = null;
                _isDragging = false;
                _isAlreadySelected = false;
            }
        }

        public void DeleteSelectedShape(Canvas canvas)
        {
            if (_selectedElement != null)
            {
                canvas.Children.Remove(_selectedElement);
                ClearSelection();
            }
        }

        public void ChangeStrokeColor(Color color)
        {
            if (_selectedElement is Shape shape)
            {
                shape.Stroke = new SolidColorBrush(color);
                _originalStroke = new SolidColorBrush(color);
            }
        }

        public void ChangeFillColor(Color color)
        {
            if (_selectedElement is Shape shape && shape is not Line)
            {
                shape.Fill = new SolidColorBrush(color);
            }
        }

        public void ChangeTextColor(Color color)
        {
            if (_selectedElement is TextBox textBox)
            {
                textBox.Foreground = new SolidColorBrush(color);
            }
        }

        public void ChangeTextFont(string fontFamily, double fontSize, FontWeight fontWeight, FontStyle fontStyle)
        {
            if (_selectedElement is TextBox textBox)
            {
                textBox.FontFamily = new FontFamily(fontFamily);
                textBox.FontSize = fontSize;
                textBox.FontWeight = fontWeight;
                textBox.FontStyle = fontStyle;
            }
        }

        public void SetTextTool(TextTool textTool)
        {
            _textTool = textTool;
        }

        public void StartTextEditing()
        {
            if (_selectedElement is TextBox textBox)
            {
                _textTool?.StartTextEditing(textBox);
            }
        }

        public void FinishTextEditing()
        {
            if (_selectedElement is TextBox textBox)
            {
                MakeTextBoxNonEditable(textBox);
                textBox.Background = Brushes.Transparent;
                textBox.BorderBrush = Brushes.Blue;

                textBox.LostFocus -= TextBox_LostFocus;
                textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                FinishTextEditing();
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (e.Key == Key.Escape || (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift))
                {
                    FinishTextEditing();
                    e.Handled = true;
                }
            }
        }

        public static void ClearAllSelections()
        {
            if (_currentlySelectedElement != null && _currentInstance != null)
            {
                _currentInstance.ClearSelection();
            }
        }
    }
}