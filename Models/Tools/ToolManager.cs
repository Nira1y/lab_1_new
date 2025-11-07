using System.Windows;
using System.Windows.Controls;
using lab_2_graphic_editor.Models.Tools;

namespace lab_2_graphic_editor.Models.Tools
{
    public class ToolManager
    {
        private Tool _currentTool;

        public Tool CurrentTool
        {
            get => _currentTool;
            set => _currentTool = value;
        }

        public void HandleMouseDown(Point position, Canvas canvas)
        {
            _currentTool?.OnMouseDown(position, canvas);
        }

        public void HandleMouseMove(Point position, Canvas canvas)
        {
            _currentTool?.OnMouseMove(position, canvas);
        }

        public void HandleMouseUp(Point position, Canvas canvas)
        {
            _currentTool?.OnMouseUp(position, canvas);
        }
    }
}