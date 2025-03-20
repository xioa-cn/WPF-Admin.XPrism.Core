using System.IO;
using System.Text.Json.Serialization;
using WPF.Admin.Themes.Converter;

namespace WPFAdmin.Config;

public class Configs {
    public static Configs Default { get; }
    [JsonPropertyName("height")] public double Height { get; set; }
    [JsonPropertyName("width")] public double Width { get; set; }
    [JsonPropertyName("index")] public string? IndexStatus { get; set; }
    [JsonPropertyName("api")] public string? ApiBaseUrl { get; set; }
    [JsonPropertyName("auth")] public string? ViewAuthSwitch { get; set; }
    [JsonPropertyName("useSystemTheme")] public bool UseSystemTheme { get; set; }

    static Configs() {
        var settingJsonFile =
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appSettings.json");
        var settingJsonString = System.IO.File.ReadAllText(settingJsonFile);
        Default = System.Text.Json.JsonSerializer.Deserialize<Configs>(settingJsonString);
        if (Default == null)
            throw new FileNotFoundException(settingJsonFile);
        LoginAuthHelper.SetViewAuthSwitch(Default.ViewAuthSwitch);
    }
}