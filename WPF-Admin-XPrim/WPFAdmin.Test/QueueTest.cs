using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace WPFAdmin.Test;

/// <summary>
/// 演示各种队列类型的用法和特点
/// </summary>
public class QueueTest {
    
    /// <summary>
    /// 演示基本泛型队列 Queue&lt;T&gt; 的用法
    /// </summary>
    [Fact]
    public void TestGenericQueue() {
        // 创建一个存储整数的队列
        Queue<int> queue = new Queue<int>();
        
        // 添加元素
        queue.Enqueue(10);
        queue.Enqueue(20);
        queue.Enqueue(30);
        
        Console.WriteLine($"队列元素数量: {queue.Count}");  // 输出: 3
        
        // 查看队首元素但不移除
        int peek = queue.Peek();
        Console.WriteLine($"队首元素: {peek}");  // 输出: 10
        
        // 移除并返回队首元素
        int item = queue.Dequeue();
        Console.WriteLine($"出队元素: {item}");  // 输出: 10
        Console.WriteLine($"出队后队列元素数量: {queue.Count}");  // 输出: 2
        
        // 检查元素是否存在
        bool contains = queue.Contains(20);
        Console.WriteLine($"队列是否包含20: {contains}");  // 输出: true
        
        // 将队列转换为数组
        int[] array = queue.ToArray();
        Console.WriteLine($"转换为数组: [{string.Join(", ", array)}]");  // 输出: [20, 30]
        
        // 遍历队列（不会移除元素）
        Console.WriteLine("遍历队列:");
        foreach (int value in queue) {
            Console.WriteLine(value);  // 输出: 20, 30
        }
        
        // 清空队列
        queue.Clear();
        Console.WriteLine($"清空后队列元素数量: {queue.Count}");  // 输出: 0
        
        // 批量添加元素
        queue = new Queue<int>(new[] { 100, 200, 300 });
        Console.WriteLine($"批量添加后队列元素数量: {queue.Count}");  // 输出: 3
    }
    
    /// <summary>
    /// 演示非泛型队列 Queue 的用法
    /// </summary>
    [Fact]
    public void TestNonGenericQueue() {
        // 创建一个非泛型队列
        Queue queue = new Queue();
        
        // 添加不同类型的元素
        queue.Enqueue(10);
        queue.Enqueue("Hello");
        queue.Enqueue(true);
        
        Console.WriteLine($"队列元素数量: {queue.Count}");  // 输出: 3
        
        // 查看队首元素但不移除
        object peek = queue.Peek();
        Console.WriteLine($"队首元素: {peek}");  // 输出: 10
        
        // 移除并返回队首元素
        object item = queue.Dequeue();
        Console.WriteLine($"出队元素: {item}");  // 输出: 10
        
        // 检查元素是否存在
        bool contains = queue.Contains("Hello");
        Console.WriteLine($"队列是否包含'Hello': {contains}");  // 输出: true
        
        // 将队列转换为数组
        object[] array = queue.ToArray();
        Console.WriteLine($"转换为数组长度: {array.Length}");  // 输出: 2
        
        // 遍历队列
        Console.WriteLine("遍历队列:");
        foreach (object value in queue) {
            Console.WriteLine(value);  // 输出: Hello, true
        }
        
        // 清空队列
        queue.Clear();
        Console.WriteLine($"清空后队列元素数量: {queue.Count}");  // 输出: 0
        
        // 批量添加元素
        queue = new Queue(new object[] { 100, "World", false });
        Console.WriteLine($"批量添加后队列元素数量: {queue.Count}");  // 输出: 3
    }
    
