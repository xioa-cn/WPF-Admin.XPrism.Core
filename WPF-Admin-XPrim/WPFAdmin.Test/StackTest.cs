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
/// 演示各种栈类型的用法和特点
/// </summary>
public class StackTest {
    
    /// <summary>
    /// 演示基本泛型栈 Stack&lt;T&gt; 的用法
    /// </summary>
    [Fact]
    public void TestGenericStack() {
        // 创建一个存储整数的栈
        Stack<int> stack = new Stack<int>();
        
        // 添加元素
        stack.Push(10);
        stack.Push(20);
        stack.Push(30);
        
        Console.WriteLine($"栈元素数量: {stack.Count}");  // 输出: 3
        
        // 查看栈顶元素但不移除
        int peek = stack.Peek();
        Console.WriteLine($"栈顶元素: {peek}");  // 输出: 30
        
        // 移除并返回栈顶元素
        int item = stack.Pop();
        Console.WriteLine($"弹出元素: {item}");  // 输出: 30
        Console.WriteLine($"弹出后栈元素数量: {stack.Count}");  // 输出: 2
        
        // 检查元素是否存在
        bool contains = stack.Contains(20);
        Console.WriteLine($"栈是否包含20: {contains}");  // 输出: true
        
        // 将栈转换为数组（顺序是从栈顶到栈底）
        int[] array = stack.ToArray();
        Console.WriteLine($"转换为数组: [{string.Join(", ", array)}]");  // 输出: [20, 10]
        
        // 遍历栈（不会移除元素，顺序是从栈顶到栈底）
        Console.WriteLine("遍历栈:");
        foreach (int value in stack) {
            Console.WriteLine(value);  // 输出: 20, 10
        }
        
        // 清空栈
        stack.Clear();
        Console.WriteLine($"清空后栈元素数量: {stack.Count}");  // 输出: 0
        
        // 批量添加元素
        stack = new Stack<int>(new[] { 100, 200, 300 });  // 注意：300在栈底，100在栈顶
        Console.WriteLine($"批量添加后栈元素数量: {stack.Count}");  // 输出: 3
        Console.WriteLine($"栈顶元素: {stack.Peek()}");  // 输出: 100
    }
    
    /// <summary>
    /// 演示非泛型栈 Stack 的用法
    /// </summary>
    [Fact]
    public void TestNonGenericStack() {
        // 创建一个非泛型栈
        Stack stack = new Stack();
        
        // 添加不同类型的元素
        stack.Push(10);
        stack.Push("Hello");
        stack.Push(true);
        
        Console.WriteLine($"栈元素数量: {stack.Count}");  // 输出: 3
        
        // 查看栈顶元素但不移除
        object peek = stack.Peek();
        Console.WriteLine($"栈顶元素: {peek}");  // 输出: True
        
        // 移除并返回栈顶元素
        object item = stack.Pop();
        Console.WriteLine($"弹出元素: {item}");  // 输出: True
        
        // 检查元素是否存在
        bool contains = stack.Contains("Hello");
        Console.WriteLine($"栈是否包含'Hello': {contains}");  // 输出: true
        
        // 将栈转换为数组
        object[] array = stack.ToArray();
        Console.WriteLine($"转换为数组长度: {array.Length}");  // 输出: 2
        Console.WriteLine($"数组第一个元素: {array[0]}");  // 输出: Hello
        
        // 遍历栈
        Console.WriteLine("遍历栈:");
        foreach (object value in stack) {
            Console.WriteLine(value);  // 输出: Hello, 10
        }
        
        // 清空栈
        stack.Clear();
        Console.WriteLine($"清空后栈元素数量: {stack.Count}");  // 输出: 0
        
        // 批量添加元素
        stack = new Stack(new object[] { 100, "World", false });
        Console.WriteLine($"批量添加后栈元素数量: {stack.Count}");  // 输出: 3
        Console.WriteLine($"栈顶元素: {stack.Peek()}");  // 输出: false
    }
    
