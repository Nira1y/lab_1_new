using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Models.Shapes
{
    public abstract class ShapeBase
    {
        protected readonly ColorService _colorService;

        public Brush Stroke => _colorService.CurrentColor;
        public Brush Fill => _colorService.CurrentColor; 
        public double StrokeThickness { get; set; } = 2;

        protected ShapeBase(ColorService colorService)
        {
            _colorService = colorService;
        }

        public abstract Shape CreateShape(Point startPoint, Point endPoint);
        public abstract void UpdateShape(Shape shape, Point startPoint, Point endPoint);
    }
}