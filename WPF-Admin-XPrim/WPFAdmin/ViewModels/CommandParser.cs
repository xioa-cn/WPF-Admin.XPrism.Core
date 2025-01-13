namespace WPFAdmin.ViewModels;

public class CommandParser {
    private readonly Dictionary<string, string?> _parameters;

    public CommandParser(string[] args) {
        _parameters = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        ParseArguments(args);
    }

    private void ParseArguments(string[] args) {
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (!arg.StartsWith($"-") && !arg.StartsWith("--")) continue;
            var key = arg.TrimStart('-');
            string? value = null;

            // 检查下一个参数是否是值
            if (i + 1 < args.Length && !args[i + 1].StartsWith($"-"))
            {
                value = args[++i];
            }

            _parameters[key] = value ?? "true";
        }
    }

    public bool HasParameter(string name) {
        return _parameters.ContainsKey(name);
    }

    public string? GetValue(string name, string? defaultValue = null) {
        return _parameters.GetValueOrDefault(name, defaultValue);
    }

    public int GetIntValue(string name, int defaultValue = 0) {
        if (_parameters.TryGetValue(name, out var value) &&
            int.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    public bool GetBoolValue(string name, bool defaultValue = false) {
        if (!_parameters.TryGetValue(name, out var value)) return defaultValue;
        return value.Equals("true", StringComparison.OrdinalIgnoreCase);

    }
    
    
    public float GetFloatValue(string name, float defaultValue = 0.0f)
    {
        if (_parameters.TryGetValue(name, out var value) && 
            float.TryParse(value, out var result))
        {
            return result;
        }
        return defaultValue;
    }

    public T GetEnumValue<T>(string name, T defaultValue) where T : struct
    {
        if (_parameters.TryGetValue(name, out var value) && 
            Enum.TryParse<T>(value, true, out T result))
        {
            return result;
        }
        return defaultValue;
    }

    public IEnumerable<string> GetValues(string name)
    {
        return _parameters.Keys.Where(k => k.StartsWith(name + "["));
    }
}