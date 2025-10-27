using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Models.Shapes
{
    public abstract class ShapeBase
    {
        public Brush Stroke { get; set; } = Brushes.Black;
        public Brush Fill { get; set; } = Brushes.Black;
        public double StrokeThickness { get; private set; } = 2;

        public abstract Shape CreateShape(Point startPoint, Point endPoint);
        public abstract void UpdateShape(Shape shape, Point startPoint, Point endPoint);
    }
}