    /// <summary>
    /// 演示线程安全栈 ConcurrentStack&lt;T&gt; 的用法
    /// </summary>
    [Fact]
    public void TestConcurrentStack() {
        // 创建一个线程安全的栈
        ConcurrentStack<string> concurrentStack = new ConcurrentStack<string>();
        
        // 添加元素
        concurrentStack.Push("Item 1");
        concurrentStack.Push("Item 2");
        concurrentStack.Push("Item 3");
        
        Console.WriteLine($"栈元素数量: {concurrentStack.Count}");  // 输出: 3
        
        // 尝试查看栈顶元素
        if (concurrentStack.TryPeek(out string peekResult)) {
            Console.WriteLine($"栈顶元素: {peekResult}");  // 输出: Item 3
        }
        
        // 尝试移除元素
        if (concurrentStack.TryPop(out string result)) {
            Console.WriteLine($"弹出元素: {result}");  // 输出: Item 3
        }
        
        Console.WriteLine($"弹出后栈元素数量: {concurrentStack.Count}");  // 输出: 2
        
        // 检查栈是否为空
        bool isEmpty = concurrentStack.IsEmpty;
        Console.WriteLine($"栈是否为空: {isEmpty}");  // 输出: false
        
        // 遍历栈
        Console.WriteLine("遍历栈:");
        foreach (string item in concurrentStack) {
            Console.WriteLine(item);  // 输出: Item 2, Item 1
        }
        
        // 批量添加元素
        string[] items = new string[] { "Batch 1", "Batch 2", "Batch 3" };
        concurrentStack.PushRange(items);
        Console.WriteLine($"批量添加后栈元素数量: {concurrentStack.Count}");  // 输出: 5
        
        // 批量移除元素
        string[] poppedItems = new string[2];
        int poppedCount = concurrentStack.TryPopRange(poppedItems, 0, 2);
        Console.WriteLine($"批量弹出元素数量: {poppedCount}");  // 输出: 2
        Console.WriteLine($"批量弹出的元素: {string.Join(", ", poppedItems)}");  // 输出: Batch 3, Batch 2
        
        // 多线程示例
        ConcurrentStack<int> numberStack = new ConcurrentStack<int>();
        
        // 创建多个生产者线程
        Task[] producers = new Task[3];
        for (int i = 0; i < producers.Length; i++) {
            int producerId = i;
            producers[i] = Task.Run(() => {
                for (int j = 0; j < 5; j++) {
                    int item = producerId * 10 + j;
                    numberStack.Push(item);
                    Console.WriteLine($"生产者 {producerId} 添加: {item}");
                    Thread.Sleep(10);  // 模拟工作
                }
            });
        }
        
        // 创建多个消费者线程
        Task[] consumers = new Task[2];
        for (int i = 0; i < consumers.Length; i++) {
            int consumerId = i;
            consumers[i] = Task.Run(() => {
                for (int j = 0; j < 7; j++) {
                    if (numberStack.TryPop(out int item)) {
                        Console.WriteLine($"消费者 {consumerId} 处理: {item}");
                    } else {
                        Console.WriteLine($"消费者 {consumerId} 等待中...");
                    }
                    Thread.Sleep(15);  // 模拟工作
                }
            });
        }
        
        // 等待所有任务完成
        Task.WaitAll(producers);
        Task.WaitAll(consumers);
        
        Console.WriteLine($"最终栈元素数量: {numberStack.Count}");
    }
    
    /// <summary>
    /// 演示不可变栈 ImmutableStack&lt;T&gt; 的用法
    /// </summary>
    [Fact]
    public void TestImmutableStack() {
        // 创建一个空的不可变栈
        ImmutableStack<double> immutableStack = ImmutableStack<double>.Empty;
        
        // 添加元素（返回新栈）
        ImmutableStack<double> stack1 = immutableStack.Push(1.0);
        ImmutableStack<double> stack2 = stack1.Push(2.0);
        ImmutableStack<double> stack3 = stack2.Push(3.0);
        
        // 原栈保持不变
        Console.WriteLine($"原栈是否为空: {immutableStack.IsEmpty}");  // 输出: true
        Console.WriteLine($"stack1元素数量: {stack1.Count()}");  // 输出: 1
        Console.WriteLine($"stack2元素数量: {stack2.Count()}");  // 输出: 2
        Console.WriteLine($"stack3元素数量: {stack3.Count()}");  // 输出: 3
        
        // 查看栈顶元素
        double peek = stack3.Peek();
        Console.WriteLine($"stack3栈顶元素: {peek}");  // 输出: 3.0
        
        // 移除元素（返回新栈和移除的元素）
        ImmutableStack<double> stackWithoutTop = stack3.Pop(out double topItem);
        Console.WriteLine($"弹出元素: {topItem}");  // 输出: 3.0
        Console.WriteLine($"弹出后新栈元素数量: {stackWithoutTop.Count()}");  // 输出: 2
        
        // 链式操作
        ImmutableStack<double> newStack = ImmutableStack<double>.Empty
            .Push(10.0)
            .Push(20.0)
            .Push(30.0);
        
        Console.WriteLine("遍历新栈:");
        foreach (double value in newStack) {
            Console.WriteLine(value);  // 输出: 30.0, 20.0, 10.0
        }
        
        // 清空栈
        ImmutableStack<double> emptyStack = newStack.Clear();
        Console.WriteLine($"清空后栈是否为空: {emptyStack.IsEmpty}");  // 输出: true
    }
    
