namespace DomusNet.Blazor.Services;

public enum ToastType
{
    Success,
    Error,
    Warning,
    Info
}

public class ToastMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Message { get; set; } = string.Empty;
    public ToastType Type { get; set; }
}

public class ToastService
{
    public event Action? OnChange;

    public List<ToastMessage> Toasts { get; } = new();

    public void Success(string message)
    {
        Show(message, ToastType.Success);
    }

    public void Error(string message)
    {
        Show(message, ToastType.Error);
    }

    public void Warning(string message)
    {
        Show(message, ToastType.Warning);
    }

    public void Info(string message)
    {
        Show(message, ToastType.Info);
    }

    private void Show(string message, ToastType type)
    {
        var toast = new ToastMessage
        {
            Message = message,
            Type = type
        };

        Toasts.Add(toast);
        OnChange?.Invoke();

        _ = RemoveAfterDelay(toast.Id);
    }

    public void Remove(Guid id)
    {
        var toast = Toasts.FirstOrDefault(t => t.Id == id);

        if (toast is not null)
        {
            Toasts.Remove(toast);
            OnChange?.Invoke();
        }
    }

    private async Task RemoveAfterDelay(Guid id)
    {
        await Task.Delay(4000);
        Remove(id);
    }
}