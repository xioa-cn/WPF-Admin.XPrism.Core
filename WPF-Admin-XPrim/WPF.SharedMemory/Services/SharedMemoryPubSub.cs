using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using WPF.SharedMemory.Model;

namespace WPF.SharedMemory.Services;

public class SharedMemoryPubSub : IDisposable {
    // 常量定义
    private const int MAX_MESSAGES = 100;
    private const int MESSAGE_SIZE = 1040;
    private const int HEADER_WRITE_POS = 0; // 4 bytes
    private const int HEADER_READ_POS = 4; // 4 bytes
    private const int HEADER_MSG_COUNT = 8; // 4 bytes
    private const int HEADER_MSG_ID = 12;   // 4 bytes
    private const int HEADER_SIZE = 16;     // 总头部大小更新为16
    // 字段
    private readonly string _channelName;
    private readonly MemoryMappedFile _mmf;
    private readonly MemoryMappedViewAccessor _accessor;
    private readonly EventWaitHandle _newMessageEvent;
    private readonly Mutex _mutex;
    private readonly ConcurrentDictionary<int, List<Action<SharedMessage>>> _subscribers;
    private readonly CancellationTokenSource _cts;
    private readonly Task _messageLoop;
    private int _messageId;
    private bool _disposed;
   
   

    public SharedMemoryPubSub(string name) {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        _channelName = name;
        Debug.WriteLine($"[{_channelName}] 初始化开始");
       
        try
        {
            // 创建命名互斥锁
            _mutex = new Mutex(false, $"Global\\{name}Mutex");
            
            // 创建共享内存
            int totalSize = HEADER_SIZE + (MAX_MESSAGES * MESSAGE_SIZE);
            _mmf = MemoryMappedFile.CreateOrOpen(name, totalSize);
            _accessor = _mmf.CreateViewAccessor();

            // 初始化头部
            bool createdNew = false;
            _mutex.WaitOne();
            try
            {
                if (_accessor.ReadInt32(HEADER_MSG_COUNT) == 0)
                {
                    _accessor.Write(HEADER_WRITE_POS, HEADER_SIZE);
                    _accessor.Write(HEADER_READ_POS, HEADER_SIZE);
                    _accessor.Write(HEADER_MSG_COUNT, 0);
                    _accessor.Write(HEADER_MSG_ID, 0);  // 初始化消息ID
                    createdNew = true;
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }

            // 创建消息通知事件
            _newMessageEvent = new EventWaitHandle(
                false,
                EventResetMode.AutoReset,
                $"Global\\{name}Event"
            );

            // 初始化订阅者字典
            _subscribers = new ConcurrentDictionary<int, List<Action<SharedMessage>>>();

            // 启动消息循环
            _cts = new CancellationTokenSource();
            _messageLoop = Task.Run(MessageLoop);

            Debug.WriteLine($"[{_channelName}] 初始化完成, 新创建: {createdNew}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{_channelName}] 初始化失败: {ex.Message}");
            Dispose();
            throw;
        }
    }

   public void Publish(int topicId, byte[] data)
    {
        try
        {
            ThrowIfDisposed();

            if (data == null || data.Length > 1024)
                throw new ArgumentException("Invalid data length");

            _mutex.WaitOne();
            try
            {
                var message = SharedMessage.Create();
                
                // 从共享内存读取并递增消息ID
                int currentMsgId = _accessor.ReadInt32(HEADER_MSG_ID);
                message.MessageId = currentMsgId + 1;
                _accessor.Write(HEADER_MSG_ID, message.MessageId);

                message.TopicId = topicId;
                message.Timestamp = DateTime.UtcNow.Ticks;
                Array.Copy(data, message.Data, data.Length);

                int writePos = _accessor.ReadInt32(HEADER_WRITE_POS);
                int readPos = _accessor.ReadInt32(HEADER_READ_POS);
                int msgCount = _accessor.ReadInt32(HEADER_MSG_COUNT);

                Debug.WriteLine($"[{_channelName}] 发布前 - WritePos: {writePos}, ReadPos: {readPos}, Count: {msgCount}, ID: {message.MessageId}");

                // 检查缓冲区是否已满
                if (msgCount >= MAX_MESSAGES)
                {
                    Debug.WriteLine($"[{_channelName}] 缓冲区已满，等待消息被读取");
                    return;
                }

                // 计算新的写入位置
                int nextWritePos = writePos + MESSAGE_SIZE;
                if (nextWritePos >= HEADER_SIZE + (MAX_MESSAGES * MESSAGE_SIZE))
                {
                    nextWritePos = HEADER_SIZE;
                }

                // 写入消息
                WriteMessage(writePos, message);
                
                // 更新头部信息
                _accessor.Write(HEADER_WRITE_POS, nextWritePos);
                _accessor.Write(HEADER_MSG_COUNT, msgCount + 1);

                Debug.WriteLine($"[{_channelName}] 消息已发布 - NewWritePos: {nextWritePos}, NewCount: {msgCount + 1}, ID: {message.MessageId}");
            }
            finally
            {
                _mutex.ReleaseMutex();
            }

            _newMessageEvent.Set();
            Thread.Sleep(10);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{_channelName}] 发布失败: {ex.Message}");
            throw;
        }
    }

