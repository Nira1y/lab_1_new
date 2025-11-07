using lab_2_graphic_editor.Models.Tools;
using lab_2_graphic_editor.Services;
using lab_2_graphic_editor.Commands;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Tools
{
    public class EraserTool : Tool
    {
        private const int EraserSize = 20;
        private Point _lastPoint;
        private bool _isErasing;
        private readonly CommandService _commandService;

        private List<UIElement> _erasedElements;
        private BitmapEraseSession _bitmapEraseSession;
        private Canvas _currentCanvas;

        public EraserTool(CommandService commandService)
        {
            Name = "Ластик";
            _commandService = commandService;
            _erasedElements = new List<UIElement>();
        }

        public override void OnMouseDown(Point position, Canvas canvas)
        {
            _isErasing = true;
            _lastPoint = position;
            _currentCanvas = canvas;
            _erasedElements.Clear();

            _bitmapEraseSession = new BitmapEraseSession(canvas);

            EraseAtPosition(position, canvas);
        }

        public override void OnMouseUp(Point position, Canvas canvas)
        {
            _isErasing = false;

            if (_erasedElements.Count > 0 || (_bitmapEraseSession != null && _bitmapEraseSession.HasChanges))
            {
                var eraseCommand = new BatchEraserCommand(
                    _erasedElements,
                    _bitmapEraseSession,
                    canvas
                );
                _commandService.CommandManager.Execute(eraseCommand);
            }

            _erasedElements.Clear();
            _bitmapEraseSession = null;
        }

        public override void OnMouseMove(Point position, Canvas canvas)
        {
            if (_isErasing && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                InterpolateErase(_lastPoint, position, canvas);
                _lastPoint = position;
            }
        }

        private void InterpolateErase(Point from, Point to, Canvas canvas)
        {
            double distance = Distance(from, to);
            int steps = Math.Max(1, (int)(distance / (EraserSize * 0.3)));

            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                Point interpolatedPoint = new Point(
                    from.X + (to.X - from.X) * t,
                    from.Y + (to.Y - from.Y) * t
                );
                EraseAtPosition(interpolatedPoint, canvas);
            }
        }

        private void EraseAtPosition(Point position, Canvas canvas)
        {
            var element = FindElementAtPosition(position, canvas);
            if (element != null && !IsSpecialElement(element))
            {
  
                if (!_erasedElements.Contains(element))
                {
                    _erasedElements.Add(element);
                    canvas.Children.Remove(element);
                }
                return;
            }

            if (_bitmapEraseSession != null)
            {
                _bitmapEraseSession.EraseAtPosition(position);
            }
        }

        private UIElement FindElementAtPosition(Point position, Canvas canvas)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                var element = canvas.Children[i];

                if (IsSpecialElement(element) || element is Image)
                    continue;

                if (IsElementAtPosition(element, position))
                {
                    return element;
                }
            }
            return null;
        }

        private bool IsElementAtPosition(UIElement element, Point position)
        {
            switch (element)
            {
                case Line line:
                    return IsLineAtPosition(line, position);
                case Ellipse ellipse:
                    return IsEllipseAtPosition(ellipse, position);
                case Rectangle rectangle:
                    return IsRectangleAtPosition(rectangle, position);
                case Polygon polygon:
                    return IsPolygonAtPosition(polygon, position);
                case Polyline polyline:
                    return IsPolylineAtPosition(polyline, position);
                case Path path:
                    return IsPathAtPosition(path, position);
                case TextBlock textBlock:
                    return IsTextBlockAtPosition(textBlock, position);
                case TextBox textBox:
                    return IsTextBoxAtPosition(textBox, position);
                case FrameworkElement frameworkElement:
                    return IsFrameworkElementAtPosition(frameworkElement, position);
                default:
                    return false;
            }
        }

        private bool IsLineAtPosition(Line line, Point position)
        {
            Point p1 = new Point(line.X1, line.Y1);
            Point p2 = new Point(line.X2, line.Y2);
            double distance = DistanceToLineSegment(position, p1, p2);
            double tolerance = Math.Max(line.StrokeThickness, 5);
            return distance <= tolerance;
        }

        private bool IsEllipseAtPosition(Ellipse ellipse, Point position)
        {
            double left = Canvas.GetLeft(ellipse);
            double top = Canvas.GetTop(ellipse);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;
            double centerX = left + ellipse.ActualWidth / 2;
            double centerY = top + ellipse.ActualHeight / 2;
            double radiusX = ellipse.ActualWidth / 2;
            double radiusY = ellipse.ActualHeight / 2;
            double normalizedX = (position.X - centerX) / radiusX;
            double normalizedY = (position.Y - centerY) / radiusY;
            return (normalizedX * normalizedX + normalizedY * normalizedY) <= 1;
        }

        private bool IsRectangleAtPosition(Rectangle rectangle, Point position)
        {
            double left = Canvas.GetLeft(rectangle);
            double top = Canvas.GetTop(rectangle);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;
            var bounds = new Rect(left, top, rectangle.ActualWidth, rectangle.ActualHeight);
            return bounds.Contains(position);
        }

        private bool IsFrameworkElementAtPosition(FrameworkElement element, Point position)
        {
            double left = Canvas.GetLeft(element);
            double top = Canvas.GetTop(element);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;
            var bounds = new Rect(left, top, element.ActualWidth, element.ActualHeight);
            return bounds.Contains(position);
        }

        private bool IsPolygonAtPosition(Polygon polygon, Point position)
        {
            return IsFrameworkElementAtPosition(polygon, position);
        }

        private bool IsPolylineAtPosition(Polyline polyline, Point position)
        {
            return IsFrameworkElementAtPosition(polyline, position);
        }

        private bool IsPathAtPosition(Path path, Point position)
        {
            return IsFrameworkElementAtPosition(path, position);
        }

        private bool IsTextBlockAtPosition(TextBlock textBlock, Point position)
        {
            return IsFrameworkElementAtPosition(textBlock, position);
        }

        private bool IsTextBoxAtPosition(TextBox textBox, Point position)
        {
            double left = Canvas.GetLeft(textBox);
            double top = Canvas.GetTop(textBox);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;
            var bounds = new Rect(left, top, textBox.ActualWidth, textBox.ActualHeight);
            return bounds.Contains(position);
        }

        private double DistanceToLineSegment(Point p, Point a, Point b)
        {
            double lengthSquared = (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
            if (lengthSquared == 0)
                return Distance(p, a);
            double t = Math.Max(0, Math.Min(1, ((p.X - a.X) * (b.X - a.X) + (p.Y - a.Y) * (b.Y - a.Y)) / lengthSquared));
            Point projection = new Point(a.X + t * (b.X - a.X), a.Y + t * (b.Y - a.Y));
            return Distance(p, projection);
        }

        private double Distance(Point a, Point b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        private bool IsSpecialElement(UIElement element)
        {
            return element is System.Windows.Controls.Primitives.Thumb ||
                   (element is FrameworkElement fe && fe.Tag?.ToString() == "selection");
        }
    }

    public class BitmapEraseSession
    {
        private readonly Canvas _canvas;
        private WriteableBitmap _originalBitmap;
        private WriteableBitmap _modifiedBitmap;
        private Image _targetImage;
        private bool _hasChanges = false;

        public bool HasChanges => _hasChanges;

        public BitmapEraseSession(Canvas canvas)
        {
            _canvas = canvas;
            InitializeBitmaps();
        }

        private void InitializeBitmaps()
        {
            foreach (var child in _canvas.Children)
            {
                if (child is Image image && image.Source is WriteableBitmap bitmap)
                {
                    _targetImage = image;
                    _originalBitmap = bitmap;

                    _modifiedBitmap = new WriteableBitmap(bitmap);
                    break;
                }
            }
        }

        public void EraseAtPosition(Point position)
        {
            if (_targetImage == null || _modifiedBitmap == null) return;

            double left = Canvas.GetLeft(_targetImage);
            double top = Canvas.GetTop(_targetImage);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            int bitmapX = (int)(position.X - left);
            int bitmapY = (int)(position.Y - top);

            if (bitmapX >= 0 && bitmapX < _modifiedBitmap.PixelWidth &&
                bitmapY >= 0 && bitmapY < _modifiedBitmap.PixelHeight)
            {
                EraseFromBitmap(_modifiedBitmap, bitmapX, bitmapY);
                _hasChanges = true;

                _targetImage.Source = _modifiedBitmap;
            }
        }

        public void ApplyFinalBitmap()
        {
            if (_hasChanges && _targetImage != null)
            {
                _targetImage.Source = _modifiedBitmap;
            }
        }

        public void RestoreOriginalBitmap()
        {
            if (_targetImage != null && _originalBitmap != null)
            {
                _targetImage.Source = _originalBitmap;
            }
        }

        private void EraseFromBitmap(WriteableBitmap bitmap, int centerX, int centerY)
        {
            try
            {
                bitmap.Lock();

                int radius = 10; 
                int startX = Math.Max(0, centerX - radius);
                int endX = Math.Min(bitmap.PixelWidth - 1, centerX + radius);
                int startY = Math.Max(0, centerY - radius);
                int endY = Math.Min(bitmap.PixelHeight - 1, centerY + radius);

                unsafe
                {
                    byte* buffer = (byte*)bitmap.BackBuffer.ToPointer();
                    int stride = bitmap.BackBufferStride;

                    for (int x = startX; x <= endX; x++)
                    {
                        for (int y = startY; y <= endY; y++)
                        {
                            double distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                            if (distance <= radius)
                            {
                                int index = y * stride + x * 4;
                                double opacity = 1.0 - (distance / radius);

                                if (buffer[index + 3] > 0)
                                {
                                    byte newAlpha = (byte)(buffer[index + 3] * (1.0 - opacity));
                                    buffer[index + 3] = newAlpha;

                                    if (newAlpha < 10)
                                    {
                                        buffer[index] = 0;
                                        buffer[index + 1] = 0;
                                        buffer[index + 2] = 0;
                                        buffer[index + 3] = 0;
                                    }
                                }
                            }
                        }
                    }
                }

                bitmap.AddDirtyRect(new Int32Rect(startX, startY, endX - startX + 1, endY - startY + 1));
                bitmap.Unlock();
            }
            catch
            {
                try { bitmap.Unlock(); } catch { }
            }
        }
    }

    public class BatchEraserCommand : Commands.ICommand
    {
        private readonly List<UIElement> _erasedElements;
        private readonly BitmapEraseSession _bitmapSession;
        private readonly Canvas _canvas;

        public BatchEraserCommand(List<UIElement> erasedElements, BitmapEraseSession bitmapSession, Canvas canvas)
        {
            _erasedElements = new List<UIElement>(erasedElements);
            _bitmapSession = bitmapSession;
            _canvas = canvas;
        }

        public void Execute()
        {
            foreach (var element in _erasedElements)
            {
                if (_canvas.Children.Contains(element))
                {
                    _canvas.Children.Remove(element);
                }
            }

            if (_bitmapSession != null && _bitmapSession.HasChanges)
            {
                _bitmapSession.ApplyFinalBitmap();
            }
        }

        public void Undo()
        {
            foreach (var element in _erasedElements)
            {
                if (!_canvas.Children.Contains(element))
                {
                    _canvas.Children.Add(element);
                }
            }

            if (_bitmapSession != null && _bitmapSession.HasChanges)
            {
                _bitmapSession.RestoreOriginalBitmap();
            }
        }
    }
}