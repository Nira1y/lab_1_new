using System.Collections.Generic;

namespace lab_2_graphic_editor.Commands
{
    public class BatchCommand : ICommand
    {
        private readonly List<ICommand> _commands;

        public BatchCommand(List<ICommand> commands)
        {
            _commands = commands;
        }

        public void Execute()
        {
            foreach (var command in _commands)
            {
                command.Execute();
            }
        }

        public void Undo()
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                _commands[i].Undo();
            }
        }
    }
}