    /// <summary>
    /// 比较不同栈类型的性能
    /// </summary>
    [Fact]
    public void CompareStackPerformance() {
        const int iterations = 1000000;
        
        // 测试 Stack<T>
        Stopwatch sw = Stopwatch.StartNew();
        Stack<int> stack = new Stack<int>();
        for (int i = 0; i < iterations; i++) {
            stack.Push(i);
        }
        for (int i = 0; i < iterations; i++) {
            int item = stack.Pop();
        }
        sw.Stop();
        Console.WriteLine($"Stack<T> 耗时: {sw.ElapsedMilliseconds} ms");
        
        // 测试 ConcurrentStack<T>
        sw.Restart();
        ConcurrentStack<int> concurrentStack = new ConcurrentStack<int>();
        for (int i = 0; i < iterations; i++) {
            concurrentStack.Push(i);
        }
        for (int i = 0; i < iterations; i++) {
            concurrentStack.TryPop(out int item);
        }
        sw.Stop();
        Console.WriteLine($"ConcurrentStack<T> 耗时: {sw.ElapsedMilliseconds} ms");
        
        // 测试 ImmutableStack<T>
        sw.Restart();
        ImmutableStack<int> immutableStack = ImmutableStack<int>.Empty;
        for (int i = 0; i < iterations / 100; i++) {  // 减少迭代次数，因为不可变栈较慢
            immutableStack = immutableStack.Push(i);
        }
        for (int i = 0; i < iterations / 100; i++) {
            immutableStack = immutableStack.Pop(out int item);
        }
        sw.Stop();
        Console.WriteLine($"ImmutableStack<T> 耗时 (iterations/100): {sw.ElapsedMilliseconds} ms");
    }
    
    /// <summary>
    /// 演示栈在实际应用中的用例 - 表达式求值
    /// </summary>
    [Fact]
    public void ExpressionEvaluationExample() {
        // 创建一个简单的后缀表达式求值器
        PostfixEvaluator evaluator = new PostfixEvaluator();
        
        // 计算一些后缀表达式
        Console.WriteLine($"3 4 + = {evaluator.Evaluate("3 4 +")}");  // 输出: 7
        Console.WriteLine($"5 1 2 + 4 * + 3 - = {evaluator.Evaluate("5 1 2 + 4 * + 3 -")}");  // 输出: 14
        Console.WriteLine($"2 3 ^ = {evaluator.Evaluate("2 3 ^")}");  // 输出: 8
    }
    
    /// <summary>
    /// 演示栈在实际应用中的用例 - 括号匹配
    /// </summary>
    [Fact]
    public void BracketMatchingExample() {
        // 测试一些括号表达式
        Console.WriteLine("() 是否匹配: " + IsBalanced("()"));  // 输出: true
        Console.WriteLine("()[]{} 是否匹配: " + IsBalanced("()[]{}"));  // 输出: true
        Console.WriteLine("(] 是否匹配: " + IsBalanced("(]"));  // 输出: false
        Console.WriteLine("([)] 是否匹配: " + IsBalanced("([)]"));  // 输出: false
        Console.WriteLine("{[()]} 是否匹配: " + IsBalanced("{[]()}"));  // 输出: true
    }
    
    /// <summary>
    /// 演示栈在实际应用中的用例 - 深度优先搜索
    /// </summary>
    [Fact]
    public void DepthFirstSearchExample() {
        // 创建一个简单的图
        Dictionary<string, List<string>> graph = new Dictionary<string, List<string>> {
            { "A", new List<string> { "B", "C" } },
            { "B", new List<string> { "A", "D", "E" } },
            { "C", new List<string> { "A", "F" } },
            { "D", new List<string> { "B" } },
            { "E", new List<string> { "B", "F" } },
            { "F", new List<string> { "C", "E" } }
        };
        
        // 执行深度优先搜索
        Console.WriteLine("从节点A开始的深度优先搜索:");
        DepthFirstSearch(graph, "B");
    }
    
