using System.Windows.Controls;
using System.Windows;

namespace lab_2_graphic_editor.Commands
{
    public class AddElementCommand : ICommand
    {
        private readonly UIElement _element;
        private readonly Canvas _canvas;

        public AddElementCommand(UIElement element, Canvas canvas)
        {
            _element = element;
            _canvas = canvas;
        }

        public void Execute()
        {
            if (!_canvas.Children.Contains(_element))
            {
                _canvas.Children.Add(_element);
            }
        }

        public void Undo()
        {
            _canvas.Children.Remove(_element);
        }
    }
}