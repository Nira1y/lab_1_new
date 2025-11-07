using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using lab_2_graphic_editor.Services;

namespace lab_2_graphic_editor.Services
{
    public class ColorChangeService
    {
        private readonly CommandService _commandService;
        private readonly SelectionService _selectionService;

        public ColorChangeService(CommandService commandService, SelectionService selectionService)
        {
            _commandService = commandService;
            _selectionService = selectionService;
        }

        public void ChangeStrokeColor(UIElement element, Color color)
        {
            if (element != null && element is Shape shape)
            {
                var newBrush = new SolidColorBrush(color);
                _commandService.ExecuteModifyStroke(element, shape.Stroke, newBrush);
                _selectionService.UpdateStrokeColor(element, color);
            }
        }

        public void ChangeFillColor(UIElement element, Color color)
        {
            if (element != null && element is Shape shape)
            {
                var newBrush = new SolidColorBrush(color);
                _commandService.ExecuteModifyFill(element, shape.Fill, newBrush);
                _selectionService.UpdateFillColor(element, color);
            }
        }

        public void ChangeTextColor(UIElement element, Color color)
        {
            if (element is TextBox textBox)
            {
                var newBrush = new SolidColorBrush(color);
                _commandService.ExecuteModifyForeground(element, textBox.Foreground, newBrush);
                textBox.Foreground = newBrush;
            }
        }

        public void ChangeStrokeOrTextColor(UIElement element, List<UIElement> selectionGroup, Color color)
        {
            if (element != null)
            {
                if (element is TextBox textBox)
                {
                    var newBrush = new SolidColorBrush(color);
                    _commandService.ExecuteModifyForeground(element, textBox.Foreground, newBrush);
                    textBox.Foreground = newBrush;
                }
                else if (element is Shape shape)
                {
                    var newBrush = new SolidColorBrush(color);
                    _commandService.ExecuteModifyStroke(element, shape.Stroke, newBrush);
                    shape.Stroke = newBrush;
                }
            }

            foreach (var groupElement in selectionGroup)
            {
                if (groupElement is TextBox textBox)
                {
                    var newBrush = new SolidColorBrush(color);
                    _commandService.ExecuteModifyForeground(groupElement, textBox.Foreground, newBrush);
                    textBox.Foreground = newBrush;
                }
                else if (groupElement is Shape shape)
                {
                    var newBrush = new SolidColorBrush(color);
                    _commandService.ExecuteModifyStroke(groupElement, shape.Stroke, newBrush);
                    shape.Stroke = newBrush;
                }
            }
        }
    }
}