    /// <summary>
    /// 演示栈在实际应用中的用例 - 撤销操作
    /// </summary>
    [Fact]
    public void UndoOperationExample() {
        // 创建一个简单的文本编辑器
        TextEditor editor = new TextEditor();
        
        // 执行一些操作
        editor.Insert("Hello");
        editor.Insert(" World");
        editor.Delete(5);  // 删除" World"
        editor.Insert("!");
        
        Console.WriteLine($"当前文本: {editor.GetText()}");  // 输出: Hello!
        
        // 撤销操作
        editor.Undo();
        Console.WriteLine($"撤销后文本: {editor.GetText()}");  // 输出: Hello
        
        editor.Undo();
        Console.WriteLine($"再次撤销后文本: {editor.GetText()}");  // 输出: Hello World
        
        editor.Undo();
        Console.WriteLine($"第三次撤销后文本: {editor.GetText()}");  // 输出: Hello
        
        // 重做操作
        editor.Redo();
        Console.WriteLine($"重做后文本: {editor.GetText()}");  // 输出: Hello World
    }
    
    // 检查括号是否匹配
    private bool IsBalanced(string expression) {
        Stack<char> stack = new Stack<char>();
        
        foreach (char c in expression) {
            if (c == '(' || c == '[' || c == '{') {
                stack.Push(c);
            } else if (c == ')' || c == ']' || c == '}') {
                if (stack.Count == 0) {
                    return false;
                }
                
                char top = stack.Pop();
                if ((c == ')' && top != '(') || 
                    (c == ']' && top != '[') || 
                    (c == '}' && top != '{')) {
                    return false;
                }
            }
        }
        
        return stack.Count == 0;
    }
    
    // 深度优先搜索实现
    private void DepthFirstSearch(Dictionary<string, List<string>> graph, string start) {
        Stack<string> stack = new Stack<string>();
        HashSet<string> visited = new HashSet<string>();
        
        // 将起始节点加入栈
        stack.Push(start);
        
        while (stack.Count > 0) {
            // 弹出一个节点
            string node = stack.Pop();
            
            // 如果节点未访问过，则访问它
            if (!visited.Contains(node)) {
                Console.WriteLine($"访问节点: {node}");
                visited.Add(node);
                
                // 将所有未访问的相邻节点加入栈
                foreach (string neighbor in graph[node]) {
                    if (!visited.Contains(neighbor)) {
                        stack.Push(neighbor);
                    }
                }
            }
        }
    }
    
    // 后缀表达式求值器
    private class PostfixEvaluator {
        public int Evaluate(string expression) {
            Stack<int> stack = new Stack<int>();
            string[] tokens = expression.Split(' ');
            
            foreach (string token in tokens) {
                // 如果是数字，则压入栈
                if (int.TryParse(token, out int number)) {
                    stack.Push(number);
                } else {
                    // 如果是运算符，则弹出两个操作数进行计算
                    int b = stack.Pop();
                    int a = stack.Pop();
                    
                    switch (token) {
                        case "+":
                            stack.Push(a + b);
                            break;
                        case "-":
                            stack.Push(a - b);
                            break;
                        case "*":
                            stack.Push(a * b);
                            break;
                        case "/":
                            stack.Push(a / b);
                            break;
                        case "^":
                            stack.Push((int)Math.Pow(a, b));
                            break;
                    }
                }
            }
            
            return stack.Pop();
        }
    }
    
    // 简单的文本编辑器（支持撤销/重做）
    private class TextEditor {
        private string _text = "";
        private Stack<(string Text, string Operation)> _undoStack = new Stack<(string, string)>();
        private Stack<(string Text, string Operation)> _redoStack = new Stack<(string, string)>();
        
        public void Insert(string text) {
            _undoStack.Push((_text, "Insert"));
            _text += text;
            _redoStack.Clear();  // 新操作会清空重做栈
        }
        
        public void Delete(int length) {
            if (length <= _text.Length) {
                _undoStack.Push((_text, "Delete"));
                _text = _text.Substring(0, _text.Length - length);
                _redoStack.Clear();
            }
        }
        
        public void Undo() {
            if (_undoStack.Count > 0) {
                var (previousText, operation) = _undoStack.Pop();
                _redoStack.Push((_text, operation));
                _text = previousText;
            }
        }
        
        public void Redo() {
            if (_redoStack.Count > 0) {
                var (previousText, operation) = _redoStack.Pop();
                _undoStack.Push((_text, operation));
                _text = previousText;
            }
        }
        
        public string GetText() {
            return _text;
        }
    }
}