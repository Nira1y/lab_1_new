using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace lab_2_graphic_editor.Models.Shapes
{
    public class RectangleShape : ShapeBase
    {

        public override Shape CreateShape(Point startPoint, Point endPoint)
        {
            var rect = new Rectangle
            {
                Stroke = Stroke,
                StrokeThickness = StrokeThickness
            };
            UpdatePosition(rect, startPoint, endPoint);
            return rect;
        }
        

        public override void UpdateShape(Shape shape, Point startPoint, Point endPoint)
        {
            if (shape is Rectangle rect) { 
                UpdatePosition(rect, startPoint, endPoint);
            }
        }
        public void UpdatePosition(Rectangle rect, Point startPoint, Point endPoint)
        {
            double left = Math.Min(startPoint.X, endPoint.X);
            double top = Math.Min(startPoint.Y, endPoint.Y);
            double height = Math.Abs(endPoint.Y - startPoint.Y);
            double width = Math.Abs(endPoint.X - startPoint.X);

            Canvas.SetLeft(rect, left);
            Canvas.SetTop(rect, top);
            rect.Width = width;
            rect.Height = height;
        }
        
    }
}
