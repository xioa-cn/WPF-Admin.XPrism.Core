using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using FlowModules.Models;
using WPF.Admin.Themes.Helper;

namespace FlowModules.Components;

// 定义一个名为FlowControl的公共类
public partial class FlowControl : UserControl {
    private GlobalHotKey _hotKeyManager;

    private void RegisterKeyword() {
        try
        {
            int id1 = _hotKeyManager.RegisterHotKey(
                GlobalHotKey.ModControl | GlobalHotKey.ModAlt,
                (uint)'S', () =>
                {
                    SetCurrentMode(ToolMode.Select);
                    SnackbarHelper.Show($"开启节点移动");
                });
            int id2 = _hotKeyManager.RegisterHotKey(
                GlobalHotKey.ModControl | GlobalHotKey.ModAlt,
                'A', () =>
                {
                    SetCurrentMode(ToolMode.AddNode);
                    SnackbarHelper.Show($"开启节点添加");
                });
            int id3 = _hotKeyManager.RegisterHotKey(
                GlobalHotKey.ModAlt,
                'F', () =>
                {
                    SetCurrentMode(currentMode = ToolMode.Connect);
                    SnackbarHelper.Show($"开启节点连线");
                });

            int id4 = _hotKeyManager.RegisterHotKey(
                GlobalHotKey.ModAlt,
                'W', () =>
                {
                    SaveNode();
                    SnackbarHelper.Show($"节点数据保存");
                });
            int id5 = _hotKeyManager.RegisterHotKey(
                GlobalHotKey.ModAlt,
                'R', () =>
                {
                    LoadFileNodes();
                    SnackbarHelper.Show($"节点读取");
                });
            int id6 = _hotKeyManager.RegisterHotKey(
                GlobalHotKey.ModAlt,
                'P', () =>
                {
                    ClearAllNodes();
                    SnackbarHelper.Show($"节点数据保存");
                });
        }
        catch (Exception ex)
        {
            SnackbarHelper.Show($"部分热键启用失败");
        }
    }

    private void UnregisterKeyword() {
        _hotKeyManager.UnregisterAllHotKeys();
    }

    private void KeywordLoaded(object sender, RoutedEventArgs e) {
        var window = Window.GetWindow(this);
        if (window is not null)
        {
            _hotKeyManager = new GlobalHotKey(window, 1);
            RegisterKeyword();
        }
    }

    private void KeywordUnregister(object sender, RoutedEventArgs e) {
        UnregisterKeyword();
    }

    private void SaveNode() {
        var serializationModel = new FlowSerializationModel();

        // 转换为可序列化的模型
        foreach (var node in this._saveModel.Nodes)
        {
            serializationModel.Nodes.Add(new NodeSerializationModel {
                Id = node.Id,
                Title = node.Title,
                X = node.Position.X,
                Y = node.Position.Y,
                InputPortIds = node.InputPorts.Select(p => new PutPort() {
                    Id = p.Id,
                    Name = p.Name
                }).ToList(),
                OutputPortIds = node.OutputPorts.Select(p => new PutPort() {
                    Id = p.Id,
                    Name = p.Name
                }).ToList()
            });
        }

        foreach (var conn in this._saveModel.Connections)
        {
            serializationModel.Connections.Add(new ConnectionSerializationModel {
                StartPortId = conn.StartPort.Id,
                EndPortId = conn.EndPort.Id
            });
        }

        var dir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Node");
        if (!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }


        var result = JsonSerializer.Serialize(serializationModel, _options);
        System.IO.File.WriteAllText(
            System.IO.Path.Combine(dir, $"{Guid.NewGuid()}.json"),
            result, _encoding);
    }

    private static readonly JsonSerializerOptions _options = new() {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) // 添加这行以支持所有Unicode字符(包括中文)
    };

    private static readonly Encoding _encoding = new UTF8Encoding(false); // 不使用 BOM 的 UTF8 编码


    private SaveModel ReadFileNodes(string path) {
        var saveModel = new SaveModel();
        var fileText = System.IO.File.ReadAllText(path);
        var readModel = JsonSerializer.Deserialize<FlowSerializationModel>(fileText, _options);

        if (readModel == null) return saveModel;

        // 首先创建所有节点
        foreach (var nodeModel in readModel.Nodes)
        {
            var node = new FlowNode {
                Id = nodeModel.Id,
                Title = nodeModel.Title,
                Position = new Point(nodeModel.X, nodeModel.Y)
            };

            // 创建输入端口
            foreach (var portId in nodeModel.InputPortIds)
            {
                node.InputPorts.Add(new NodePort { Id = portId.Id, Name = portId.Name });
            }

            // 创建输出端口
            foreach (var portId in nodeModel.OutputPortIds)
            {
                node.OutputPorts.Add(new NodePort { Id = portId.Id, Name = portId.Name });
            }

            saveModel.Nodes.Add(node);
        }

        // 创建连接
        foreach (var connModel in readModel.Connections)
        {
            // 查找起始和结束端口
            var startPort = saveModel.Nodes
                .SelectMany(n => n.OutputPorts)
                .FirstOrDefault(p => p.Id == connModel.StartPortId);

            var endPort = saveModel.Nodes
                .SelectMany(n => n.InputPorts)
                .FirstOrDefault(p => p.Id == connModel.EndPortId);

            if (startPort != null && endPort != null)
            {
                var connection = new FlowConnection(startPort, endPort);
                connection.StartPortId = connModel.StartPortId;
                connection.EndPortId = connModel.EndPortId;
                saveModel.Connections.Add(connection);
            }
        }

        return saveModel;
    }
}