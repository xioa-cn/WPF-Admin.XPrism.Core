namespace WPFAdmin.Test;

public class MyHashSet {

    private const int DefaultCapacity = 1000;
    private LinkedList[] buckets;

    public MyHashSet()
    {
        buckets = new LinkedList[DefaultCapacity];
        for (int i = 0; i < DefaultCapacity; i++)
        {
            buckets[i] = new LinkedList();
        }
    }

    public void Add(int key)
    {
        int index = GetHash(key);
        if (!buckets[index].Contains(key))
        {
            buckets[index].Add(key);
        }
    }

    public void Remove(int key)
    {
        int index = GetHash(key);
        buckets[index].Remove(key);
    }

    public bool Contains(int key)
    {
        int index = GetHash(key);
        return buckets[index].Contains(key);
    }

    private int GetHash(int key)
    {
        return key % DefaultCapacity;
    }
}

 // 链表节点类
public class ListNode
{
    public int Value { get; set; }
    public ListNode Next { get; set; }

    public ListNode(int value)
    {
        Value = value;
        Next = null;
    }
}
// 链表类，用于处理哈希冲突
public class LinkedList
{
    private ListNode head;

    public LinkedList()
    {
        head = null;
    }

    public void Add(int value)
    {
        if (head == null)
        {
            head = new ListNode(value);
            return;
        }

        // 如果值已存在，不添加
        if (Contains(value))
        {
            return;
        }

        // 添加到链表头部
        ListNode newNode = new ListNode(value);
        newNode.Next = head;
        head = newNode;
    }

    public bool Contains(int value)
    {
        ListNode current = head;
        while (current != null)
        {
            if (current.Value == value)
            {
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    public void Remove(int value)
    {
        if (head == null)
        {
            return;
        }

        // 如果头节点是要删除的值
        if (head.Value == value)
        {
            head = head.Next;
            return;
        }

        // 查找要删除的节点
        ListNode current = head;
        while (current.Next != null && current.Next.Value != value)
        {
            current = current.Next;
        }

        // 如果找到了要删除的节点
        if (current.Next != null)
        {
            current.Next = current.Next.Next;
        }
    }
}