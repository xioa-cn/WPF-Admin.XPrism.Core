using System.Windows.Controls;
using FlowModules.Components;
using FlowModules.Models;

namespace FlowModules.Commands;
public class AddConnectionCommand : IUndoableCommand
{
    private readonly FlowControl flowControl;
    private readonly NodePort startPort;
    private readonly NodePort endPort;
    private FlowConnection connection;

    public AddConnectionCommand(FlowControl flowControl, NodePort startPort, NodePort endPort)
    {
        this.flowControl = flowControl;
        this.startPort = startPort;
        this.endPort = endPort;
    }

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter)
    {
        // 使用FlowControl的AddConnection方法创建连接
        connection = flowControl.AddConnection(startPort, endPort);
        
        // 更新端口连接状态
        startPort.IsConnected = true;
        endPort.IsConnected = true;
    }

    public void UnExecute()
    {
        if (connection != null)
        {
            // 移除连接线
            var canvas = connection.Path.Parent as Canvas;
            if (canvas != null)
            {
                canvas.Children.Remove(connection.Path);
            }
            
            // 重置端口连接状态
            startPort.IsConnected = false;
            endPort.IsConnected = false;
        }
    }

    public event EventHandler CanExecuteChanged;
}