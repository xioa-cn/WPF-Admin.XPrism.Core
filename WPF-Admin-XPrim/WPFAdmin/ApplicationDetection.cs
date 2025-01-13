namespace WPFAdmin;

public partial class App {
    private bool Detection {
        get
        {
            string? mName = System.Diagnostics.Process.GetCurrentProcess().MainModule?.ModuleName;
            string? pName = System.IO.Path.GetFileNameWithoutExtension(mName);

            return System.Diagnostics.Process.GetProcessesByName(pName).Length > 1;
        }
        
    }
}