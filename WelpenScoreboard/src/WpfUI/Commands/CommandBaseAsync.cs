using System.Windows.Threading;

namespace WelpenScoreboard.WpfUI.Commands;
/// <summary>
/// Base class for asynchronous commands.
/// </summary>
internal abstract class CommandBaseAsync : IAsyncCommand
{
    private readonly Action<Exception> _onException;
    private bool _isExecuting;

    protected CommandBaseAsync(Action<Exception> onException)
    {
        _onException = onException;
    }

    public bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            _isExecuting = value;
            OnCanExecuteChanged();
        }
    }

    public event EventHandler? CanExecuteChanged;

    public virtual bool CanExecute(object? parameter = null)
    {
        return !IsExecuting;
    }

    public async void Execute(object? parameter = null)
    {
        if (!CanExecute()) return;

        IsExecuting = true;
        try
        {
            await Task.Run(async () => await ExecuteAsync(parameter))
                .ContinueWith((t) =>
                {
                    IsExecuting = false;
                    if (t.IsFaulted) _onException?.Invoke(t.Exception.InnerException ?? t.Exception);
                }, GetTaskSchedulerFromContext());
        }
        catch (Exception ex)
        {
            IsExecuting = false;
            _onException?.Invoke(ex);
        }
    }

    private static TaskScheduler GetTaskSchedulerFromContext()
    {
        if (SynchronizationContext.Current is not DispatcherSynchronizationContext) return TaskScheduler.Current;
        return TaskScheduler.FromCurrentSynchronizationContext();
    }

    protected void OnCanExecuteChanged()
    {
        RunOnUIThread(() => CanExecuteChanged?.Invoke(this, new EventArgs()));
    }

    public abstract Task ExecuteAsync(object? parameter);

    private static void RunOnUIThread(Action action)
    {
        if (System.Windows.Application.Current == null)
        {
            action();
            return;
        }
        System.Windows.Application.Current.Dispatcher.BeginInvoke(action);
    }
}
