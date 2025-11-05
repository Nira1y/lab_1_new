using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Services
{
    public class RotationService
    {
        public Point GetElementCenter(UIElement element)
        {
            if (element is Line line)
            {
                return new Point((line.X1 + line.X2) / 2, (line.Y1 + line.Y2) / 2);
            }
            else if (element is Polygon polygon)
            {
                return GetPolygonCenter(polygon);
            }

            Rect bounds = GetElementBounds(element);
            return new Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
        }

        public void RotateElement(UIElement element, Point center, Point startPoint, Point newPoint)
        {
            double startAngle = Math.Atan2(startPoint.Y - center.Y, startPoint.X - center.X);
            double newAngle = Math.Atan2(newPoint.Y - center.Y, newPoint.X - center.X);
            double rotationAngle = (newAngle - startAngle) * 180 / Math.PI;

            RotateTransform rotateTransform = GetOrCreateRotateTransform(element);

            if (element is Line || element is Polygon)
            {
                rotateTransform.CenterX = center.X;
                rotateTransform.CenterY = center.Y;
            }

            rotateTransform.Angle += rotationAngle;
        }

        private RotateTransform GetOrCreateRotateTransform(UIElement element)
        {
            if (element.RenderTransform is RotateTransform existingTransform)
                return existingTransform;

            if (element.RenderTransform is TransformGroup transformGroup)
            {
                foreach (var transform in transformGroup.Children)
                {
                    if (transform is RotateTransform rotateTransform)
                        return rotateTransform;
                }

                var newRotateTransform = new RotateTransform();
                transformGroup.Children.Add(newRotateTransform);
                return newRotateTransform;
            }

            var newTransform = new RotateTransform();
            element.RenderTransform = newTransform;

            if (!(element is Line) && !(element is Polygon))
            {
                element.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            return newTransform;
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

        private Rect GetElementBounds(UIElement element)
        {
            if (element is Shape shape)
            {
                if (shape is Line line)
                {
                    return new Rect(
                        Math.Min(line.X1, line.X2),
                        Math.Min(line.Y1, line.Y2),
                        Math.Abs(line.X2 - line.X1),
                        Math.Abs(line.Y2 - line.Y1)
                    );
                }
                else if (shape is Polygon polygon)
                {
                    return GetPolygonBounds(polygon);
                }
                else
                {
                    double left = Canvas.GetLeft(element);
                    double top = Canvas.GetTop(element);
                    double width = (element as FrameworkElement)?.Width ?? 0;
                    double height = (element as FrameworkElement)?.Height ?? 0;

                    return new Rect(left, top, width, height);
                }
            }

            return new Rect(0, 0, 0, 0);
        }

        private Rect GetPolygonBounds(Polygon polygon)
        {
            if (polygon.Points.Count == 0) return new Rect(0, 0, 0, 0);

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

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }
}