    public IDisposable Subscribe(int topicId, Action<SharedMessage> handler) {
        ThrowIfDisposed();

        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        Debug.WriteLine($"[{_channelName}] 添加订阅: Topic={topicId}");

        _subscribers.AddOrUpdate(
            topicId,
            new List<Action<SharedMessage>> { handler },
            (_, list) =>
            {
                lock (list)
                {
                    list.Add(handler);
                    return list;
                }
            }
        );

        return new Subscription(this, topicId, handler);
    }

    public void Unsubscribe(int topicId, Action<SharedMessage> handler) {
        if (_subscribers.TryGetValue(topicId, out var handlers))
        {
            lock (handlers)
            {
                handlers.Remove(handler);
                if (handlers.Count == 0)
                {
                    _subscribers.TryRemove(topicId, out _);
                }
            }
        }
    }

    private async Task MessageLoop()
    {
        Debug.WriteLine($"[{_channelName}] 消息循环启动");
        int lastProcessedMessageId = 0;

        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                await WaitOneAsync(_newMessageEvent, _cts.Token);

                _mutex.WaitOne();
                try
                {
                    int writePos = _accessor.ReadInt32(HEADER_WRITE_POS);
                    int readPos = _accessor.ReadInt32(HEADER_READ_POS);
                    int msgCount = _accessor.ReadInt32(HEADER_MSG_COUNT);

                    if (msgCount == 0)
                    {
                        continue;
                    }

                    // 读取消息
                    var message = ReadMessage(readPos);

                    // 检查消息ID连续性
                    if (lastProcessedMessageId > 0 && message.MessageId != lastProcessedMessageId + 1)
                    {
                        Debug.WriteLine($"[{_channelName}] 警告：消息不连续！上一个ID: {lastProcessedMessageId}, 当前ID: {message.MessageId}");
                    }
                    lastProcessedMessageId = message.MessageId;

                    // 处理消息
                    if (_subscribers.TryGetValue(message.TopicId, out var handlers))
                    {
                        foreach (var handler in handlers.ToList())
                        {
                            try
                            {
                                handler(message);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"[{_channelName}] 处理异常: {ex.Message}");
                            }
                        }
                    }

                    // 更新读取位置
                    int nextReadPos = readPos + MESSAGE_SIZE;
                    if (nextReadPos >= HEADER_SIZE + (MAX_MESSAGES * MESSAGE_SIZE))
                    {
                        nextReadPos = HEADER_SIZE;
                    }

                    _accessor.Write(HEADER_READ_POS, nextReadPos);
                    _accessor.Write(HEADER_MSG_COUNT, msgCount - 1);

                    Debug.WriteLine($"[{_channelName}] 处理完成 - NewReadPos: {nextReadPos}, NewCount: {msgCount - 1}, ID: {message.MessageId}");
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{_channelName}] 消息循环异常: {ex.Message}");
                await Task.Delay(1000, _cts.Token);
            }
        }
    }

    private void WriteMessage(int position, SharedMessage message) {
        _accessor.Write(position, message.MessageId);
        _accessor.Write(position + 4, message.TopicId);
        _accessor.Write(position + 8, message.Timestamp);
        _accessor.WriteArray(position + 16, message.Data, 0, message.Data.Length);
    }

    private SharedMessage ReadMessage(int position) {
        var message = SharedMessage.Create();
        message.MessageId = _accessor.ReadInt32(position);
        message.TopicId = _accessor.ReadInt32(position + 4);
        message.Timestamp = _accessor.ReadInt64(position + 8);
        _accessor.ReadArray(position + 16, message.Data, 0, message.Data.Length);
        return message;
    }

    private static async Task<bool> WaitOneAsync(WaitHandle handle, CancellationToken cancellationToken) {
        var tcs = new TaskCompletionSource<bool>();
        var registration = ThreadPool.RegisterWaitForSingleObject(
            handle,
            (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
            tcs,
            -1,
            true);

        using (cancellationToken.Register(() =>
               {
                   registration.Unregister(null);
                   tcs.TrySetCanceled();
               }))
        {
            return await tcs.Task;
        }
    }

    private void ThrowIfDisposed() {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(SharedMemoryPubSub));
        }
    }

    public void Dispose() {
        if (!_disposed)
        {
            _disposed = true;
            _cts.Cancel();

            try
            {
                _messageLoop?.Wait(TimeSpan.FromSeconds(1));
            }
            catch
            {
            }

            _accessor?.Dispose();
            _mmf?.Dispose();
            _newMessageEvent?.Dispose();
            _mutex?.Dispose();
            _cts.Dispose();

            Debug.WriteLine($"[{_channelName}] 已释放资源");
        }
    }

    private class Subscription : IDisposable {
        private readonly SharedMemoryPubSub _pubSub;
        private readonly int _topicId;
        private readonly Action<SharedMessage> _handler;
        private bool _disposed;

        public Subscription(SharedMemoryPubSub pubSub, int topicId, Action<SharedMessage> handler) {
            _pubSub = pubSub;
            _topicId = topicId;
            _handler = handler;
        }

        public void Dispose() {
            if (!_disposed)
            {
                _pubSub.Unsubscribe(_topicId, _handler);
                _disposed = true;
            }
        }
    }
}


