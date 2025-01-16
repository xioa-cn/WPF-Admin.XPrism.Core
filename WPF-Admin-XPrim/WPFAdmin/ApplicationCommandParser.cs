using WPF.Admin.Models.Models;
using WPFAdmin.ViewModels;
using WPFAdmin.Views;

namespace WPFAdmin;

public partial class App {
    private CommandParser _commandLine;

    private void EnableDebugMode() {
        // 调试模式逻辑
    }

    private ApplicationStartupMode StartupCommandLine(string[]? args) {
        if (args is null || args.Length < 1)
            return ApplicationStartupMode.Normal;
        // --debug  --width 1024 --height 768 --maximize true
        _commandLine = new CommandParser(args);
        if (!_commandLine.HasParameter("debug")) return ApplicationStartupMode.Normal;

        var config = _commandLine.GetValue("config", "");
        if (config == "window")
        {
            ConfigWindow configWindow = new ConfigWindow();
            configWindow.ShowDialog();
        }

        EnableDebugMode();
        return ApplicationStartupMode.Debug;
    }
}