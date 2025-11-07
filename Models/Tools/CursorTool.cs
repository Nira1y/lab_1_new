using lab_2_graphic_editor.Models.Texts;
using lab_2_graphic_editor.Models;
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
        private Point _elementStartPosition;
        private bool _isAlreadySelected = false;
        private ResizeService.ResizeHandle _activeResizeHandle;
        private RotateHandle _rotateHandle;
        private List<UIElement> _selectionGroup = new List<UIElement>();
        private Canvas _currentCanvas;

        // Сервисы
        private readonly CommandService _commandService;
        private readonly SelectionService _selectionService;
        private readonly ResizeService _resizeService;
        private readonly RotationService _rotationService;
        private readonly HandleService _handleService;
        private readonly ZOrderService _zOrderService;
        private readonly ElementMovementService _movementService;
        private readonly TextEditingService _textEditingService;
        private readonly ColorChangeService _colorChangeService;
        private readonly ElementPropertiesService _propertiesService;

        // Для отслеживания изменений
        private ElementPropertiesService.ElementProperties _originalProperties;

        private static UIElement _currentlySelectedElement;
        private static CursorTool _currentInstance;
        private TextTool _textTool;

        public CursorTool(ColorService colorService, CommandService commandService)
        {
            Name = "Курсор";
            _currentInstance = this;
            _commandService = commandService;
            _selectionService = new SelectionService();
            _resizeService = new ResizeService();
            _rotationService = new RotationService();
            _handleService = new HandleService();
            _zOrderService = new ZOrderService();
            _movementService = new ElementMovementService();
            _textEditingService = new TextEditingService();
            _colorChangeService = new ColorChangeService(commandService, _selectionService);
            _propertiesService = new ElementPropertiesService();
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
            _elementStartPosition = _movementService.GetElementPosition(_selectedElement);

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
                _textEditingService.MakeTextBoxNonEditable(textBox);
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
                    Point rotatedDelta = _movementService.RotatePoint(mouseDelta, -rotateTransform.Angle);

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

                _movementService.MoveElementToPosition(_selectedElement, newX, newY);
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
                Point newPosition = _movementService.GetElementPosition(_selectedElement);
                if (_originalProperties.OriginalPosition != newPosition)
                {
                    _commandService.ExecuteModifyPosition(_selectedElement, _originalProperties.OriginalPosition, newPosition);
                }
            }
            else if (_isResizing && _selectedElement != null)
            {
                Size newSize = _propertiesService.GetElementSize(_selectedElement);
                if (_originalProperties.OriginalSize != newSize)
                {
                    _commandService.ExecuteModifySize(_selectedElement, _originalProperties.OriginalSize, newSize);
                }
            }
            else if (_isRotating && _selectedElement != null)
            {
                double newRotation = _propertiesService.GetElementRotation(_selectedElement);
                if (Math.Abs(_originalProperties.OriginalRotation - newRotation) > 0.1)
                {
                    var rotationCommand = new ModifyRotationCommand(_selectedElement, _originalProperties.OriginalRotation, newRotation);
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
                _originalProperties = _propertiesService.SaveOriginalProperties(_selectedElement);
            }
        }

        #region Basic Selection and Movement

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
            _colorChangeService.ChangeStrokeColor(_selectedElement, color);
        }

        public void ChangeFillColor(Color color)
        {
            _colorChangeService.ChangeFillColor(_selectedElement, color);
        }

        public void ChangeTextColor(Color color)
        {
            _colorChangeService.ChangeTextColor(_selectedElement, color);
        }

        public void ChangeStrokeOrTextColor(Color color)
        {
            _colorChangeService.ChangeStrokeOrTextColor(_selectedElement, _selectionGroup, color);
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
                _textEditingService.ChangeTextFont(textBox, fontFamily, fontSize, fontWeight, fontStyle);
            }
        }

        public void SetTextTool(TextTool textTool)
        {
            _textTool = textTool;
            _textEditingService.SetTextTool(textTool);
        }

        public void StartTextEditing()
        {
            if (_selectedElement is TextBox textBox)
            {
                _textEditingService.StartTextEditing(textBox);
            }
        }

        public void FinishTextEditing()
        {
            if (_selectedElement is TextBox textBox)
            {
                _textEditingService.FinishTextEditing(textBox);
            }
        }

        #endregion

    }
}