using System.Text;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPF.Admin.Models;
using WPF.SharedMemory.Model;
using WPF.SharedMemory.Services;

namespace SharedMemoryDemoModules.ViewModels;

public partial class SharedMemoryViewModel : BindableBase {
    private readonly SharedMemoryPubSub _sharedMemoryPubSub;

    public SharedMemoryViewModel() {
        _sharedMemoryPubSub = new SharedMemoryPubSub("demo");
    }

    [ObservableProperty] private string? _message;

    [ObservableProperty] private string? _send;
    [RelayCommand]
    private void Subscribe() {
        _sharedMemoryPubSub.Subscribe(MessageTopics.DATA_SYNC, OnThemeChanged);
    }

    private void OnThemeChanged((int MessageId, int TopicId, byte[] Data) obj)
    {
        var ms = Encoding.UTF8.GetString(obj.Data).TrimEnd('\0');
        Dispatcher.CurrentDispatcher.Invoke(() =>
        {
            Message = ms;
        });
    }

    [RelayCommand]
    private void Unsubscribe() {
        _sharedMemoryPubSub.Unsubscribe(MessageTopics.DATA_SYNC);
    }

    [RelayCommand]
    private void PublishMessage() {
        _sharedMemoryPubSub.Publish(
            MessageTopics.DATA_SYNC,
            Encoding.UTF8.GetBytes(Send)
        );
    }
}