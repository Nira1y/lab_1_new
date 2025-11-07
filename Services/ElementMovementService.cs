using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Services
{
    public class ElementMovementService
    {
        public Point GetElementPosition(UIElement element)
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

        public void MoveElementToPosition(UIElement element, double newX, double newY)
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

        public Point RotatePoint(Point point, double angle)
        {
            double angleRad = angle * Math.PI / 180.0;
            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);

            return new Point(
                point.X * cos - point.Y * sin,
                point.X * sin + point.Y * cos
            );
        }
    }
}