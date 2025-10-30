using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media;

namespace lab_2_graphic_editor.Models.Shapes
{
    public class TriangleShape : ShapeBase
    {
        public bool WithFill {  get; set; }

        public TriangleShape(bool withFill = false) 
        { 
            WithFill = withFill;
        }
        public override Shape CreateShape(Point startPoint, Point endPoint)
        {
            var polygon = new Polygon
            {
                Stroke = Stroke,
                StrokeThickness = StrokeThickness,
                Fill = WithFill ? Fill : Brushes.Transparent
            };

            UpdatePoints(polygon, startPoint, endPoint);
            return polygon;
        }


        public override void UpdateShape(Shape shape, Point startPoint, Point endPoint)
        {
            if (shape is Polygon polygon)
            {
                UpdatePoints(polygon, startPoint, endPoint);
            }
        }
        private void UpdatePoints(Polygon polygon, Point startPoint, Point endPoint)
        {
            double width = endPoint.X - startPoint.X;
            double height = endPoint.Y - startPoint.Y;

            var points = new PointCollection
            {
                new Point(startPoint.X + width / 2, startPoint.Y),        
                new Point(startPoint.X, startPoint.Y + height),            
                new Point(startPoint.X + width, startPoint.Y + height)    
            };

            polygon.Points = points;
        }
    }
}
