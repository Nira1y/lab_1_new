using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using lab_2_graphic_editor.Commands;

namespace lab_2_graphic_editor.Commands
{
    public class ModifyElementCommand : ICommand
    {
        private readonly UIElement _element;
        private readonly object _oldValue;
        private readonly object _newValue;
        private readonly string _propertyName;

        public ModifyElementCommand(UIElement element, object oldValue, object newValue, string propertyName)
        {
            _element = element;
            _oldValue = oldValue;
            _newValue = newValue;
            _propertyName = propertyName;
        }

        public void Execute()
        {
            SetPropertyValue(_newValue);
        }

        public void Undo()
        {
            SetPropertyValue(_oldValue);
        }

        private void SetPropertyValue(object value)
        {
            switch (_propertyName)
            {
                case "Fill":
                    if (_element is Shape shape)
                        shape.Fill = value as Brush;
                    break;

                case "Stroke":
                    if (_element is Shape shapeWithStroke)
                        shapeWithStroke.Stroke = value as Brush;
                    break;

                case "StrokeThickness":
                    if (_element is Shape shapeWithThickness)
                        shapeWithThickness.StrokeThickness = (double)value;
                    break;

                case "Position":
                    var point = (Point)value;
                    if (_element is FrameworkElement frameworkElement)
                    {
                        Canvas.SetLeft(frameworkElement, point.X);
                        Canvas.SetTop(frameworkElement, point.Y);
                    }
                    break;

                case "Size":
                    var size = (Size)value;
                    if (_element is FrameworkElement resizableElement)
                    {
                        resizableElement.Width = size.Width;
                        resizableElement.Height = size.Height;
                    }
                    break;

                case "Foreground":
                    if (_element is TextBox textBox)
                        textBox.Foreground = value as Brush;
                    break;

                case "Rotation":
                    var angle = (double)value;
                    if (_element.RenderTransform is RotateTransform rotateTransform)
                    {
                        rotateTransform.Angle = angle;
                    }
                    else
                    {
                        _element.RenderTransform = new RotateTransform(angle);
                    }
                    break;
            }
        }
    }
}