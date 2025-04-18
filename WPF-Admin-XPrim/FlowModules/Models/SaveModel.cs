﻿namespace FlowModules.Models;

public class PutPort {
    public string Id { get; set; }
    public string Name { get; set; }
}
public class NodeSerializationModel
{
    public string Id { get; set; }
    public string Title { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public List<PutPort> InputPortIds { get; set; } = new();
    public List<PutPort> OutputPortIds { get; set; } = new();
}

public class ConnectionSerializationModel
{
    public string StartPortId { get; set; }
    public string EndPortId { get; set; }
}

public class FlowSerializationModel
{
    public List<NodeSerializationModel> Nodes { get; set; } = new();
    public List<ConnectionSerializationModel> Connections { get; set; } = new();
}
public class SaveModel {
    public List<FlowConnection> Connections { get; set; } = new();
    public List<FlowNode> Nodes { get; set; } = new();
}