    /// <summary>
    /// 演示线程安全队列 ConcurrentQueue&lt;T&gt; 的用法
    /// </summary>
    [Fact]
    public void TestConcurrentQueue() {
        // 创建一个线程安全的队列
        ConcurrentQueue<string> concurrentQueue = new ConcurrentQueue<string>();
        
        // 添加元素
        concurrentQueue.Enqueue("Item 1");
        concurrentQueue.Enqueue("Item 2");
        concurrentQueue.Enqueue("Item 3");
        
        Console.WriteLine($"队列元素数量: {concurrentQueue.Count}");  // 输出: 3
        
        // 尝试查看队首元素
        if (concurrentQueue.TryPeek(out string peekResult)) {
            Console.WriteLine($"队首元素: {peekResult}");  // 输出: Item 1
        }
        
        // 尝试移除元素
        if (concurrentQueue.TryDequeue(out string result)) {
            Console.WriteLine($"出队元素: {result}");  // 输出: Item 1
        }
        
        Console.WriteLine($"出队后队列元素数量: {concurrentQueue.Count}");  // 输出: 2
        
        // 检查队列是否为空
        bool isEmpty = concurrentQueue.IsEmpty;
        Console.WriteLine($"队列是否为空: {isEmpty}");  // 输出: false
        
        // 遍历队列
        Console.WriteLine("遍历队列:");
        foreach (string item in concurrentQueue) {
            Console.WriteLine(item);  // 输出: Item 2, Item 3
        }
        
        // 多线程示例
        ConcurrentQueue<int> numberQueue = new ConcurrentQueue<int>();
        
        // 创建多个生产者线程
        System.Threading.Tasks. Task[] producers = new System.Threading.Tasks. Task[3];
        for (int i = 0; i < producers.Length; i++) {
            int producerId = i;
            producers[i] = System.Threading.Tasks. Task.Run(() => {
                for (int j = 0; j < 5; j++) {
                    int item = producerId * 10 + j;
                    numberQueue.Enqueue(item);
                    Console.WriteLine($"生产者 {producerId} 添加: {item}");
                    Thread.Sleep(10);  // 模拟工作
                }
            });
        }
        
        // 创建多个消费者线程
        System.Threading.Tasks. Task[] consumers = new System.Threading.Tasks. Task[2];
        for (int i = 0; i < consumers.Length; i++) {
            int consumerId = i;
            consumers[i] = System.Threading.Tasks. Task.Run(() => {
                for (int j = 0; j < 7; j++) {
                    if (numberQueue.TryDequeue(out int item)) {
                        Console.WriteLine($"消费者 {consumerId} 处理: {item}");
                    } else {
                        Console.WriteLine($"消费者 {consumerId} 等待中...");
                    }
                    Thread.Sleep(15);  // 模拟工作
                }
            });
        }
        
        // 等待所有任务完成
        System.Threading.Tasks.Task.WaitAll(producers);
        System.Threading.Tasks.Task.WaitAll(consumers);
        
        Console.WriteLine($"最终队列元素数量: {numberQueue.Count}");
    }
    
    /// <summary>
    /// 演示不可变队列 ImmutableQueue&lt;T&gt; 的用法
    /// </summary>
    [Fact]
    public void TestImmutableQueue() {
        // 创建一个空的不可变队列
        ImmutableQueue<double> immutableQueue = ImmutableQueue<double>.Empty;
        
        // 添加元素（返回新队列）
        ImmutableQueue<double> queue1 = immutableQueue.Enqueue(1.0);
        ImmutableQueue<double> queue2 = queue1.Enqueue(2.0);
        ImmutableQueue<double> queue3 = queue2.Enqueue(3.0);
        
        // 原队列保持不变
        Console.WriteLine($"原队列是否为空: {immutableQueue.IsEmpty}");  // 输出: true
        Console.WriteLine($"queue1元素数量: {queue1.Count()}");  // 输出: 1
        Console.WriteLine($"queue2元素数量: {queue2.Count()}");  // 输出: 2
        Console.WriteLine($"queue3元素数量: {queue3.Count()}");  // 输出: 3
        
        // 查看队首元素
        double peek = queue3.Peek();
        Console.WriteLine($"queue3队首元素: {peek}");  // 输出: 1.0
        
        // 移除元素（返回新队列和移除的元素）
        ImmutableQueue<double> queueWithoutFirst = queue3.Dequeue(out double firstItem);
        Console.WriteLine($"出队元素: {firstItem}");  // 输出: 1.0
        Console.WriteLine($"出队后新队列元素数量: {queueWithoutFirst.Count()}");  // 输出: 2
        
        // 链式操作
        ImmutableQueue<double> newQueue = ImmutableQueue<double>.Empty
            .Enqueue(10.0)
            .Enqueue(20.0)
            .Enqueue(30.0);
        
        Console.WriteLine("遍历新队列:");
        foreach (double value in newQueue) {
            Console.WriteLine(value);  // 输出: 10.0, 20.0, 30.0
        }
    }
    
