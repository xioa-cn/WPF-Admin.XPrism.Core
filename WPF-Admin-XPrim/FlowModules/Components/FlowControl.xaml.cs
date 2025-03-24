using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using FlowModules.Commands;
using FlowModules.Models;
using WPF.Admin.Themes.Controls;
using WPF.Admin.Themes.Helper;

namespace FlowModules.Components
{
    public partial class FlowControl : UserControl
    {
        private UndoRedoManager undoRedoManager;
        private FlowNode selectedNode;
        private NodePort startPort;
        private bool isConnecting;
        private Line tempConnectionLine;
        private SaveModel _saveModel = new SaveModel();

        private enum ToolMode
        {
            Select,
            Connect,
            AddNode
        }

        private ToolMode currentMode { get; set; } = ToolMode.Select;


        private void SetCurrentMode(ToolMode mode)
        {
            currentMode = mode;
            ModeViewText.Text = mode switch
            {
                ToolMode.AddNode => "添加节点",
                ToolMode.Select => "移动节点",
                ToolMode.Connect => "节点连线"
            };
        }

        private bool isRunning = false;

        public FlowControl()
        {
            InitializeComponent();
            undoRedoManager = new UndoRedoManager();
            _saveModel.Connections = connections;
            _saveModel.Nodes = nowFlowNodes;
            // 绑定工具栏按钮事件
            //var selectButton = this.FindName("SelectButton") as Button;
            //var connectButton = this.FindName("ConnectButton") as Button;
            //var nodeButton = this.FindName("NodeButton") as Button;
            //var runButton = this.FindName("RunButton") as Button;
            //var stopButton = this.FindName("StopButton") as Button;

            // 绑定画布事件
            MainCanvas.MouseLeftButtonDown += MainCanvas_MouseLeftButtonDown;

            //if (selectButton != null) selectButton.Click += SelectButton_Click;
            //if (connectButton != null) connectButton.Click += ConnectButton_Click;
            //if (nodeButton != null) nodeButton.Click += NodeButton_Click;
            //if (runButton != null) runButton.Click += RunButton_Click;
            //if (stopButton != null) stopButton.Click += StopButton_Click;

            // 绑定缩放滑块事件
            //ZoomSlider.ValueChanged += ZoomSlider_ValueChanged;

            // 添加鼠标滚轮缩放
            this.PreviewMouseWheel += FlowControl_PreviewMouseWheel;
            this.Loaded += KeywordLoaded;
            this.Unloaded += KeywordUnregister;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            currentMode = ToolMode.Select;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            currentMode = ToolMode.Connect;
        }

        private void NodeButton_Click(object sender, RoutedEventArgs e)
        {
            currentMode = ToolMode.AddNode;
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            isRunning = true;
            // TODO: 实现流程执行逻辑
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            isRunning = false;
            // TODO: 实现停止流程执行逻辑
        }

