namespace WPF.SharedMemory.Model;

public class Configs {
    // 可配置的消息大小
    public const int MAX_MESSAGES = 100; // 最大消息数量
    public const int MAX_MESSAGE_SIZE = 1024; // 每条消息最大字节数（比如改为4KB）
    public const int HEADER_SIZE = 8; // 头部大小保持不变
    private const int MESSAGE_SIZE = MAX_MESSAGE_SIZE + 16; // 16是消息头（MessageId, TopicId, Timestamp）
}