    /// <summary>
    /// 演示优先级队列 PriorityQueue&lt;TElement, TPriority&gt; 的用法
    /// </summary>
    [Fact]
    public void TestPriorityQueue() {
        // 创建一个优先级队列，元素为字符串，优先级为整数
        PriorityQueue<string, int> priorityQueue = new PriorityQueue<string, int>();
        
        // 添加元素和对应的优先级（数字越小优先级越高）
        priorityQueue.Enqueue("任务C", 3);
        priorityQueue.Enqueue("任务A", 1);
        priorityQueue.Enqueue("任务D", 4);
        priorityQueue.Enqueue("任务B", 2);
        
        Console.WriteLine($"队列元素数量: {priorityQueue.Count}");  // 输出: 4
        
        // 查看最高优先级的元素但不移除
        string peek = priorityQueue.Peek();
        Console.WriteLine($"最高优先级元素: {peek}");  // 输出: 任务A
        
        // 按优先级顺序出队
        Console.WriteLine("按优先级出队:");
        while (priorityQueue.Count > 0) {
            string item = priorityQueue.Dequeue();
            Console.WriteLine(item);  // 输出顺序: 任务A, 任务B, 任务C, 任务D
        }
        
        // 使用自定义比较器（按字符串长度排序）
        PriorityQueue<string, string> customPriorityQueue = 
            new PriorityQueue<string, string>(Comparer<string>.Create((a, b) => a.Length.CompareTo(b.Length)));
        
        customPriorityQueue.Enqueue("Apple", "Apple");
        customPriorityQueue.Enqueue("Banana", "Banana");
        customPriorityQueue.Enqueue("Cherry", "Cherry");
        customPriorityQueue.Enqueue("Fig", "Fig");
        
        Console.WriteLine("按字符串长度优先级出队:");
        while (customPriorityQueue.Count > 0) {
            string item = customPriorityQueue.Dequeue();
            Console.WriteLine(item);  // 输出顺序: Fig, Apple, Banana, Cherry
        }
        
        // 创建具有相同优先级的元素
        PriorityQueue<string, int> samepriorityQueue = new PriorityQueue<string, int>();
        samepriorityQueue.Enqueue("First", 1);
        samepriorityQueue.Enqueue("Second", 1);
        samepriorityQueue.Enqueue("Third", 1);
        
        Console.WriteLine("相同优先级出队 (FIFO顺序):");
        while (samepriorityQueue.Count > 0) {
            string item = samepriorityQueue.Dequeue();
            Console.WriteLine(item);  // 输出顺序: First, Second, Third
        }
    }
    
