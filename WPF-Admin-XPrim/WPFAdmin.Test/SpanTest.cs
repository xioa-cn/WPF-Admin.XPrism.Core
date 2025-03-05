using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace WPFAdmin.Test;

/// <summary>
/// 演示 C# 高性能编程技术的测试类，包括 Span、栈分配、数组池、SIMD 和跨平台电池状态检测
/// </summary>
public class SpanTest {
    /// <summary>
    /// 测试 Span&lt;T&gt; 的基本用法，包括创建、遍历和切片操作
    /// </summary>
    [Fact]
    public void Test01() {
        // Span<T> 是一个高性能的引用类型，提供对连续内存的安全访问
        byte[] arr = new byte[100];
        Span<byte> span = new Span<byte>(arr);

        string text = "hello world";
        ReadOnlySpan<char> spant = text.AsSpan();
        foreach (var item in spant)
        {
            Debug.WriteLine(item);
        }

        var first = spant.Slice(0, 5);
        Debug.WriteLine(first.ToString());
    }

    /// <summary>
    /// 演示如何使用 MemoryMarshal 进行类型转换，避免内存复制
    /// </summary>
    public void ProcessBinaryData() {
        byte[] data = new byte[100];
        Span<byte> span = data;

        // 直接读取不同类型的数据
        Span<int> intSpan = MemoryMarshal.Cast<byte, int>(span);
    }

    /// <summary>
    /// 测试栈分配内存的用法，避免堆分配和垃圾回收压力
    /// </summary>
    [Fact]
    public void Test02() {
        //在栈上分配内存，避免堆分配和GC压力
        Span<int> num = stackalloc int[100];
        for (int i = 0; i < 100; i++)
        {
            num[i] = i;
        }

        unsafe
        {
            //在栈上分配内存，避免堆分配和GC压力
            int* por = stackalloc int[100];
            for (int i = 0; i < 100; i++)
            {
                por[i] = i;
            }
        }
    }

    /// <summary>
    /// 测试 ArrayPool 的使用，通过对象池复用数组减少内存分配
    /// </summary>
    [Fact]
    public void Test03() {
        // 从共享的数组池中租用一个长度为102的字节数组
        byte[] bytes = ArrayPool<byte>.Shared.Rent(102);

        try
        {
            // 在try块中执行代码
        }
        finally
        {
            // 在finally块中返回数组到共享的数组池中
            ArrayPool<byte>.Shared.Return(bytes);
        }
    }

