using lab_2_graphic_editor.Commands;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab_2_graphic_editor.Services
{
    public class CommandService
    {
        private readonly CommandManager _commandManager = new CommandManager();

        public CommandManager CommandManager => _commandManager;

        public Action<object, object> CanExecuteChanged { get; internal set; }

        public void ExecuteAddElement(UIElement element, Canvas canvas)
        {
            var command = new AddElementCommand(element, canvas);
            _commandManager.Execute(command);
        }

        public void ExecuteRemoveElement(UIElement element, Canvas canvas)
        {
            var command = new RemoveElementCommand(element, canvas);
            _commandManager.Execute(command);
        }

        public void ExecuteModifyFill(UIElement element, Brush oldFill, Brush newFill)
        {
            var command = new ModifyElementCommand(element, oldFill, newFill, "Fill");
            _commandManager.Execute(command);
        }

        public void ExecuteModifyStroke(UIElement element, Brush oldStroke, Brush newStroke)
        {
            var command = new ModifyElementCommand(element, oldStroke, newStroke, "Stroke");
            _commandManager.Execute(command);
        }

        public void ExecuteModifyPosition(UIElement element, Point oldPosition, Point newPosition)
        {
            var command = new ModifyElementCommand(element, oldPosition, newPosition, "Position");
            _commandManager.Execute(command);
        }

        public void ExecuteModifySize(UIElement element, Size oldSize, Size newSize)
        {
            var command = new ModifyElementCommand(element, oldSize, newSize, "Size");
            _commandManager.Execute(command);
        }

        public void ExecuteModifyForeground(UIElement element, Brush oldForeground, Brush newForeground)
        {
            var command = new ModifyElementCommand(element, oldForeground, newForeground, "Foreground");
            _commandManager.Execute(command);
        }

        public void ExecuteBatchCommand(List<ICommand> commands)
        {
            var batchCommand = new BatchCommand(commands);
            _commandManager.Execute(batchCommand);
        }

        public void Undo()
        {
            _commandManager.Undo();
        }

        public void Redo()
        {
            _commandManager.Redo();
        }

        public void ClearHistory()
        {
            _commandManager.Clear();
        }
    }
}