    /// <summary>
    /// 比较不同队列类型的性能
    /// </summary>
    [Fact]
    public void CompareQueuePerformance() {
        const int iterations = 1000000;
        
        // 测试 Queue<T>
        Stopwatch sw = Stopwatch.StartNew();
        Queue<int> queue = new Queue<int>();
        for (int i = 0; i < iterations; i++) {
            queue.Enqueue(i);
        }
        for (int i = 0; i < iterations; i++) {
            int item = queue.Dequeue();
        }
        sw.Stop();
        Console.WriteLine($"Queue<T> 耗时: {sw.ElapsedMilliseconds} ms");
        
        // 测试 ConcurrentQueue<T>
        sw.Restart();
        ConcurrentQueue<int> concurrentQueue = new ConcurrentQueue<int>();
        for (int i = 0; i < iterations; i++) {
            concurrentQueue.Enqueue(i);
        }
        for (int i = 0; i < iterations; i++) {
            concurrentQueue.TryDequeue(out int item);
        }
        sw.Stop();
        Console.WriteLine($"ConcurrentQueue<T> 耗时: {sw.ElapsedMilliseconds} ms");
        
        // 测试 ImmutableQueue<T>
        sw.Restart();
        ImmutableQueue<int> immutableQueue = ImmutableQueue<int>.Empty;
        for (int i = 0; i < iterations / 100; i++) {  // 减少迭代次数，因为不可变队列较慢
            immutableQueue = immutableQueue.Enqueue(i);
        }
        for (int i = 0; i < iterations / 100; i++) {
            immutableQueue = immutableQueue.Dequeue(out int item);
        }
        sw.Stop();
        Console.WriteLine($"ImmutableQueue<T> 耗时 (iterations/100): {sw.ElapsedMilliseconds} ms");
        
        // 测试 PriorityQueue<T,T>
        sw.Restart();
        PriorityQueue<int, int> priorityQueue = new PriorityQueue<int, int>();
        for (int i = 0; i < iterations; i++) {
            priorityQueue.Enqueue(i, i);
        }
        for (int i = 0; i < iterations; i++) {
            int item = priorityQueue.Dequeue();
        }
        sw.Stop();
        Console.WriteLine($"PriorityQueue<T,T> 耗时: {sw.ElapsedMilliseconds} ms");
    }
    
    /// <summary>
    /// 演示队列在实际应用中的用例 - 任务调度器
    /// </summary>
    [Fact]
    public void TaskSchedulerExample() {
        // 创建一个简单的任务调度器
        TaskScheduler scheduler = new TaskScheduler();
        
        // 添加一些任务
        scheduler.AddTask(new Task("发送邮件", 2));
        scheduler.AddTask(new Task("生成报告", 1));
        scheduler.AddTask(new Task("备份数据", 3));
        scheduler.AddTask(new Task("系统更新", 1));
        
        // 处理所有任务
        Console.WriteLine("按优先级处理任务:");
        scheduler.ProcessAllTasks();
    }
    
    /// <summary>
    /// 演示队列在实际应用中的用例 - 广度优先搜索
    /// </summary>
    [Fact]
    public void BreadthFirstSearchExample() {
        // 创建一个简单的图
        Dictionary<string, List<string>> graph = new Dictionary<string, List<string>> {
            { "A", new List<string> { "B", "C" } },
            { "B", new List<string> { "A", "D", "E" } },
            { "C", new List<string> { "A", "F" } },
            { "D", new List<string> { "B" } },
            { "E", new List<string> { "B", "F" } },
            { "F", new List<string> { "C", "E" } }
        };
        
        // 执行广度优先搜索
        Console.WriteLine("从节点A开始的广度优先搜索:");
        BreadthFirstSearch(graph, "A");
    }
    
    // 广度优先搜索实现
    private void BreadthFirstSearch(Dictionary<string, List<string>> graph, string start) {
        Queue<string> queue = new Queue<string>();
        HashSet<string> visited = new HashSet<string>();
        
        // 将起始节点加入队列并标记为已访问
        queue.Enqueue(start);
        visited.Add(start);
        
        while (queue.Count > 0) {
            // 出队一个节点
            string node = queue.Dequeue();
            Console.WriteLine($"访问节点: {node}");
            
            // 访问所有相邻节点
            foreach (string neighbor in graph[node]) {
                if (!visited.Contains(neighbor)) {
                    // 将未访问的相邻节点加入队列并标记为已访问
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }
    }
    
    // 任务类
    private class Task {
        public string Name { get; }
        public int Priority { get; }
        
        public Task(string name, int priority) {
            Name = name;
            Priority = priority;
        }
    }
    
    // 简单的任务调度器
    private class TaskScheduler {
        private readonly PriorityQueue<Task, int> _taskQueue = new PriorityQueue<Task, int>();
        
        public void AddTask(Task task) {
            _taskQueue.Enqueue(task, task.Priority);
        }
        
        public void ProcessAllTasks() {
            while (_taskQueue.Count > 0) {
                Task task = _taskQueue.Dequeue();
                Console.WriteLine($"处理任务: {task.Name} (优先级: {task.Priority})");
                // 模拟任务处理
                Thread.Sleep(10);
            }
        }
    }
}