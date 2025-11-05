using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Services
{
    public class ResizeService
    {
        public enum ResizeHandle
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Top,
            Bottom,
            Left,
            Right
        }

        public Rect GetElementBounds(UIElement element)
        {
            if (element is Shape shape)
            {
                if (shape is Line line)
                {
                    Rect lineRect = new Rect(
                        Math.Min(line.X1, line.X2),
                        Math.Min(line.Y1, line.Y2),
                        Math.Abs(line.X2 - line.X1),
                        Math.Abs(line.Y2 - line.Y1)
                    );

                    // Для линий учитываем трансформацию только если она есть
                    if (line.RenderTransform != null && !line.RenderTransform.Value.IsIdentity)
                    {
                        return line.RenderTransform.TransformBounds(lineRect);
                    }

                    return lineRect;
                }
                else if (shape is Polygon polygon)
                {
                    // Для полигонов учитываем трансформацию только если она есть
                    Rect polygonRect = GetPolygonBoundsWithoutTransform(polygon);
                    if (polygon.RenderTransform != null && !polygon.RenderTransform.Value.IsIdentity)
                    {
                        return polygon.RenderTransform.TransformBounds(polygonRect);
                    }
                    return polygonRect;
                }
                else
                {
                    // Для обычных фигур (Rectangle, Ellipse) НЕ учитываем трансформацию
                    // чтобы рамка выделения не "улетала"
                    double left = Canvas.GetLeft(element);
                    double top = Canvas.GetTop(element);
                    double width = (element as FrameworkElement)?.Width ?? 0;
                    double height = (element as FrameworkElement)?.Height ?? 0;

                    return new Rect(left, top, width, height);
                }
            }

            return new Rect(0, 0, 0, 0);
        }

        private Rect GetPolygonBoundsWithoutTransform(Polygon polygon)
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


        public void ResizeElement(UIElement element, ResizeHandle handle, Point startPoint, Point newPoint)
        {
            if (element is Shape shape)
            {
                Rect originalBounds = GetElementBounds(element);
                double deltaX = newPoint.X - startPoint.X;
                double deltaY = newPoint.Y - startPoint.Y;

                switch (handle)
                {
                    case ResizeHandle.TopLeft:
                        ResizeTopLeft(shape, originalBounds, deltaX, deltaY);
                        break;
                    case ResizeHandle.TopRight:
                        ResizeTopRight(shape, originalBounds, deltaX, deltaY);
                        break;
                    case ResizeHandle.BottomLeft:
                        ResizeBottomLeft(shape, originalBounds, deltaX, deltaY);
                        break;
                    case ResizeHandle.BottomRight:
                        ResizeBottomRight(shape, originalBounds, deltaX, deltaY);
                        break;
                    case ResizeHandle.Top:
                        ResizeTop(shape, originalBounds, deltaY);
                        break;
                    case ResizeHandle.Bottom:
                        ResizeBottom(shape, originalBounds, deltaY);
                        break;
                    case ResizeHandle.Left:
                        ResizeLeft(shape, originalBounds, deltaX);
                        break;
                    case ResizeHandle.Right:
                        ResizeRight(shape, originalBounds, deltaX);
                        break;
                }
            }
        }

        private void ResizeTopLeft(Shape shape, Rect bounds, double deltaX, double deltaY)
        {
            double newWidth = Math.Max(1, bounds.Width - deltaX);
            double newHeight = Math.Max(1, bounds.Height - deltaY);

            if (shape is Line line)
            {
                line.X1 += deltaX;
                line.Y1 += deltaY;
            }
            else if (shape is Polygon polygon)
            {
                ResizePolygon(polygon, bounds, newWidth, newHeight, -deltaX, -deltaY);
            }
            else
            {
                Canvas.SetLeft(shape, bounds.Left + deltaX);
                Canvas.SetTop(shape, bounds.Top + deltaY);

                if (shape is FrameworkElement frameworkElement)
                {
                    frameworkElement.Width = newWidth;
                    frameworkElement.Height = newHeight;
                }
            }
        }

        private void ResizeTopRight(Shape shape, Rect bounds, double deltaX, double deltaY)
        {
            double newWidth = Math.Max(1, bounds.Width + deltaX);
            double newHeight = Math.Max(1, bounds.Height - deltaY);

            if (shape is Line line)
            {
                line.X2 += deltaX;
                line.Y1 += deltaY;
            }
            else if (shape is Polygon polygon)
            {
                ResizePolygon(polygon, bounds, newWidth, newHeight, 0, -deltaY);
            }
            else
            {
                Canvas.SetTop(shape, bounds.Top + deltaY);

                if (shape is FrameworkElement frameworkElement)
                {
                    frameworkElement.Width = newWidth;
                    frameworkElement.Height = newHeight;
                }
            }
        }

        private void ResizeBottomLeft(Shape shape, Rect bounds, double deltaX, double deltaY)
        {
            double newWidth = Math.Max(1, bounds.Width - deltaX);
            double newHeight = Math.Max(1, bounds.Height + deltaY);

            if (shape is Line line)
            {
                line.X1 += deltaX;
                line.Y2 += deltaY;
            }
            else if (shape is Polygon polygon)
            {
                ResizePolygon(polygon, bounds, newWidth, newHeight, -deltaX, 0);
            }
            else
            {
                Canvas.SetLeft(shape, bounds.Left + deltaX);

                if (shape is FrameworkElement frameworkElement)
                {
                    frameworkElement.Width = newWidth;
                    frameworkElement.Height = newHeight;
                }
            }
        }

        private void ResizeBottomRight(Shape shape, Rect bounds, double deltaX, double deltaY)
        {
            double newWidth = Math.Max(1, bounds.Width + deltaX);
            double newHeight = Math.Max(1, bounds.Height + deltaY);

            if (shape is Line line)
            {
                line.X2 += deltaX;
                line.Y2 += deltaY;
            }
            else if (shape is Polygon polygon)
            {
                ResizePolygon(polygon, bounds, newWidth, newHeight, 0, 0);
            }
            else if (shape is FrameworkElement frameworkElement)
            {
                frameworkElement.Width = newWidth;
                frameworkElement.Height = newHeight;
            }
        }

        private void ResizeTop(Shape shape, Rect bounds, double deltaY)
        {
            double newHeight = Math.Max(1, bounds.Height - deltaY);

            if (shape is Line line)
            {
                line.Y1 += deltaY;
            }
            else if (shape is Polygon polygon)
            {
                ResizePolygon(polygon, bounds, bounds.Width, newHeight, 0, -deltaY);
            }
            else
            {
                Canvas.SetTop(shape, bounds.Top + deltaY);

                if (shape is FrameworkElement frameworkElement)
                {
                    frameworkElement.Height = newHeight;
                }
            }
        }

        private void ResizeBottom(Shape shape, Rect bounds, double deltaY)
        {
            double newHeight = Math.Max(1, bounds.Height + deltaY);

            if (shape is Line line)
            {
                line.Y2 += deltaY;
            }
            else if (shape is Polygon polygon)
            {
                ResizePolygon(polygon, bounds, bounds.Width, newHeight, 0, 0);
            }
            else if (shape is FrameworkElement frameworkElement)
            {
                frameworkElement.Height = newHeight;
            }
        }

        private void ResizeLeft(Shape shape, Rect bounds, double deltaX)
        {
            double newWidth = Math.Max(1, bounds.Width - deltaX);

            if (shape is Line line)
            {
                line.X1 += deltaX;
            }
            else if (shape is Polygon polygon)
            {
                ResizePolygon(polygon, bounds, newWidth, bounds.Height, -deltaX, 0);
            }
            else
            {
                Canvas.SetLeft(shape, bounds.Left + deltaX);

                if (shape is FrameworkElement frameworkElement)
                {
                    frameworkElement.Width = newWidth;
                }
            }
        }

        private void ResizeRight(Shape shape, Rect bounds, double deltaX)
        {
            double newWidth = Math.Max(1, bounds.Width + deltaX);

            if (shape is Line line)
            {
                line.X2 += deltaX;
            }
            else if (shape is Polygon polygon)
            {
                ResizePolygon(polygon, bounds, newWidth, bounds.Height, 0, 0);
            }
            else if (shape is FrameworkElement frameworkElement)
            {
                frameworkElement.Width = newWidth;
            }
        }

        private void ResizePolygon(Polygon polygon, Rect originalBounds, double newWidth, double newHeight, double offsetX, double offsetY)
        {
            PointCollection newPoints = new PointCollection();
            Point center = GetPolygonCenter(polygon);

            foreach (Point point in polygon.Points)
            {
                double scaleX = newWidth / originalBounds.Width;
                double scaleY = newHeight / originalBounds.Height;

                double newX = center.X + (point.X - center.X) * scaleX + offsetX;
                double newY = center.Y + (point.Y - center.Y) * scaleY + offsetY;

                newPoints.Add(new Point(newX, newY));
            }

            polygon.Points = newPoints;
        }

        public Point GetPolygonCenter(Polygon polygon)
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
    }
}