using System.Collections.ObjectModel;
using System.Runtime.InteropServices.JavaScript;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlowModules.Models
{
    public class FlowNode : Control
    {
        public string Id { get; set; }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(FlowNode));
            
        public static readonly DependencyProperty InputPortsProperty =
            DependencyProperty.Register("InputPorts", typeof(ObservableCollection<NodePort>), typeof(FlowNode));
            
        public static readonly DependencyProperty OutputPortsProperty =
            DependencyProperty.Register("OutputPorts", typeof(ObservableCollection<NodePort>), typeof(FlowNode));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ObservableCollection<NodePort> InputPorts
        {
            get => (ObservableCollection<NodePort>)GetValue(InputPortsProperty);
            set => SetValue(InputPortsProperty, value);
        }

        public ObservableCollection<NodePort> OutputPorts
        {
            get => (ObservableCollection<NodePort>)GetValue(OutputPortsProperty);
            set => SetValue(OutputPortsProperty, value);
        }

        public Point Position { get; set; }

        static FlowNode()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlowNode), 
                new FrameworkPropertyMetadata(typeof(FlowNode)));
        }

        public FlowNode() {
            Width = 150;
            Height = 100;
            Id =  Guid.NewGuid().ToString();
            InputPorts = new ObservableCollection<NodePort>();
            OutputPorts = new ObservableCollection<NodePort>();
        }

        public void AddInputPort(NodePort port)
        {
            port.Node = this;  // 设置端口的Node引用
            InputPorts.Add(port);
        }

        public void AddOutputPort(NodePort port)
        {
            port.Node = this;  // 设置端口的Node引用
            OutputPorts.Add(port);
        }
    }

    public enum PortType
    {
        Input,
        Output
    }

   
}