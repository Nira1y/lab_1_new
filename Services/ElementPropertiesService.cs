using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Services
{
    public class ElementPropertiesService
    {
        public Point GetElementPosition(UIElement element)
        {
            // Используем метод из ElementMovementService
            var movementService = new ElementMovementService();
            return movementService.GetElementPosition(element);
        }

        public double GetElementRotation(UIElement element)
        {
            if (element.RenderTransform is RotateTransform rotateTransform)
            {
                return rotateTransform.Angle;
            }
            return 0;
        }

        public Size GetElementSize(UIElement element)
        {
            if (element is FrameworkElement frameworkElement)
            {
                return new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight);
            }
            return new Size(0, 0);
        }

        public class ElementProperties
        {
            public Point OriginalPosition { get; set; }
            public double OriginalRotation { get; set; }
            public Size OriginalSize { get; set; }
            public Brush OriginalStroke { get; set; }
            public Brush OriginalFill { get; set; }
            public Brush OriginalForeground { get; set; }
        }

        public ElementProperties SaveOriginalProperties(UIElement element)
        {
            if (element == null) return null;

            return new ElementProperties
            {
                OriginalPosition = GetElementPosition(element),
                OriginalRotation = GetElementRotation(element),
                OriginalSize = GetElementSize(element),
                OriginalStroke = (element as Shape)?.Stroke,
                OriginalFill = (element as Shape)?.Fill,
                OriginalForeground = (element as TextBox)?.Foreground
            };
        }
    }
}