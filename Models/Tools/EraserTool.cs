// EraserTool.cs
using lab_2_graphic_editor.Models.Tools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Tools
{
    public class EraserTool : Tool
    {
        public EraserTool()
        {
            Name = "Ластик";
        }

        public override void OnMouseDown(Point position, Canvas canvas)
        {

            RemoveShapeAtPosition(position, canvas);
        }

        public override void OnMouseUp(Point position, Canvas canvas)
        {
        }

        public override void OnMouseMove(Point position, Canvas canvas)
        {

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                RemoveShapeAtPosition(position, canvas);
            }
        }

        private void RemoveShapeAtPosition(Point position, Canvas canvas)
        {
 
            var element = FindElementAtPosition(position, canvas);

            if (element != null && !IsSpecialElement(element))
            {
                canvas.Children.Remove(element);
            }
        }

        private UIElement FindElementAtPosition(Point position, Canvas canvas)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                var element = canvas.Children[i];

                if (IsElementAtPosition(element, position))
                {
                    return element;
                }
            }
            return null;
        }

        private bool IsElementAtPosition(UIElement element, Point position)
        {
            if (IsSpecialElement(element))
                return false;

  
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
            double left = (double)ellipse.GetValue(Canvas.LeftProperty);
            double top = (double)ellipse.GetValue(Canvas.TopProperty);
            double width = ellipse.ActualWidth;
            double height = ellipse.ActualHeight;

            double centerX = left + width / 2;
            double centerY = top + height / 2;
            double radiusX = width / 2;
            double radiusY = height / 2;

            double normalizedX = (position.X - centerX) / radiusX;
            double normalizedY = (position.Y - centerY) / radiusY;

            return (normalizedX * normalizedX + normalizedY * normalizedY) <= 1;
        }

        private bool IsRectangleAtPosition(Rectangle rectangle, Point position)
        {
            double left = (double)rectangle.GetValue(Canvas.LeftProperty);
            double top = (double)rectangle.GetValue(Canvas.TopProperty);

            var bounds = new Rect(left, top, rectangle.ActualWidth, rectangle.ActualHeight);
            return bounds.Contains(position);
        }

        private bool IsFrameworkElementAtPosition(FrameworkElement element, Point position)
        {
            double left = (double)element.GetValue(Canvas.LeftProperty);
            double top = (double)element.GetValue(Canvas.TopProperty);

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

        private bool IsSpecialElement(UIElement element)
        {
            return element is System.Windows.Controls.Primitives.Thumb ||
                   (element is FrameworkElement fe && fe.Tag?.ToString() == "selection"); 
        }
    }
}