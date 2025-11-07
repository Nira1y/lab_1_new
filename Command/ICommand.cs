namespace lab_2_graphic_editor.Commands
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}