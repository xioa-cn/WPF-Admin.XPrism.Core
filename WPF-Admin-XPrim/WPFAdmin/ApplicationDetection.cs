using System.Windows;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WPFAdmin;

public partial class App {
    private bool Detection {
        get
        {
            string? mName = System.Diagnostics.Process.GetCurrentProcess().MainModule?.ModuleName;
            string? pName = System.IO.Path.GetFileNameWithoutExtension(mName);

            return System.Diagnostics.Process.GetProcessesByName(pName).Length > 5;
        }
        
    }
    
    private void Detect() {
        if (Detection)
        {
            MessageBox.Show("本程序一次只能运行一个实例！", "提示");
            Application.Current.Shutdown();
            Environment.Exit(0);
            return;
        }
    }
}