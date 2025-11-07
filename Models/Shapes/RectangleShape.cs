using lab_2_graphic_editor.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Models.Shapes
{
    public class RectangleShape : ShapeBase
    {
        public bool WithFill { get; set; }

        public RectangleShape(ColorService colorService, bool withFill = false) : base(colorService)
        {
            WithFill = withFill;
        }

        public override Shape CreateShape(Point startPoint, Point endPoint)
        {
            var rect = new Rectangle
            {
                Stroke = Stroke,
                StrokeThickness = StrokeThickness,
                Fill = WithFill ? Fill : Brushes.Transparent
            };
            UpdatePosition(rect, startPoint, endPoint);
            return rect;
        }

        public override void UpdateShape(Shape shape, Point startPoint, Point endPoint)
        {
            if (shape is Rectangle rect)
            {
                UpdatePosition(rect, startPoint, endPoint);
            }
        }

        private void UpdatePosition(Rectangle rect, Point startPoint, Point endPoint)
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