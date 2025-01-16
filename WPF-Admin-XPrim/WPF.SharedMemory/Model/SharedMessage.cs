using System.Runtime.InteropServices;

namespace WPF.SharedMemory.Model;

[StructLayout(LayoutKind.Sequential)] //编译器按顺序布局内存
public struct SharedMessage {
    public int MessageId;
    public int TopicId;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = Configs.MAX_MESSAGE_SIZE)] // 固定大小的数组，在内存中占用多少字节
    public byte[] Data;

    public static SharedMessage Create()
    {
        return new SharedMessage
        {
            Data = new byte[1024]
        };
    }

    public long Timestamp;
}