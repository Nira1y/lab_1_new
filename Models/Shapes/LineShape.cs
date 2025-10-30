using lab_2_graphic_editor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Models.Shapes
{
    public class LineShape : ShapeBase
    {
        public LineShape(ColorService colorService) : base(colorService)
        {
        }

        public override Shape CreateShape(Point startPoint, Point endPoint)
        {
            return new Line 
            { 
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                Stroke = Stroke,
                StrokeThickness = StrokeThickness,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
        }

        public override void UpdateShape(Shape shape, Point startPoint, Point endPoint)
        {
            if (shape is Line line) 
            {
                line.X1 = startPoint.X;
                line.Y1 = startPoint.Y;
                line.X2 = endPoint.X;
                line.Y2 = endPoint.Y;
            }
        }
    }
}