    /// <summary>
    /// 使用普通循环方式对数组进行加法操作
    /// </summary>
    /// <param name="a">第一个浮点数组</param>
    /// <param name="b">第二个浮点数组</param>
    /// <param name="result">结果数组</param>
    static void AddArraysRegular(float[] a, float[] b, float[] result) {
        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i] + b[i];
        }
    }

    /// <summary>
    /// 使用 SIMD 向量化方式对数组进行加法操作，提高计算性能
    /// </summary>
    /// <param name="a">第一个浮点数组</param>
    /// <param name="b">第二个浮点数组</param>
    /// <param name="result">结果数组</param>
    static void AddArraysVector(float[] a, float[] b, float[] result) {
        int vectorSize = Vector<float>.Count;
        int i = 0;

        // 处理可以被向量大小整除的部分
        for (; i <= a.Length - vectorSize; i += vectorSize)
        {
            Vector<float> va = new Vector<float>(a, i);
            Vector<float> vb = new Vector<float>(b, i);
            result[i] = va[0] + vb[0];
        }

        // 处理剩余元素
        for (; i < a.Length; i++)
        {
            result[i] = a[i] + b[i];
        }
    }

    /// <summary>
    /// 测试比较普通循环和 SIMD 向量化计算的性能差异
    /// </summary>
    [Fact]
    public void Test04() {
        const int ArraySize = 100_000_00;
        float[] a = new float[ArraySize];
        float[] b = new float[ArraySize];
        float[] result1 = new float[ArraySize];
        float[] result2 = new float[ArraySize];

        // 初始化数组
        for (int i = 0; i < ArraySize; i++)
        {
            a[i] = i * 0.1f;
            b[i] = i * 0.2f;
        }

        // 使用普通循环
        Stopwatch sw = Stopwatch.StartNew();
        AddArraysRegular(a, b, result1);
        sw.Stop();
        Console.WriteLine($"普通循环耗时: {sw.ElapsedMilliseconds} ms");

        // 使用 SIMD 向量
        sw.Restart();
        AddArraysVector(a, b, result2);
        sw.Stop();
        Console.WriteLine($"SIMD 向量耗时: {sw.ElapsedMilliseconds} ms");

        // 验证结果一致性
        bool resultsMatch = true;
        for (int i = 0; i < ArraySize; i++)
        {
            if (Math.Abs(result1[i] - result2[i]) > 0.0001f)
            {
                resultsMatch = false;
                break;
            }
        }

        Console.WriteLine($"结果一致: {resultsMatch}");
    }

    /// <summary>
    /// 测试跨平台获取电池信息的功能
    /// </summary>
    [Fact]
    public void Test05() {
        // 获取电池信息
        var batteryInfo = GetBatteryInfo();

        // 输出电池信息
        Console.WriteLine($"电池电量: {batteryInfo.BatteryLevel}%");
        Console.WriteLine($"是否正在充电: {batteryInfo.IsCharging}");
    }

    /// <summary>
    /// 电池信息结构，包含电量和充电状态
    /// </summary>
    public struct BatteryInfo {
        /// <summary>
        /// 电池电量百分比，范围0-100，-1表示获取失败
        /// </summary>
        public int BatteryLevel;
        
        /// <summary>
        /// 是否正在充电
        /// </summary>
        public bool IsCharging;
    }

    /// <summary>
    /// 获取电池信息的跨平台方法，支持Windows、Linux和macOS
    /// </summary>
    /// <returns>包含电池电量和充电状态的BatteryInfo结构</returns>
    public BatteryInfo GetBatteryInfo() {
        BatteryInfo info = new BatteryInfo();

        if (OperatingSystem.IsWindows())
        {
            // Windows平台实现
            GetWindowsBatteryInfo(ref info);
        }
        else if (OperatingSystem.IsLinux())
        {
            // Linux平台实现
            GetLinuxBatteryInfo(ref info);
        }
        else if (OperatingSystem.IsMacOS())
        {
            // macOS平台实现
            GetMacOSBatteryInfo(ref info);
        }
        else
        {
            // 默认实现或不支持的平台
            info.BatteryLevel = -1;
            info.IsCharging = false;
            Console.WriteLine("当前平台不支持获取电池信息");
        }

        return info;
    }

    /// <summary>
    /// Windows平台获取电池信息的实现，使用Windows API
    /// </summary>
    /// <param name="info">要填充的电池信息结构</param>
    private void GetWindowsBatteryInfo(ref BatteryInfo info)
    {
        // Windows API调用
        SYSTEM_POWER_STATUS status = new SYSTEM_POWER_STATUS();
        if (GetSystemPowerStatus(ref status))
        {
            // 电池电量
            info.BatteryLevel = status.BatteryLifePercent;
            
            // 充电状态
            // ACLineStatus: 0 = 离线, 1 = 在线, 255 = 未知
            // BatteryFlag: 8 = 充电中
            info.IsCharging = (status.ACLineStatus == 1) || ((status.BatteryFlag & 8) != 0);
        }
    }

    /// <summary>
    /// Windows电源状态结构，用于P/Invoke调用
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct SYSTEM_POWER_STATUS
    {
        /// <summary>AC电源状态: 0=离线, 1=在线, 255=未知</summary>
        public byte ACLineStatus;
        /// <summary>电池状态标志: 1=高, 2=低, 4=极低, 8=充电中, 128=无电池</summary>
        public byte BatteryFlag;
        /// <summary>电池电量百分比: 0-100, 255=未知</summary>
        public byte BatteryLifePercent;
        /// <summary>保留字段</summary>
        public byte Reserved1;
        /// <summary>剩余电池寿命(秒)</summary>
        public int BatteryLifeTime;
        /// <summary>满电电池寿命(秒)</summary>
        public int BatteryFullLifeTime;
    }

    /// <summary>
    /// Windows API函数声明，用于获取系统电源状态
    /// </summary>
    /// <param name="lpSystemPowerStatus">电源状态结构引用</param>
    /// <returns>操作是否成功</returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetSystemPowerStatus(ref SYSTEM_POWER_STATUS lpSystemPowerStatus);

    /// <summary>
    /// Linux平台获取电池信息的实现，通过读取系统文件
    /// </summary>
    /// <param name="info">要填充的电池信息结构</param>
    private void GetLinuxBatteryInfo(ref BatteryInfo info)
    {
        try
        {
            // 在Linux上，电池信息通常在/sys/class/power_supply/目录下
            string batteryPath = "/sys/class/power_supply/BAT0";
            
            // 读取电池电量
            if (File.Exists($"{batteryPath}/capacity"))
            {
                string capacityStr = File.ReadAllText($"{batteryPath}/capacity").Trim();
                if (int.TryParse(capacityStr, out int capacity))
                {
                    info.BatteryLevel = capacity;
                }
            }
            
            // 读取充电状态
            if (File.Exists($"{batteryPath}/status"))
            {
                string status = File.ReadAllText($"{batteryPath}/status").Trim();
                info.IsCharging = status.Equals("Charging", StringComparison.OrdinalIgnoreCase);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取Linux电池信息失败: {ex.Message}");
            info.BatteryLevel = -1;
        }
    }

    /// <summary>
    /// macOS平台获取电池信息的实现，通过执行shell命令
    /// </summary>
    /// <param name="info">要填充的电池信息结构</param>
    private void GetMacOSBatteryInfo(ref BatteryInfo info)
    {
        // 在macOS上，我们可以使用IOKit框架，但这需要更复杂的P/Invoke
        // 这里使用简化的实现，实际应用中可能需要更完整的IOKit调用
        try
        {
            // 使用shell命令获取电池信息
            Process process = new Process();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = "-c \"pmset -g batt\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            
            // 解析输出
            // 输出格式类似于: "Now drawing from 'Battery Power'\n -InternalBattery-0 (id=12345)\t95%; discharging; 4:21 remaining"
            if (output.Contains("%"))
            {
                // 提取电池电量
                int percentIndex = output.IndexOf('%');
                int startIndex = output.LastIndexOf('\t', percentIndex) + 1;
                string percentStr = output.Substring(startIndex, percentIndex - startIndex);
                if (int.TryParse(percentStr, out int percent))
                {
                    info.BatteryLevel = percent;
                }
                
                // 检查充电状态
                info.IsCharging = output.Contains("charging") && !output.Contains("discharging");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取macOS电池信息失败: {ex.Message}");
            info.BatteryLevel = -1;
        }
    }
}