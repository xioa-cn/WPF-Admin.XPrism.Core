using System.Windows.Input;

namespace FlowModules.Commands;

public class UndoRedoManager
{
    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();

    public bool CanUndo => undoStack.Count > 0;
    public bool CanRedo => redoStack.Count > 0;

    public void ExecuteCommand(ICommand command)
    {
        if (command.CanExecute(null))
        {
            command.Execute(null);
            undoStack.Push(command);
            redoStack.Clear();
        }
    }

    public void Undo()
    {
        if (CanUndo)
        {
            ICommand command = undoStack.Pop();
            if (command is IUndoableCommand undoableCommand)
            {
                undoableCommand.UnExecute();
                redoStack.Push(command);
            }
        }
    }

    public void Redo()
    {
        if (CanRedo)
        {
            ICommand command = redoStack.Pop();
            command.Execute(null);
            undoStack.Push(command);
        }
    }
}

public interface IUndoableCommand : ICommand
{
    void UnExecute();
}