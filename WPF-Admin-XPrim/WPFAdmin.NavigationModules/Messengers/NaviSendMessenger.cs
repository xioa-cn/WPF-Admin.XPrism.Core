namespace WPFAdmin.NavigationModules.Messengers;

public class NaviSendMessenger<T>
{
    public T Model { get; set; }
    public NaviSendMessenger(T data)
    {
        this.Model = data;
    }
}