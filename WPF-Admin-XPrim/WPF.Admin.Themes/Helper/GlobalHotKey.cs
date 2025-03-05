using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WPF.Admin.Themes.Helper
{
    /// <summary>
    /// 全局热键管理类，用于在WPF应用程序中注册和处理全局热键
    /// </summary>
    public class GlobalHotKey
    {
        // 注册
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        // 卸载
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Alt 修饰键
        /// </summary>
        public const uint ModAlt = 0x0001;

        /// <summary>
        /// Ctrl 修饰键
        /// </summary>
        public const uint ModControl = 0x0002;

        /// <summary>
        /// Shift 修饰键
        /// </summary>
        public const uint ModShift = 0x0004;

        /// <summary>
        /// Windows 修饰键
        /// </summary>
        public const uint ModWin = 0x0008;

        /// <summary>
        /// 防止热键重复触发
        /// </summary>
        public const uint ModNorepeat = 0x4000;

        // 热键消息常量，当热键被按下时，系统会发送此消息
        private const int WmHotkey = 0x0312;

        // 热键ID计数器，用于生成唯一的热键ID
        private int _currentId;

        // 窗口句柄，用于注册热键
        private IntPtr _handle;

        // 消息源，用于接收窗口消息
        private HwndSource? _source;

        // 窗口引用
        private readonly Window _window;

        // 已注册的热键ID列表，用于跟踪和管理已注册的热键
        private readonly ConcurrentDictionary<int, Action> _registeredIds = new();

        // 标记热键管理器是否已初始化
        private bool _isInitialized;

        /// <summary>
        /// 创建全局热键管理器实例
        /// </summary>
        /// <param name="window">要关联的窗口</param>
        /// <param name="currentId">热键ID起始值，默认为0</param>
        public GlobalHotKey(Window window, int currentId = 0)
        {
            _currentId = currentId;
            _window = window;

            // 尝试多种方式初始化热键管理器
            TryInitialize();

            // 窗口关闭时自动注销所有热键
            _window.Closed += (_, _) => { UnregisterAllHotKeys(); };
        }

        /// <summary>
        /// 尝试通过多种方式初始化热键管理器
        /// </summary>
        private void TryInitialize()
        {
            // 如果窗口已加载，直接初始化
            if (_window.IsLoaded)
            {
                InitializeDirectly();
                return;
            }

            // 注册多个事件，确保能捕获到窗口初始化
            _window.Loaded += (_, _) =>
            {
                if (!_isInitialized)
                    InitializeDirectly();
            };

            _window.ContentRendered += (_, _) =>
            {
                if (!_isInitialized)
                    InitializeDirectly();
            };

            _window.SourceInitialized += (_, _) =>
            {
                if (!_isInitialized)
                    InitializeDirectly();
            };

            // 如果以上事件都没触发，使用延迟初始化作为备选方案
            Task.Delay(500).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!_isInitialized) InitializeDirectly();
                });
            });
        }

        /// <summary>
        /// 直接初始化热键管理器，获取窗口句柄并设置消息钩子
        /// </summary>
        private void InitializeDirectly()
        {
            try
            {
                if (_isInitialized) return;

                Debug.WriteLine("开始初始化热键...");

                // 获取窗口句柄
                var helper = new WindowInteropHelper(_window);
                _handle = helper.Handle;

                if (_handle == IntPtr.Zero)
                {
                    Console.WriteLine("窗口句柄为空，尝试强制创建句柄");
                    // 尝试强制创建句柄
                    helper.EnsureHandle();
                    _handle = helper.Handle;
                }

                if (_handle == IntPtr.Zero)
                {
                    throw new InvalidOperationException("无法获取窗口句柄");
                }

                Debug.WriteLine($"获取到窗口句柄: {_handle}");

                // 添加消息钩子
                _source = HwndSource.FromHwnd(_handle);
                if (_source == null)
                {
                    throw new InvalidOperationException("无法创建消息源");
                }

                _source.AddHook(WndProc);
                _isInitialized = true;
                Debug.WriteLine("成功添加消息钩子，热键初始化完成");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"初始化热键失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 注册全局热键
        /// </summary>
        /// <param name="modifiers">修饰键组合，如MOD_CONTROL | MOD_ALT</param>
        /// <param name="key">虚拟键码，如(uint)'A'表示A键</param>
        /// /// <param name="action">虚拟键码触发方法</param>
        /// <returns>注册成功的热键ID</returns>
        /// <exception cref="InvalidOperationException">注册失败时抛出异常</exception>
        public int RegisterHotKey(uint modifiers, uint key, Action action)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("热键管理器尚未初始化，请稍后再试");
            }

            if (_handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("窗口句柄无效");
            }

            _currentId++;
            if (!RegisterHotKey(_handle, _currentId, modifiers, key))
            {
                _currentId--;
                // 
                int error = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"无法注册热键，错误代码: {error}");
            }

            _registeredIds.TryAdd(_currentId, action);
            Debug.WriteLine($"成功注册热键: ID={_currentId}, Modifiers={modifiers}, Key={key}");
            return _currentId;
        }

        /// <summary>
        /// 注销指定ID的热键
        /// </summary>
        /// <param name="id">要注销的热键ID</param>
        /// <returns>注销是否成功</returns>
        public bool UnregisterHotKey(int id)
        {
            if (_handle == IntPtr.Zero) return false;

            bool result = UnregisterHotKey(_handle, id);
            if (result)
            {
                _registeredIds.Remove(id, out _);
                Debug.WriteLine($"成功注销热键: ID={id}");
            }

            return result;
        }

        /// <summary>
        /// 注销所有已注册的热键
        /// </summary>
        public void UnregisterAllHotKeys()
        {
            if (_handle == IntPtr.Zero) return;

            foreach (var id in _registeredIds)
            {
                UnregisterHotKey(_handle, id.Key);
            }

            _registeredIds.Clear();

            // 移除消息钩子
            if (_source != null)
            {
                _source.RemoveHook(WndProc);
                _source = null;
            }

            Debug.WriteLine("已注销所有热键");
        }

        /// <summary>
        /// 窗口消息处理函数，用于捕获热键消息
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="msg">消息ID</param>
        /// <param name="wParam">消息参数，热键ID</param>
        /// <param name="lParam">消息参数，包含按键信息</param>
        /// <param name="handled">是否已处理</param>
        /// <returns>消息处理结果</returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WmHotkey)
            {
                int id = wParam.ToInt32();
                Debug.WriteLine($"收到热键消息: ID={id}");
                var (_, value) = _registeredIds.FirstOrDefault(e => e.Key == id);
                if (value == null) return IntPtr.Zero;
                value.Invoke();
                handled = true;
                Debug.WriteLine($"处理热键: ID={id}");
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// 获取热键管理器是否已初始化
        /// </summary>
        public bool IsInitialized => _isInitialized;
    }
}
