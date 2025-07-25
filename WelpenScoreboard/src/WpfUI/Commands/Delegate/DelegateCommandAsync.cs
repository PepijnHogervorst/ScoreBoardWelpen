namespace WelpenScoreboard.WpfUI.Commands.Delegate;

internal class DelegateCommandAsync : CommandBaseAsync
{
    private readonly Func<Task> _callback;
    private readonly Func<bool>? _canExecuteCallback;

    public DelegateCommandAsync(Func<Task> callback, Action<Exception> onException) : base(onException)
    {
        _callback = callback;
    }

    public DelegateCommandAsync(Func<Task> callback, Action<Exception> onException, Func<bool>? canExecuteCallback) : base(onException)
    {
        _callback = callback;
        _canExecuteCallback = canExecuteCallback;
    }

    public override Task ExecuteAsync(object? parameter) => _callback();

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
