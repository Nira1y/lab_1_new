using lab_2_graphic_editor.Models.Texts;
using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Commands;
using lab_2_graphic_editor.Services;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Tools
{
    public class CursorTool : Tool
    {
        private Point _startPoint;
        private bool _isDragging = false;
        private bool _isResizing = false;
        private bool _isRotating = false;
        private UIElement _selectedElement;
        private readonly ColorService _colorService;
        private Point _elementStartPosition;
        private bool _isAlreadySelected = false;
        private ResizeService.ResizeHandle _activeResizeHandle;
        private RotateHandle _rotateHandle;
        private List<UIElement> _selectionGroup = new List<UIElement>();
        private Canvas _currentCanvas;
        private readonly CommandService _commandService;
        private readonly SelectionService _selectionService;
        private readonly ResizeService _resizeService;
        private readonly RotationService _rotationService;
        private readonly HandleService _handleService;
        private readonly ZOrderService _zOrderService;

        // Для отслеживания изменений
        private Point _originalPosition;
        private double _originalRotation;
        private Brush _originalStroke;
        private Brush _originalFill;
        private Brush _originalForeground;
        private Size _originalSize;

        private static UIElement _currentlySelectedElement;
        private static CursorTool _currentInstance;
        private TextTool _textTool;

        public CursorTool(ColorService colorService, CommandService commandService)
        {
            Name = "Курсор";
            _colorService = colorService;
            _currentInstance = this;
            _commandService = commandService;
            _selectionService = new SelectionService();
            _resizeService = new ResizeService();
            _rotationService = new RotationService();
            _handleService = new HandleService();
            _zOrderService = new ZOrderService();
        }

        public UIElement SelectedElement => _selectedElement;
        public bool HasSelection => _selectedElement != null || _selectionGroup.Count > 0;

        public override void OnMouseDown(Point position, Canvas canvas)
        {
            if (canvas == null) return;

            _currentCanvas = canvas;

            _activeResizeHandle = _handleService.GetResizeHandleAtPosition(position, canvas);
            _isRotating = IsRotateHandleAtPosition(position, canvas);

            if (_activeResizeHandle != ResizeService.ResizeHandle.None || _isRotating)
            {
                _isResizing = _activeResizeHandle != ResizeService.ResizeHandle.None;
                _startPoint = position;

                // Сохраняем исходные значения перед изменением
                SaveOriginalProperties();
                return;
            }

            UIElement clickedElement = _selectionService.FindElementAtPosition(position, canvas);

            bool isMultiSelect = Keyboard.IsKeyDown(Key.LeftCtrl) ||
                                 Keyboard.IsKeyDown(Key.RightCtrl) ||
                                 Keyboard.IsKeyDown(Key.LeftShift) ||
                                 Keyboard.IsKeyDown(Key.RightShift);

            if (clickedElement != null)
            {
                HandleSingleElementSelection(clickedElement, position, isMultiSelect);
            }
            else
            {
                if (!isMultiSelect)
                {
                    ClearAllSelections();
                    _isAlreadySelected = false;
                    RemoveHandles(canvas);
                }
            }
        }

        private void HandleSingleElementSelection(UIElement clickedElement, Point position, bool isMultiSelect)
        {
            if (_currentlySelectedElement != null && _currentlySelectedElement != clickedElement)
            {
                ClearSelectionFromPrevious();
                _isAlreadySelected = false;
            }

            _isDragging = true;
            _startPoint = position;
            _selectedElement = clickedElement;
            _elementStartPosition = GetElementPosition(_selectedElement);

            // Сохраняем исходные свойства перед перемещением
            SaveOriginalProperties();

            if (!_isAlreadySelected || _selectedElement != _currentlySelectedElement)
            {
                _selectionService.SaveOriginalProperties(_selectedElement);
                _isAlreadySelected = true;
            }

            _selectionService.HighlightSelectedElement(_selectedElement);
            _currentlySelectedElement = _selectedElement;

            if (_selectedElement is TextBox textBox)
            {
                MakeTextBoxNonEditable(textBox);
            }

            _handleService.CreateResizeHandles(_currentCanvas, _selectedElement);
            CreateRotateHandle(_currentCanvas);
        }

        public override void OnMouseMove(Point position, Canvas canvas)
        {
            if (canvas == null) return;

            if (_isDragging && _selectedElement != null)
            {
                double deltaX, deltaY;

                if (_selectedElement.RenderTransform is RotateTransform rotateTransform && rotateTransform.Angle != 0)
                {
                    Point center = _rotationService.GetElementCenter(_selectedElement);

                    Point mouseDelta = new Point(position.X - _startPoint.X, position.Y - _startPoint.Y);
                    Point rotatedDelta = RotatePoint(mouseDelta, -rotateTransform.Angle);

                    deltaX = rotatedDelta.X;
                    deltaY = rotatedDelta.Y;
                }
                else
                {
                    deltaX = position.X - _startPoint.X;
                    deltaY = position.Y - _startPoint.Y;
                }

                double newX = _elementStartPosition.X + deltaX;
                double newY = _elementStartPosition.Y + deltaY;

                MoveElementToPosition(_selectedElement, newX, newY);
                UpdateHandlesPosition(canvas);
            }
            else if (_isResizing && _selectedElement != null)
            {
                _resizeService.ResizeElement(_selectedElement, _activeResizeHandle, _startPoint, position);
                _startPoint = position;
                UpdateHandlesPosition(canvas);
            }
            else if (_isRotating && _selectedElement != null)
            {
                Point center = _rotationService.GetElementCenter(_selectedElement);
                _rotationService.RotateElement(_selectedElement, center, _startPoint, position);
                _startPoint = position;
                UpdateHandlesPosition(canvas);
            }
        }

        public override void OnMouseUp(Point position, Canvas canvas)
        {
            // Регистрируем команды после завершения действий
            if (_isDragging && _selectedElement != null)
            {
                Point newPosition = GetElementPosition(_selectedElement);
                if (_originalPosition != newPosition)
                {
                    _commandService.ExecuteModifyPosition(_selectedElement, _originalPosition, newPosition);
                }
            }
            else if (_isResizing && _selectedElement != null)
            {
                Size newSize = GetElementSize(_selectedElement);
                if (_originalSize != newSize)
                {
                    _commandService.ExecuteModifySize(_selectedElement, _originalSize, newSize);
                }
            }
            else if (_isRotating && _selectedElement != null)
            {
                double newRotation = GetElementRotation(_selectedElement);
                if (Math.Abs(_originalRotation - newRotation) > 0.1)
                {
                    // Для поворота используем команду модификации с кастомными параметрами
                    var rotationCommand = new ModifyRotationCommand(_selectedElement, _originalRotation, newRotation);
                    _commandService.CommandManager.Execute(rotationCommand);
                }
            }

            _isDragging = false;
            _isResizing = false;
            _isRotating = false;
            _activeResizeHandle = ResizeService.ResizeHandle.None;
        }

        private void SaveOriginalProperties()
        {
            if (_selectedElement != null)
            {
                _originalPosition = GetElementPosition(_selectedElement);
                _originalRotation = GetElementRotation(_selectedElement);
                _originalSize = GetElementSize(_selectedElement);

                if (_selectedElement is Shape shape)
                {
                    _originalStroke = shape.Stroke;
                    _originalFill = shape.Fill;
                }
                else if (_selectedElement is TextBox textBox)
                {
                    _originalForeground = textBox.Foreground;
                }
            }
        }

        private double GetElementRotation(UIElement element)
        {
            if (element.RenderTransform is RotateTransform rotateTransform)
            {
                return rotateTransform.Angle;
            }
            return 0;
        }

        private Size GetElementSize(UIElement element)
        {
            if (element is FrameworkElement frameworkElement)
            {
                return new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight);
            }
            return new Size(0, 0);
        }

        private Point RotatePoint(Point point, double angle)
        {
            double angleRad = angle * Math.PI / 180.0;
            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);

            return new Point(
                point.X * cos - point.Y * sin,
                point.X * sin + point.Y * cos
            );
        }

        private void MoveElementToPosition(UIElement element, double newX, double newY)
        {
            if (element is Line line)
            {
                double deltaX = newX - line.X1;
                double deltaY = newY - line.Y1;

                line.X1 = newX;
                line.Y1 = newY;
                line.X2 += deltaX;
                line.Y2 += deltaY;
            }
            else if (element is Polygon polygon)
            {
                Point currentCenter = GetPolygonCenter(polygon);
                double deltaX = newX - currentCenter.X;
                double deltaY = newY - currentCenter.Y;

                PointCollection newPoints = new PointCollection();
                foreach (Point point in polygon.Points)
                {
                    newPoints.Add(new Point(point.X + deltaX, point.Y + deltaY));
                }
                polygon.Points = newPoints;
            }
            else if (element is Polyline polyline)
            {
                Point currentCenter = GetPolylineCenter(polyline);
                double deltaX = newX - currentCenter.X;
                double deltaY = newY - currentCenter.Y;

                PointCollection newPoints = new PointCollection();
                foreach (Point point in polyline.Points)
                {
                    newPoints.Add(new Point(point.X + deltaX, point.Y + deltaY));
                }
                polyline.Points = newPoints;
            }
            else if (element is TextBox textBox)
            {
                Canvas.SetLeft(textBox, newX);
                Canvas.SetTop(textBox, newY);
            }
            else
            {
                Canvas.SetLeft(element, newX);
                Canvas.SetTop(element, newY);
            }
        }

        #region Basic Selection and Movement

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
            else if (element is Polyline polyline)
            {
                return GetPolylineCenter(polyline);
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

        private Point GetPolygonCenter(Polygon polygon)
        {
            if (polygon.Points.Count == 0) return new Point(0, 0);

            double sumX = 0;
            double sumY = 0;

            foreach (Point point in polygon.Points)
            {
                sumX += point.X;
                sumY += point.Y;
            }

            return new Point(sumX / polygon.Points.Count, sumY / polygon.Points.Count);
        }

        private Point GetPolylineCenter(Polyline polyline)
        {
            if (polyline.Points.Count == 0) return new Point(0, 0);

            double sumX = 0;
            double sumY = 0;

            foreach (Point point in polyline.Points)
            {
                sumX += point.X;
                sumY += point.Y;
            }

            return new Point(sumX / polyline.Points.Count, sumY / polyline.Points.Count);
        }

        private void MakeTextBoxNonEditable(TextBox textBox)
        {
            textBox.IsHitTestVisible = false;
            textBox.Focusable = false;
            textBox.Cursor = Cursors.SizeAll;
        }

        private void ClearSelectionFromPrevious()
        {
            if (_currentlySelectedElement != null)
            {
                _selectionService.ClearElementHighlight(_currentlySelectedElement);
                _currentlySelectedElement = null;
                _isAlreadySelected = false;
            }
        }

        public void ClearSelection()
        {
            if (_selectedElement != null)
            {
                _selectionService.ClearElementHighlight(_selectedElement);
                _selectedElement = null;
                _currentlySelectedElement = null;
                _isDragging = false;
                _isAlreadySelected = false;
            }

            if (_currentCanvas != null)
            {
                RemoveHandles(_currentCanvas);
            }
        }

        public static void ClearAllSelections()
        {
            if (_currentlySelectedElement != null && _currentInstance != null)
            {
                _currentInstance.ClearSelection();
            }
        }

        #endregion

        #region Color Changes with Undo/Redo

        public void ChangeStrokeColor(Color color)
        {
            if (_selectedElement != null && _selectedElement is Shape shape)
            {
                var newBrush = new SolidColorBrush(color);
                _commandService.ExecuteModifyStroke(_selectedElement, shape.Stroke, newBrush);
                _selectionService.UpdateStrokeColor(_selectedElement, color);
            }
        }

        public void ChangeFillColor(Color color)
        {
            if (_selectedElement != null && _selectedElement is Shape shape)
            {
                var newBrush = new SolidColorBrush(color);
                _commandService.ExecuteModifyFill(_selectedElement, shape.Fill, newBrush);
                _selectionService.UpdateFillColor(_selectedElement, color);
            }     
        }

        public void ChangeTextColor(Color color)
        {
            if (_selectedElement is TextBox textBox)
            {
                var newBrush = new SolidColorBrush(color);
                _commandService.ExecuteModifyForeground(_selectedElement, textBox.Foreground, newBrush);
                textBox.Foreground = newBrush;
            }
        }

        public void ChangeStrokeOrTextColor(Color color)
        {
            if (_selectedElement != null)
            {
                if (_selectedElement is TextBox textBox)
                {
                    var newBrush = new SolidColorBrush(color);
                    _commandService.ExecuteModifyForeground(_selectedElement, textBox.Foreground, newBrush);
                    textBox.Foreground = newBrush;
                }
                else if (_selectedElement is Shape shape)
                {
                    var newBrush = new SolidColorBrush(color);
                    _commandService.ExecuteModifyStroke(_selectedElement, shape.Stroke, newBrush);
                    shape.Stroke = newBrush;
                }
            }

            foreach (var element in _selectionGroup)
            {
                if (element is TextBox textBox)
                {
                    var newBrush = new SolidColorBrush(color);
                    _commandService.ExecuteModifyForeground(element, textBox.Foreground, newBrush);
                    textBox.Foreground = newBrush;
                }
                else if (element is Shape shape)
                {
                    var newBrush = new SolidColorBrush(color);
                    _commandService.ExecuteModifyStroke(element, shape.Stroke, newBrush);
                    shape.Stroke = newBrush;
                }
            }
        }

        #endregion


        #region Rotation Handles

        private void CreateRotateHandle(Canvas canvas)
        {
            if (canvas == null || _selectedElement == null) return;

            RemoveRotateHandle(canvas);

            Rect bounds = _resizeService.GetElementBounds(_selectedElement);
            Point rotateHandlePosition = new Point(bounds.Left + bounds.Width / 2, bounds.Top - 30);

            _rotateHandle = new RotateHandle
            {
                Position = rotateHandlePosition,
                Visual = new Ellipse
                {
                    Width = 12,
                    Height = 12,
                    Fill = Brushes.White,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                }
            };

            Canvas.SetLeft(_rotateHandle.Visual, rotateHandlePosition.X - 6);
            Canvas.SetTop(_rotateHandle.Visual, rotateHandlePosition.Y - 6);
            canvas.Children.Add(_rotateHandle.Visual);
        }

        private bool IsRotateHandleAtPosition(Point position, Canvas canvas)
        {
            if (canvas == null || _rotateHandle?.Visual == null) return false;

            double left = Canvas.GetLeft(_rotateHandle.Visual);
            double top = Canvas.GetTop(_rotateHandle.Visual);

            Rect handleBounds = new Rect(left, top, _rotateHandle.Visual.Width, _rotateHandle.Visual.Height);

            Rect expandedBounds = new Rect(
                handleBounds.X - 3,
                handleBounds.Y - 3,
                handleBounds.Width + 6,
                handleBounds.Height + 6
            );

            return expandedBounds.Contains(position);
        }

        private void RemoveRotateHandle(Canvas canvas)
        {
            if (canvas == null || _rotateHandle?.Visual == null) return;

            canvas.Children.Remove(_rotateHandle.Visual);
            _rotateHandle = null;
        }

        #endregion

        #region Z-Order Management

        public void BringToFront()
        {
            if (_currentCanvas == null || _selectedElement == null) return;

            _zOrderService.BringToFront(_currentCanvas, _selectedElement);
        }

        public void SendToBack()
        {
            if (_currentCanvas == null || _selectedElement == null) return;

            _zOrderService.SendToBack(_currentCanvas, _selectedElement);
        }

        #endregion

        #region Helper Methods

        private void UpdateHandlesPosition(Canvas canvas)
        {
            if (canvas == null) return;

            RemoveHandles(canvas);
            _handleService.CreateResizeHandles(canvas, _selectedElement);
            CreateRotateHandle(canvas);
        }

        private void RemoveHandles(Canvas canvas)
        {
            if (canvas == null) return;

            _handleService.RemoveResizeHandles(canvas);
            RemoveRotateHandle(canvas);
        }

        #endregion

        #region Text Editing

        public void DeleteSelectedShape()
        {
            if (_currentCanvas == null || _selectedElement == null) return;

            _commandService.ExecuteRemoveElement(_selectedElement, _currentCanvas);
            ClearSelection();
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

        #endregion

        private class RotateHandle
        {
            public Point Position { get; set; }
            public Shape Visual { get; set; }
        }
    }
    public class ModifyRotationCommand : Commands.ICommand
    {
        private readonly UIElement _element;
        private readonly double _oldAngle;
        private readonly double _newAngle;

        public ModifyRotationCommand(UIElement element, double oldAngle, double newAngle)
        {
            _element = element;
            _oldAngle = oldAngle;
            _newAngle = newAngle;
        }

        public void Execute()
        {
            SetRotation(_newAngle);
        }

        public void Undo()
        {
            SetRotation(_oldAngle);
        }

        private void SetRotation(double angle)
        {
            if (_element.RenderTransform is RotateTransform rotateTransform)
            {
                rotateTransform.Angle = angle;
            }
            else
            {
                _element.RenderTransform = new RotateTransform(angle);
            }
        }
    }
}