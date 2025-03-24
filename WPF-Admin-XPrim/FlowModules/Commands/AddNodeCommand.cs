
using FlowModules.Components;
using FlowModules.Models;
using System.Windows;

namespace FlowModules.Commands;
public class AddNodeCommand : IUndoableCommand
{
    private readonly FlowControl flowControl;
    private readonly Point position;
    private FlowNode addedNode;

    public AddNodeCommand(FlowControl flowControl, Point position)
    {
        this.flowControl = flowControl;
        this.position = position;
    }

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter)
    {
        addedNode = flowControl.AddNode(position);
    }

    public void UnExecute()
    {
        if (addedNode != null)
        {
            flowControl.MainCanvas.Children.Remove(addedNode);
        }
    }

    public event EventHandler CanExecuteChanged;
}