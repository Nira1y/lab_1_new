using System.Windows;
using System.Windows.Media;

namespace lab_2_graphic_editor.Commands
{
    public class ModifyRotationCommand : ICommand
    {
        private readonly UIElement _element;
        private readonly double _oldAngle;
        private readonly double _newAngle;

        public ModifyRotationCommand(UIElement element, double oldAngle, double newAngle)
        {
            _element = element;
            _oldAngle = oldAngle;
            _newAngle = newAngle;
        }

        public void Execute()
        {
            SetRotation(_newAngle);
        }

        public void Undo()
        {
            SetRotation(_oldAngle);
        }

        private void SetRotation(double angle)
        {
            if (_element.RenderTransform is RotateTransform rotateTransform)
            {
                rotateTransform.Angle = angle;
            }
            else
            {
                _element.RenderTransform = new RotateTransform(angle);
            }
        }
    }
}