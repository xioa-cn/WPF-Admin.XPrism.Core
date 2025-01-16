namespace WPF.SharedMemory.Services;

public static class WaitHandleExtensions {
    public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken) {
        var tcs = new TaskCompletionSource<bool>();

        RegisteredWaitHandle registration = ThreadPool.RegisterWaitForSingleObject(
            handle,
            (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
            tcs,
            -1,
            true);

        cancellationToken.Register(() =>
        {
            registration.Unregister(null);
            tcs.TrySetCanceled();
        });

        return tcs.Task;
    }
}