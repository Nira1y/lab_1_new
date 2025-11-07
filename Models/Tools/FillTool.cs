using lab_2_graphic_editor.Commands;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Models.Tools
{
    public class FillTool : Tool
    {
        private readonly ColorService _colorService;
        private readonly CommandService _commandService;
        private const int ColorTolerance = 50;

        public FillTool(ColorService colorService, CommandService commandService)
        {
            Name = "Заливка";
            _colorService = colorService;
            _commandService = commandService;
        }

        public override void OnMouseDown(Point position, Canvas canvas)
        {
            if (canvas == null) return;

            UIElement elementUnderCursor = FindElementAtPosition(position, canvas);

            if (elementUnderCursor != null)
            {
                ApplyFillToElement(elementUnderCursor);
                return;
            }
            ApplyBitmapFill(position, canvas);
        }

        public override void OnMouseMove(Point position, Canvas canvas) { }
        public override void OnMouseUp(Point position, Canvas canvas) { }

        private void ApplyFillToElement(UIElement element)
        {
            var fillColor = _colorService.CurrentColor;

            switch (element)
            {
                case Rectangle rectangle:
                    var oldRectFill = rectangle.Fill;
                    rectangle.Fill = fillColor;
                    _commandService.ExecuteModifyFill(element, oldRectFill, fillColor);
                    break;

                case Ellipse ellipse:
                    var oldEllipseFill = ellipse.Fill;
                    ellipse.Fill = fillColor;
                    _commandService.ExecuteModifyFill(element, oldEllipseFill, fillColor);
                    break;

                case Polygon polygon:
                    var oldPolygonFill = polygon.Fill;
                    polygon.Fill = fillColor;
                    _commandService.ExecuteModifyFill(element, oldPolygonFill, fillColor);
                    break;

                case Line line:
                    var oldLineStroke = line.Stroke;
                    line.Stroke = fillColor;
                    _commandService.ExecuteModifyStroke(element, oldLineStroke, fillColor);
                    break;

                case TextBox textBox:
                    var oldTextBoxBackground = textBox.Background;
                    textBox.Background = fillColor;
                    _commandService.ExecuteModifyFill(element, oldTextBoxBackground, fillColor);
                    break;

                case Path path:
                    var oldPathFill = path.Fill;
                    path.Fill = fillColor;
                    _commandService.ExecuteModifyFill(element, oldPathFill, fillColor);
                    break;
            }
        }

        private void ApplyBitmapFill(Point position, Canvas canvas)
        {
            BitmapSource originalBitmap = RenderCanvasToBitmap(canvas);

            Color targetColor = GetPixelColor(originalBitmap, position);
            Color baseFillColor = (_colorService.CurrentColor as SolidColorBrush)?.Color ?? Colors.Black;
            WriteableBitmap filledBitmap = new WriteableBitmap(originalBitmap);
            FloodFill(filledBitmap, (int)position.X, (int)position.Y, targetColor, baseFillColor);

            var fillCommand = new BitmapFillCommand(canvas, originalBitmap, filledBitmap, position);
            _commandService.CommandManager.Execute(fillCommand);
        }

        private BitmapSource RenderCanvasToBitmap(Canvas canvas)
        {
            try
            {
                int width = (int)canvas.ActualWidth;
                int height = (int)canvas.ActualHeight;
                if (width <= 0 || height <= 0) return null;

                var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(canvas);
                return rtb;
            }
            catch
            {
                return null;
            }
        }

        private Color GetPixelColor(BitmapSource bitmap, Point position)
        {
            if (bitmap == null) return Colors.Transparent;

            int x = (int)Math.Clamp(position.X, 0, bitmap.PixelWidth - 1);
            int y = (int)Math.Clamp(position.Y, 0, bitmap.PixelHeight - 1);

            var pixels = new byte[4];
            try
            {
                bitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
            }
            catch
            {
                return Colors.Transparent;
            }

            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        private void FloodFill(WriteableBitmap bmp, int x, int y, Color targetColor, Color fillColor)
        {
            int width = bmp.PixelWidth;
            int height = bmp.PixelHeight;
            var visited = new bool[height * width];
            var queue = new Queue<Point>();

            if (!ColorEquals(targetColor, GetPixelFromBitmap(bmp, x, y), ColorTolerance)) return;

            queue.Enqueue(new Point(x, y));

            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();
                int px = (int)p.X;
                int py = (int)p.Y;
                int idx = py * width + px;

                if (px < 0 || py < 0 || px >= width || py >= height) continue;
                if (visited[idx]) continue;

                Color current = GetPixelFromBitmap(bmp, px, py);
                if (!ColorEquals(current, targetColor, ColorTolerance)) continue;

                SetPixelInBitmap(bmp, px, py, fillColor);
                visited[idx] = true;
                queue.Enqueue(new Point(px + 1, py));
                queue.Enqueue(new Point(px - 1, py));
                queue.Enqueue(new Point(px, py + 1));
                queue.Enqueue(new Point(px, py - 1));
            }
        }

        private Color GetPixelFromBitmap(WriteableBitmap bmp, int x, int y)
        {
            var pixels = new byte[4];
            bmp.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        private void SetPixelInBitmap(WriteableBitmap bmp, int x, int y, Color color)
        {
            try
            {
                bmp.Lock();
                unsafe
                {
                    byte* buffer = (byte*)bmp.BackBuffer.ToPointer();
                    int stride = bmp.BackBufferStride;
                    int index = y * stride + x * 4;

                    buffer[index] = color.B;
                    buffer[index + 1] = color.G;
                    buffer[index + 2] = color.R;
                    buffer[index + 3] = color.A;
                }
                bmp.AddDirtyRect(new Int32Rect(x, y, 1, 1));
                bmp.Unlock();
            }
            catch
            {
                bmp.Unlock();
            }
        }

        private bool ColorEquals(Color a, Color b, int tolerance)
        {
            return Math.Abs(a.A - b.A) <= tolerance &&
                   Math.Abs(a.R - b.R) <= tolerance &&
                   Math.Abs(a.G - b.G) <= tolerance &&
                   Math.Abs(a.B - b.B) <= tolerance;
        }

        private UIElement FindElementAtPosition(Point position, Canvas canvas)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                UIElement element = canvas.Children[i];

                if (IsCanvasBackground(element))
                    continue;

                if (IsElementAtPosition(element, position) && IsFillableElement(element))
                {
                    return element;
                }
            }
            return null;
        }

        private bool IsCanvasBackground(UIElement element)
        {
            if (element is Rectangle rect)
            {
                if (rect.StrokeThickness == 0 &&
                    rect.Width >= Application.Current.MainWindow.Width - 100 &&
                    rect.Height >= Application.Current.MainWindow.Height - 100)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsElementAtPosition(UIElement element, Point position)
        {
            switch (element)
            {
                case Shape shape when shape is Line line:
                    return IsLineAtPosition(line, position);
                case Shape shape:
                    return IsShapeAtPosition(shape, position);
                case TextBox textBox:
                    return IsTextBoxAtPosition(textBox, position);
                default:
                    return false;
            }
        }

        private bool IsShapeAtPosition(Shape shape, Point position)
        {
            double left = Canvas.GetLeft(shape);
            double top = Canvas.GetTop(shape);

            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            var bounds = new Rect(left, top, shape.ActualWidth, shape.ActualHeight);
            return bounds.Contains(position);
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

        private bool IsLineAtPosition(Line line, Point position)
        {
            Point p1 = new Point(line.X1, line.Y1);
            Point p2 = new Point(line.X2, line.Y2);

            double distance = DistanceToLineSegment(position, p1, p2);
            return distance <= Math.Max(line.StrokeThickness, 5);
        }

        private double DistanceToLineSegment(Point p, Point a, Point b)
        {
            double lengthSquared = (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);

            if (lengthSquared == 0)
                return Distance(p, a);

            double t = Math.Max(0, Math.Min(1, ((p.X - a.X) * (b.X - a.X) + (p.Y - a.Y) * (b.Y - a.Y)) / lengthSquared));

            Point projection = new Point(
                a.X + t * (b.X - a.X),
                a.Y + t * (b.Y - a.Y)
            );

            return Distance(p, projection);
        }

        private double Distance(Point a, Point b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        private bool IsFillableElement(UIElement element)
        {
            return element is Shape || element is TextBox;
        }
    }
}