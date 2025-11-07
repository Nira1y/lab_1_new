using System.Windows.Controls;
using System.Windows;

namespace lab_2_graphic_editor.Commands
{
    public class RemoveElementCommand : ICommand
    {
        private readonly UIElement _element;
        private readonly Canvas _canvas;
        private readonly int _originalIndex;

        public RemoveElementCommand(UIElement element, Canvas canvas)
        {
            _element = element;
            _canvas = canvas;
            _originalIndex = canvas.Children.IndexOf(element);
        }

        public void Execute()
        {
            _canvas.Children.Remove(_element);
        }

        public void Undo()
        {
            if (_originalIndex >= 0 && _originalIndex <= _canvas.Children.Count)
            {
                _canvas.Children.Insert(_originalIndex, _element);
            }
            else
            {
                _canvas.Children.Add(_element);
            }
        }
    }
}