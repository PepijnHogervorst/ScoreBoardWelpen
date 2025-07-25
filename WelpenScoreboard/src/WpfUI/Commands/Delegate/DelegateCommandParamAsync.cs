namespace WelpenScoreboard.WpfUI.Commands.Delegate;
/// <summary>
/// Async delegate <see cref="ICommand"/> with a parameter
/// </summary>
/// <typeparam name="T"></typeparam>
internal class DelegateCommandParamAsync<T> : CommandBaseAsync
{
    private readonly Func<T, Task> _callback;
    private readonly Func<bool>? _canExecuteCallback;

    public DelegateCommandParamAsync(Func<T, Task> callback, Action<Exception> onException) : base(onException)
    {
        _callback = callback;
    }

    public DelegateCommandParamAsync(Func<T, Task> callback, Action<Exception> onException, Func<bool>? canExecuteCallback) : base(onException)
    {
        _callback = callback;
        _canExecuteCallback = canExecuteCallback;
    }

    public override async Task ExecuteAsync(object? parameter)
    {
        if (parameter == null ||
            parameter is not T tParam) return;

        await _callback(tParam);
    }

    public override bool CanExecute(object? parameter = null)
    {
        if (_canExecuteCallback is not null)
        {
            if (!_canExecuteCallback()) return false;
        }
        return base.CanExecute(parameter);
    }

    public void Refresh()
    {
        OnCanExecuteChanged();
    }
}
