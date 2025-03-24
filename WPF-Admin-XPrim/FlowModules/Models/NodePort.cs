using System.Windows;
using System.Windows.Controls;

namespace FlowModules.Models;
public class NodePort : Control
{
    public static readonly DependencyProperty NameProperty =
        DependencyProperty.Register("Name", typeof(string), typeof(NodePort));

    public static readonly DependencyProperty IsConnectedProperty =
        DependencyProperty.Register("IsConnected", typeof(bool), typeof(NodePort));

    public static readonly DependencyProperty PortTypeProperty =
        DependencyProperty.Register("PortType", typeof(PortType), typeof(NodePort));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public Point Position { get; set; }
    public FlowNode Node { get; set; }

    public bool IsConnected
    {
        get => (bool)GetValue(IsConnectedProperty);
        set => SetValue(IsConnectedProperty, value);
    }

    public PortType PortType
    {
        get => (PortType)GetValue(PortTypeProperty);
        set => SetValue(PortTypeProperty, value);
    }

    static NodePort()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NodePort),
            new FrameworkPropertyMetadata(typeof(NodePort)));
    }

    public NodePort()
    {
        Width = 20;
        Height = 20;
        Name = "新端口";
        IsConnected = false;
        PortType = PortType.Input;
        IsHitTestVisible = true;
        PortType = PortType.Input; // 默认为输入端口
    }
}