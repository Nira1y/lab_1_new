using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Services
{
    public class GroupingService
    {
        public Canvas GroupElements(Canvas canvas, List<UIElement> elements)
        {
            if (elements.Count < 2) return null;

            var groupContainer = new Canvas();
            groupContainer.Background = Brushes.Transparent;

            Rect totalBounds = GetTotalBounds(elements);

            foreach (var element in elements)
            {
                UIElement clonedElement = CloneElement(element);
                if (clonedElement != null)
                {

                    Point relativePosition = GetRelativePosition(element, totalBounds);

                    Canvas.SetLeft(clonedElement, relativePosition.X);
                    Canvas.SetTop(clonedElement, relativePosition.Y);

                    groupContainer.Children.Add(clonedElement);
                }
            }

            foreach (var element in elements)
            {
                canvas.Children.Remove(element);
            }

            Canvas.SetLeft(groupContainer, totalBounds.Left);
            Canvas.SetTop(groupContainer, totalBounds.Top);
            groupContainer.Width = totalBounds.Width;
            groupContainer.Height = totalBounds.Height;

            canvas.Children.Add(groupContainer);
            return groupContainer;
        }

        private Rect GetTotalBounds(List<UIElement> elements)
        {
            if (elements.Count == 0) return new Rect(0, 0, 0, 0);

            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;

            foreach (var element in elements)
            {
                Rect bounds = GetElementBounds(element);

                minX = Math.Min(minX, bounds.Left);
                maxX = Math.Max(maxX, bounds.Right);
                minY = Math.Min(minY, bounds.Top);
                maxY = Math.Max(maxY, bounds.Bottom);
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private Point GetRelativePosition(UIElement element, Rect totalBounds)
        {
            Rect elementBounds = GetElementBounds(element);
            return new Point(
                elementBounds.Left - totalBounds.Left,
                elementBounds.Top - totalBounds.Top
            );
        }

        private UIElement CloneElement(UIElement original)
        {
            if (original is Rectangle rect)
            {
                return new Rectangle
                {
                    Width = rect.Width,
                    Height = rect.Height,
                    Fill = rect.Fill,
                    Stroke = rect.Stroke,
                    StrokeThickness = rect.StrokeThickness,
                    StrokeDashArray = rect.StrokeDashArray?.Clone(),
                    RenderTransform = rect.RenderTransform?.Clone()
                };
            }
            else if (original is Ellipse ellipse)
            {
                return new Ellipse
                {
                    Width = ellipse.Width,
                    Height = ellipse.Height,
                    Fill = ellipse.Fill,
                    Stroke = ellipse.Stroke,
                    StrokeThickness = ellipse.StrokeThickness,
                    StrokeDashArray = ellipse.StrokeDashArray?.Clone(),
                    RenderTransform = ellipse.RenderTransform?.Clone()
                };
            }
            else if (original is Line line)
            {
                double left = Canvas.GetLeft(line);
                double top = Canvas.GetTop(line);

                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                return new Line
                {
                    X1 = line.X1 - left,
                    Y1 = line.Y1 - top,
                    X2 = line.X2 - left,
                    Y2 = line.Y2 - top,
                    Stroke = line.Stroke,
                    StrokeThickness = line.StrokeThickness,
                    StrokeDashArray = line.StrokeDashArray?.Clone(),
                    RenderTransform = line.RenderTransform?.Clone()
                };
            }
            else if (original is Polygon polygon)
            {
                var newPolygon = new Polygon
                {
                    Fill = polygon.Fill,
                    Stroke = polygon.Stroke,
                    StrokeThickness = polygon.StrokeThickness,
                    StrokeDashArray = polygon.StrokeDashArray?.Clone(),
                    RenderTransform = polygon.RenderTransform?.Clone()
                };

                double left = Canvas.GetLeft(polygon);
                double top = Canvas.GetTop(polygon);

                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                var newPoints = new PointCollection();
                foreach (Point point in polygon.Points)
                {
                    newPoints.Add(new Point(point.X - left, point.Y - top));
                }
                newPolygon.Points = newPoints;

                return newPolygon;
            }
            else if (original is TextBox textBox)
            {
                return new TextBox
                {
                    Text = textBox.Text,
                    FontFamily = textBox.FontFamily,
                    FontSize = textBox.FontSize,
                    FontWeight = textBox.FontWeight,
                    FontStyle = textBox.FontStyle,
                    Foreground = textBox.Foreground,
                    Background = textBox.Background,
                    Width = textBox.Width,
                    Height = textBox.Height,
                    TextWrapping = textBox.TextWrapping,
                    RenderTransform = textBox.RenderTransform?.Clone()
                };
            }

            return null;
        }

        public void UngroupElements(Canvas canvas, Canvas groupContainer)
        {
            if (canvas == null || groupContainer == null) return;

            double groupLeft = Canvas.GetLeft(groupContainer);
            double groupTop = Canvas.GetTop(groupContainer);

            groupLeft = double.IsNaN(groupLeft) ? 0 : groupLeft;
            groupTop = double.IsNaN(groupTop) ? 0 : groupTop;

            var elementsToUngroup = new List<UIElement>();
            foreach (UIElement element in groupContainer.Children)
            {
                elementsToUngroup.Add(element);
            }

            groupContainer.Children.Clear();
            canvas.Children.Remove(groupContainer);

            foreach (var element in elementsToUngroup)
            {
                double localLeft = Canvas.GetLeft(element);
                double localTop = Canvas.GetTop(element);

                localLeft = double.IsNaN(localLeft) ? 0 : localLeft;
                localTop = double.IsNaN(localTop) ? 0 : localTop;


                Canvas.SetLeft(element, groupLeft + localLeft);
                Canvas.SetTop(element, groupTop + localTop);

                canvas.Children.Add(element);
            }
        }

        private Rect GetElementBounds(UIElement element)
        {
            if (element is Line line)
            {
                double x1 = line.X1;
                double y1 = line.Y1;
                double x2 = line.X2;
                double y2 = line.Y2;

                double left = Canvas.GetLeft(line);
                double top = Canvas.GetTop(line);

                if (!double.IsNaN(left))
                {
                    x1 += left;
                    x2 += left;
                }
                if (!double.IsNaN(top))
                {
                    y1 += top;
                    y2 += top;
                }

                return new Rect(
                    Math.Min(x1, x2),
                    Math.Min(y1, y2),
                    Math.Abs(x2 - x1),
                    Math.Abs(y2 - y1)
                );
            }
            else if (element is Polygon polygon)
            {
                if (polygon.Points.Count == 0)
                    return new Rect(0, 0, 0, 0);

                double minX = double.MaxValue;
                double maxX = double.MinValue;
                double minY = double.MaxValue;
                double maxY = double.MinValue;

                double left = Canvas.GetLeft(polygon);
                double top = Canvas.GetTop(polygon);

                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                foreach (Point point in polygon.Points)
                {
                    double x = point.X + left;
                    double y = point.Y + top;

                    minX = Math.Min(minX, x);
                    maxX = Math.Max(maxX, x);
                    minY = Math.Min(minY, y);
                    maxY = Math.Max(maxY, y);
                }

                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
            else
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                double width = (element as FrameworkElement)?.ActualWidth ?? 0;
                double height = (element as FrameworkElement)?.ActualHeight ?? 0;

                return new Rect(
                    double.IsNaN(left) ? 0 : left,
                    double.IsNaN(top) ? 0 : top,
                    width,
                    height
                );
            }
        }

    }
}