//
// /// <summary>
// /// 基于共享内存的发布订阅系统
// /// 用于跨进程间的消息通信
// /// </summary>
// public class SharedMemoryPubSub : IDisposable {
//     private const int MAX_MESSAGES = 100; // 最多存储100条消息
//     private const int MESSAGE_SIZE = 1040; // 每条消息大小（头部8字节 + 消息结构大小）
//     private const int HEADER_SIZE = 8; // 头部大小（写位置4字节 + 读位置4字节）
//
//     
//     private readonly MemoryMappedFile _mmf; // 内存映射文件
//     private readonly MemoryMappedViewAccessor _accessor; // 内存访问器
//     private readonly EventWaitHandle _newMessageEvent; // 消息通知事件
//     private readonly ConcurrentDictionary<int, List<Action<SharedMessage>>> _subscribers; // 订阅者集合
//     private readonly CancellationTokenSource _cts; // 取消令牌源
//     private readonly Task _messageLoop; // 消息循环任务
//     private int _messageId; // 消息ID计数器
//     private bool _disposed; // 释放标志
//
//     /// <summary>
//     /// 初始化共享内存发布订阅系统
//     /// </summary>
//     /// <param name="name">共享内存名称，用于跨进程识别</param>
//     /// <exception cref="ArgumentNullException">name 为空时抛出</exception>
//     /// <exception cref="InvalidOperationException">初始化失败时抛出</exception>
//     public SharedMemoryPubSub(string name) {
//         if (string.IsNullOrEmpty(name))
//             throw new ArgumentNullException(nameof(name));
//
//         try
//         {
//             int totalSize = HEADER_SIZE + (MAX_MESSAGES * MESSAGE_SIZE);
//             _mmf = MemoryMappedFile.CreateOrOpen(name, totalSize);
//             _accessor = _mmf.CreateViewAccessor();
//
//             if (_accessor.ReadInt32(0) == 0)
//             {
//                 _accessor.Write(0, HEADER_SIZE);
//                 _accessor.Write(4, HEADER_SIZE);
//             }
//
//             _newMessageEvent = new EventWaitHandle(false, EventResetMode.AutoReset, $"Global\\{name}Event");
//             _subscribers = new ConcurrentDictionary<int, List<Action<SharedMessage>>>();
//             _cts = new CancellationTokenSource();
//             _messageLoop = Task.Run(MessageLoop);
//         }
//         catch (Exception ex)
//         {
//             Dispose();
//             throw new InvalidOperationException("Failed to initialize shared memory", ex);
//         }
//     }
//
//     /// <summary>
//     /// 发布消息到指定主题
//     /// </summary>
//     /// <param name="topicId">主题ID</param>
//     /// <param name="data">消息数据</param>
//     /// <exception cref="ArgumentNullException">data 为空时抛出</exception>
//     /// <exception cref="ArgumentException">data 超过1024字节时抛出</exception>
//     /// <exception cref="ObjectDisposedException">对象已释放时抛出</exception>
//     public void Publish(int topicId, byte[] data) {
//         ThrowIfDisposed();
//
//         if (data == null)
//             throw new ArgumentNullException(nameof(data));
//
//         if (data.Length > 1024)
//             throw new ArgumentException("Data must be less than 1024 bytes", nameof(data));
//
//         var message = SharedMessage.Create();
//         message.MessageId = Interlocked.Increment(ref _messageId);
//         message.TopicId = topicId;
//         message.Timestamp = DateTime.UtcNow.Ticks;
//         Array.Copy(data, message.Data, data.Length);
//
//         lock (_accessor)
//         {
//             int writePos = _accessor.ReadInt32(0);
//             
//             if (writePos >= HEADER_SIZE + (MAX_MESSAGES * MESSAGE_SIZE))
//             {
//                 writePos = HEADER_SIZE;
//             }
//
//             WriteMessage(writePos, message);
//             _accessor.Write(0, writePos + MESSAGE_SIZE);
//         }
//
//         _newMessageEvent.Set();
//     }
//
//     private void WriteMessage(int position, SharedMessage message)
//     {
//         _accessor.Write(position, message.MessageId);
//         _accessor.Write(position + 4, message.TopicId);
//         _accessor.Write(position + 8, message.Timestamp);
//         _accessor.WriteArray(position + 16, message.Data, 0, message.Data.Length);
//     }
//     
//     private SharedMessage ReadMessage(int position)
//     {
//         var message = SharedMessage.Create();
//         message.MessageId = _accessor.ReadInt32(position);
//         message.TopicId = _accessor.ReadInt32(position + 4);
//         message.Timestamp = _accessor.ReadInt64(position + 8);
//         _accessor.ReadArray(position + 16, message.Data, 0, message.Data.Length);
//         return message;
//     }
//     /// <summary>
//     /// 消息循环，负责接收和分发消息
//     /// </summary>
//     private async Task MessageLoop()
//     {
//         while (!_cts.Token.IsCancellationRequested)
//         {
//             try
//             {
//                 await _newMessageEvent.WaitOneAsync(_cts.Token);
//
//                 lock (_accessor)
//                 {
//                     int writePos = _accessor.ReadInt32(0);
//                     int readPos = _accessor.ReadInt32(4);
//
//                     while (readPos < writePos)
//                     {
//                         var message = ReadMessage(readPos);
//
//                         if (_subscribers.TryGetValue(message.TopicId, out var handlers))
//                         {
//                             foreach (var handler in handlers.ToList())
//                             {
//                                 try
//                                 {
//                                     handler(message);
//                                 }
//                                 catch (Exception ex)
//                                 {
//                                     Debug.WriteLine($"Handler error: {ex.Message}");
//                                 }
//                             }
//                         }
//
//                         readPos += MESSAGE_SIZE;
//
//                         if (readPos >= HEADER_SIZE + (MAX_MESSAGES * MESSAGE_SIZE))
//                         {
//                             readPos = HEADER_SIZE;
//                         }
//                     }
//
//                     _accessor.Write(4, readPos);
//                 }
//             }
//             catch (OperationCanceledException)
//             {
//                 break;
//             }
//             catch (Exception ex)
//             {
//                 Debug.WriteLine($"MessageLoop error: {ex.Message}");
//                 await Task.Delay(1000, _cts.Token);
//             }
//         }
//     }
//     /// <summary>
//     /// 订阅指定主题的消息
//     /// </summary>
//     /// <param name="topicId">主题ID</param>
//     /// <param name="handler">消息处理器</param>
//     /// <returns>订阅令牌，用于取消订阅</returns>
//     /// <exception cref="ArgumentNullException">handler 为空时抛出</exception>
//     /// <exception cref="ObjectDisposedException">对象已释放时抛出</exception>
//     public IDisposable Subscribe(int topicId, Action<SharedMessage> handler) {
//         ThrowIfDisposed();
//
//         if (handler == null)
//             throw new ArgumentNullException(nameof(handler));
//
//         _subscribers.AddOrUpdate(
//             topicId,
//             new List<Action<SharedMessage>> { handler },
//             (_, list) =>
//             {
//                 lock (list)
//                 {
//                     list.Add(handler);
//                     return list;
//                 }
//             }
//         );
//
//         return new Subscription(this, topicId, handler);
//     }
//
//     
//     
//
//     /// <summary>
//     /// 取消订阅
//     /// </summary>
//     /// <param name="topicId">主题ID</param>
//     /// <param name="handler">要取消的消息处理器</param>
//     public void Unsubscribe(int topicId, Action<SharedMessage> handler) {
//         if (_subscribers.TryGetValue(topicId, out var handlers))
//         {
//             lock (handlers)
//             {
//                 handlers.Remove(handler);
//                 if (handlers.Count == 0)
//                 {
//                     _subscribers.TryRemove(topicId, out _);
//                 }
//             }
//         }
//     }
//
//     /// <summary>
//     /// 检查对象是否已释放
//     /// </summary>
//     /// <exception cref="ObjectDisposedException">对象已释放时抛出</exception>
//     private void ThrowIfDisposed() {
//         if (_disposed)
//         {
//             throw new ObjectDisposedException(nameof(SharedMemoryPubSub));
//         }
//     }
//
//     public void Dispose() {
//         if (!_disposed)
//         {
//             _disposed = true;
//             _cts.Cancel();
//
//             try
//             {
//                 _messageLoop?.Wait(TimeSpan.FromSeconds(1));
//             }
//             catch
//             {
//             }
//
//             _accessor?.Dispose();
//             _mmf?.Dispose();
//             _newMessageEvent?.Dispose();
//             _cts.Dispose();
//         }
//     }
//
//     /// <summary>
//     /// 订阅令牌类，用于管理订阅生命周期
//     /// </summary>
//     private class Subscription : IDisposable {
//         private readonly SharedMemoryPubSub _pubSub; // 发布订阅系统引用
//         private readonly int _topicId; // 订阅的主题ID
//         private readonly Action<SharedMessage> _handler; // 消息处理器
//         private bool _disposed; // 释放标志
//
//         /// <summary>
//         /// 初始化订阅令牌
//         /// </summary>
//         /// <param name="pubSub">发布订阅系统</param>
//         /// <param name="topicId">主题ID</param>
//         /// <param name="handler">消息处理器</param>
//         public Subscription(SharedMemoryPubSub pubSub, int topicId, Action<SharedMessage> handler) {
//             _pubSub = pubSub;
//             _topicId = topicId;
//             _handler = handler;
//         }
//
//         /// <summary>
//         /// 释放订阅
//         /// </summary>
//         public void Dispose() {
//             if (!_disposed)
//             {
//                 _pubSub.Unsubscribe(_topicId, _handler);
//                 _disposed = true;
//             }
//         }
//     }
// }