        public FlowNode AddNode(Point position, bool isStartPort = true)
        {
            var node = new FlowNode
            {
                Title = "新节点",
                Width = 150,
                Height = 100,
                Position = position
            };
            if (isStartPort)
            {
                // 添加默认的输入输出端口
                NodePort inputPort = new NodePort
                {
                    Name = "输入1",
                    Position = new Point(0, 25),
                    PortType = PortType.Input,
                    Node = node // 设置端口的Node引用
                };
                NodePort outputPort = new NodePort
                {
                    Name = "输出1",
                    Position = new Point(150, 25),
                    PortType = PortType.Output,
                    Node = node // 设置端口的Node引用
                };

                node.InputPorts.Add(inputPort);
                node.OutputPorts.Add(outputPort);
            }
            Canvas.SetLeft(node, position.X);
            Canvas.SetTop(node, position.Y);
            if (node != null)
            {
                var contextMenu = new ContextMenu();

                // 添加输入端口菜单项
                var addInputMenuItem = new MenuItem { Header = "添加输入端口" };
                addInputMenuItem.Click += (s, args) =>
                {
                    if (node.InputPorts.Count() > 2)
                    {
                        SnackbarHelper.Show("最多添加三个输入节点");
                        return;
                    }

                    var newPort = new NodePort
                    {
                        Name = $"输入{node.InputPorts.Count + 1}",
                        PortType = PortType.Input,
                        Position = new Point(0, 25 * (node.InputPorts.Count + 1)),
                        Node = node
                    };
                    node.InputPorts.Add(newPort);
                    var portElement = newPort as Control;
                    if (portElement != null)
                    {
                        portElement.MouseLeftButtonDown += Port_MouseLeftButtonDown;
                        portElement.MouseLeftButtonUp += Port_MouseLeftButtonUp;
                    }
                };
                contextMenu.Items.Add(addInputMenuItem);

                // 添加输出端口菜单项
                var addOutputMenuItem = new MenuItem { Header = "添加输出端口" };
                addOutputMenuItem.Click += (s, args) =>
                {
                    if (node.OutputPorts.Count() > 2)
                    {
                        SnackbarHelper.Show("最多添加三个输出节点");
                        return;
                    }

                    var newPort = new NodePort
                    {
                        Name = $"输出{node.OutputPorts.Count + 1}",
                        PortType = PortType.Output,
                        Position = new Point(150, 25 * (node.OutputPorts.Count + 1)),
                        Node = node
                    };
                    node.OutputPorts.Add(newPort);
                    var portElement = newPort as Control;
                    if (portElement != null)
                    {
                        portElement.MouseLeftButtonDown += Port_MouseLeftButtonDown;
                        portElement.MouseLeftButtonUp += Port_MouseLeftButtonUp;
                    }
                };
                contextMenu.Items.Add(addOutputMenuItem);

                // 添加节点编辑功能
                var nodeContentMenuItem = new MenuItem() { Header = "节点编辑" };
                nodeContentMenuItem.Click += (s, args) => { };
                contextMenu.Items.Add(nodeContentMenuItem);

                // 删除节点功能
                var deleteNodeMenuItem = new MenuItem { Header = "删除节点" };
                deleteNodeMenuItem.Click += (s, args) => { };
                contextMenu.Items.Add(deleteNodeMenuItem);

                node.ContextMenu = contextMenu;
            }

            MainCanvas.Children.Add(node);

            // 绑定节点事件
            node.MouseLeftButtonDown += Node_MouseLeftButtonDown;
            node.MouseMove += Node_MouseMove;
            node.MouseLeftButtonUp += Node_MouseLeftButtonUp;

            // 绑定端口事件
            foreach (var port in node.InputPorts.Concat(node.OutputPorts))
            {
                var portElement = port as Control;
                if (portElement != null)
                {
                    portElement.MouseLeftButtonDown += Port_MouseLeftButtonDown;
                    portElement.MouseLeftButtonUp += Port_MouseLeftButtonUp;
                }
            }

            // 记录节点
            nowFlowNodes.Add(node);
            return node;
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentMode == ToolMode.AddNode)
            {
                var position = e.GetPosition(MainCanvas);
                var command = new AddNodeCommand(this, position);

                undoRedoManager.ExecuteCommand(command);
            }
        }

