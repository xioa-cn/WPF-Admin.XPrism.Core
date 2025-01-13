using System.Text.Json.Serialization;

namespace WPF.Admin.Models.Models;

public class Router {
    [JsonPropertyName("Routers")] public List<TreeItemModel> Routers { get; set; }
}