using System.Windows;
using System.Windows.Controls;
using FlowModules.Models;
using WPF.Admin.Service.Services;

namespace FlowModules.Components;

public partial class FlowControl : UserControl {
    private void ClearAllNodes() {
        // 清除所有连接线
        foreach (var connection in _saveModel.Connections.ToList())
        {
            MainCanvas.Children.Remove(connection.Path);
        }

        connections.Clear();

        // 清除所有节点
        foreach (var node in _saveModel.Nodes.ToList())
        {
            MainCanvas.Children.Remove(node);
        }

        nowFlowNodes.Clear();

        // 重置状态
        selectedNode = null;
        startPort = null;
        isConnecting = false;
        if (tempConnectionLine != null)
        {
            MainCanvas.Children.Remove(tempConnectionLine);
            tempConnectionLine = null;
        }
    }

    private void LoadFileNodes() {
        ClearAllNodes();
        var dir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Node");
        if (!System.IO.Directory.Exists(dir)) return;

        var files = System.IO.Directory.GetFiles(dir, "*.json");
        foreach (var file in files)
        {
            var saveModel = ReadFileNodes(file);
            foreach (var node in saveModel.Nodes)
            {
                var n = AddNode(node.Position, false);
                for (int i = 0; i < node.InputPorts.Count(); i++)
                {
                    var newPort = new NodePort {
                        Id = node.InputPorts[i].Id,
                        Name = node.InputPorts[i].Name,
                        PortType = PortType.Input,
                        Position = new Point(0, 25 * (node.InputPorts.Count + 1)),
                        Node = n
                    };
                    n.InputPorts.Add(newPort);
                    var portElement = newPort as Control;
                    if (portElement != null)
                    {
                        portElement.MouseLeftButtonDown += Port_MouseLeftButtonDown;
                        portElement.MouseLeftButtonUp += Port_MouseLeftButtonUp;
                    }
                    //var con = saveModel.Connections.FirstOrDefault(e => e.EndPortId == newPort.Id);
                    //if (con is not null)
                    //{
                    //    con.StartPort = newPort;
                    //}
                }

                for (int i = 0; i < node.OutputPorts.Count(); i++)
                {
                    var newPort = new NodePort {
                        Id = node.OutputPorts[i].Id,
                        Name = node.OutputPorts[i].Name,
                        PortType = PortType.Input,
                        Position = new Point(0, 25 * (node.OutputPorts.Count + 1)),
                        Node = n
                    };
                    n.OutputPorts.Add(newPort);
                    var portElement = newPort as Control;
                    if (portElement != null)
                    {
                        portElement.MouseLeftButtonDown += Port_MouseLeftButtonDown;
                        portElement.MouseLeftButtonUp += Port_MouseLeftButtonUp;
                    }
                    //var con = saveModel.Connections.FirstOrDefault(e => e.StartPortId == newPort.Id);
                    //if (con is not null)
                    //{
                    //    con.EndPort = newPort;
                    //}
                }
            }
            MainCanvas.UpdateLayout();//强制更新布局
            isConnecting = true;
            // 创建连接
            foreach (var connection in saveModel.Connections)
            {
                var flowStartPort = FindNodePort(connection.StartPortId);
                var flowEndPort = FindNodePort(connection.EndPortId);
                if (flowStartPort is not null && flowEndPort is not null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        var connectionUi = AddConnection(flowStartPort, flowEndPort);
                        connectionUi.UpdatePath();
                    });
                }
            }

            isConnecting = false;
        }
    }

    private NodePort? FindNodePort(string portId) {
        foreach (var node in nowFlowNodes)
        {
            foreach (var nodeOutPort in node.OutputPorts)
            {
                if (nodeOutPort.Id == portId)
                {
                    return nodeOutPort;
                }
            }

            foreach (var nodeInPort in node.InputPorts)
            {
                if (nodeInPort.Id == portId)
                {
                    return nodeInPort;
                }
            }
        }

        return null;
    }
}