        private void Node_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentMode == ToolMode.Select)
            {
                selectedNode = sender as FlowNode;
                if (selectedNode != null)
                {
                    selectedNode.CaptureMouse();
                    e.Handled = true;
                }
            }
        }


        private void Node_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (currentMode == ToolMode.Select && selectedNode != null)
            {
                selectedNode.ReleaseMouseCapture();
                selectedNode = null;
                e.Handled = true;
            }
        }

        private void Port_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentMode == ToolMode.Connect)
            {
                var port = sender as NodePort;
                if (port != null && !isConnecting)
                {
                    isConnecting = true;
                    startPort = port;

                    // 创建临时连接线
                    tempConnectionLine = new Line
                    {
                        Stroke = Brushes.LightGreen,
                        StrokeThickness = 2,
                        StrokeDashArray = new DoubleCollection { 4, 2 }
                    };
                    MainCanvas.Children.Add(tempConnectionLine);

                    // 设置初始位置
                    var startPortPosition = port.TranslatePoint(new Point(port.Width / 2, port.Height / 2), MainCanvas);
                    tempConnectionLine.X1 = startPortPosition.X;
                    tempConnectionLine.Y1 = startPortPosition.Y;
                    tempConnectionLine.X2 = startPortPosition.X;
                    tempConnectionLine.Y2 = startPortPosition.Y;

                    // 绑定鼠标移动事件
                    MainCanvas.MouseMove += MainCanvas_MouseMoveWhileConnecting;
                    e.Handled = true;
                }
            }
        }

        private void Port_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var endPort = sender as NodePort;
            bool isValidConnection = false;
            NodePort outputPort = null;
            NodePort inputPort = null;

            if (endPort != null && isConnecting && startPort != endPort)
            {
                // 验证端口类型
                isValidConnection = (startPort.PortType == PortType.Output && endPort.PortType == PortType.Input) ||
                                    (startPort.PortType == PortType.Input && endPort.PortType == PortType.Output);

                if (isValidConnection)
                {
                    // 确保输出端口在前，输入端口在后
                    outputPort = startPort.PortType == PortType.Output ? startPort : endPort;
                    inputPort = startPort.PortType == PortType.Input ? startPort : endPort;
                }
            }

            // 在成功创建永久连接线后再清理临时连接线
            if (isValidConnection && outputPort != null && inputPort != null && !outputPort.IsConnected &&
                !inputPort.IsConnected)
            {
                var connection = AddConnection(outputPort, inputPort);

              
            }

            // 清理临时连接线
            if (tempConnectionLine != null)
            {
                MainCanvas.Children.Remove(tempConnectionLine);
                tempConnectionLine = null;
                MainCanvas.MouseMove -= MainCanvas_MouseMoveWhileConnecting;
            }

            isConnecting = false;
            startPort = null;
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            undoRedoManager.Undo();
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            undoRedoManager.Redo();
        }

        private void MainCanvas_MouseMoveWhileConnecting(object sender, MouseEventArgs e)
        {
            if (isConnecting && tempConnectionLine != null)
            {
                var startPortPosition =
                    startPort.TranslatePoint(new Point(startPort.Width / 2, startPort.Height / 2), MainCanvas);
                var currentPosition = e.GetPosition(MainCanvas);

                tempConnectionLine.X1 = startPortPosition.X;
                tempConnectionLine.Y1 = startPortPosition.Y;
                tempConnectionLine.X2 = currentPosition.X;
                tempConnectionLine.Y2 = currentPosition.Y;
            }
        }

        private List<FlowConnection> connections { get; set; } = new List<FlowConnection>();
        private List<FlowNode> nowFlowNodes { get; set; } = new List<FlowNode>();

        public FlowConnection AddConnection(NodePort startPort, NodePort endPort)
        {
            var connection = new FlowConnection(startPort, endPort);

            // 创建连接线路径
            var pathGeometry = new PathGeometry();
            var pathFigure = new PathFigure();
            var bezierSegment = new BezierSegment();
            pathFigure.Segments.Add(bezierSegment);
            pathGeometry.Figures.Add(pathFigure);

            connection.Path = new Path
            {
                Data = pathGeometry,
                Stroke = connection.ConnectionColor,
                StrokeThickness = 2,
                Tag = connection
            };

            // 设置Z轴顺序，确保连接线显示在节点下方
            Panel.SetZIndex(connection.Path, -1);

            // 添加右键菜单
            var contextMenu = new ContextMenu();
            var deleteMenuItem = new MenuItem { Header = "删除连接" };
            deleteMenuItem.Click += (s, e) => DeleteConnection(connection);
            contextMenu.Items.Add(deleteMenuItem);
            connection.Path.ContextMenu = contextMenu;

            MainCanvas.Children.Add(connection.Path);
            connections.Add(connection);

            // 更新连接线路径
            connection.UpdatePath();
            // 更新连接状态
            startPort.IsConnected = true;
            endPort.IsConnected = true;
            return connection;
        }

        private void DeleteConnection(FlowConnection connection)
        {
            // 移除连接线
            MainCanvas.Children.Remove(connection.Path);
            connections.Remove(connection);

            // 重置端口连接状态
            connection.StartPort.IsConnected = false;
            connection.EndPort.IsConnected = false;
        }

        private void Node_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentMode == ToolMode.Select && selectedNode != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(MainCanvas);
                Canvas.SetLeft(selectedNode, position.X - selectedNode.Width / 2);
                Canvas.SetTop(selectedNode, position.Y - selectedNode.Height / 2);

                // 更新与该节点相关的所有连接线
                foreach (var child in MainCanvas.Children)
                {
                    if (child is Path path && path.Tag is FlowConnection connection)
                    {
                        if (connection.StartPort.Node == selectedNode ||
                            connection.EndPort.Node == selectedNode)
                        {
                            connection.UpdatePath();
                        }
                    }
                }

                e.Handled = true;
            }
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CanvasScale != null)
            {
                CanvasScale.ScaleX = e.NewValue;
                CanvasScale.ScaleY = e.NewValue;
                //ZoomText.Text = $"{e.NewValue:P0}";
            }
        }

        private void FlowControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                var delta = e.Delta * 0.001;
                // var newValue = ZoomSlider.Value + delta;
                //
                // // 限制缩放范围
                // newValue = Math.Max(ZoomSlider.Minimum, Math.Min(ZoomSlider.Maximum, newValue));
                // ZoomSlider.Value = newValue;
                if (CanvasScale != null)
                {
                    CanvasScale.ScaleX = CanvasScale.ScaleX + delta;
                    CanvasScale.ScaleY = CanvasScale.ScaleY + delta;
                }

                e.Handled = true;
            }
        }
    }
}