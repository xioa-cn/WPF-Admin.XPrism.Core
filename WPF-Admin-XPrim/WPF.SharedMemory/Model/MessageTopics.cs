namespace WPF.SharedMemory.Model;

/// <summary>
/// 消息主题定义
/// 用于区分不同类型的消息，确保发布者和订阅者之间的消息分类
/// </summary>
public class MessageTopics
{
    /// <summary>
    /// UI更新消息
    /// 用于通知界面更新，如：主题变化、控件状态改变等
    /// </summary>
    public const int UI_UPDATE = 1;

    /// <summary>
    /// 日志消息
    /// 用于系统日志、操作日志、调试信息等
    /// </summary>
    public const int LOG_MESSAGE = 2;

    /// <summary>
    /// 错误消息
    /// 用于异常通知、错误警告、故障报告等
    /// </summary>
    public const int ERROR_MESSAGE = 3;

    /// <summary>
    /// 数据同步消息
    /// 用于进程间的数据同步、状态更新等
    /// </summary>
    public const int DATA_SYNC = 4;

    /// <summary>
    /// 命令消息
    /// 用于系统控制命令，如：启动、停止、重启等操作
    /// </summary>
    public const int COMMAND = 5;
}