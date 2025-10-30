using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Models.Shapes
{
    public class EllipseShape : ShapeBase
    {
        public bool WithFill { get; set; }

        public EllipseShape(bool withFill = false)
        {
            WithFill = withFill;
        }
        public override Shape CreateShape(Point startPoint, Point endPoint)
        {
            var ellipse = new Ellipse
            {
                Stroke = Stroke,
                StrokeThickness = StrokeThickness,
                Fill = WithFill ? Fill : Brushes.Transparent

            };
            UpdatePosition(ellipse, startPoint, endPoint);
            return ellipse;
        }

        public override void UpdateShape(Shape shape, Point startPoint, Point endPoint)
        {
            if (shape is Ellipse ellipse) 
            { 
                UpdatePosition(ellipse, startPoint, endPoint);
            }
        }

        public void UpdatePosition(Ellipse ellipse, Point startPoint, Point endPoint)
        {
            double left = Math.Min(startPoint.X, endPoint.X);
            double top = Math.Min(startPoint.Y, endPoint.Y);
            double height = Math.Abs(endPoint.Y - startPoint.Y);
            double width = Math.Abs(endPoint.X - startPoint.X);

            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);
            ellipse.Width = width;
            ellipse.Height = height;
        }
    }
}
