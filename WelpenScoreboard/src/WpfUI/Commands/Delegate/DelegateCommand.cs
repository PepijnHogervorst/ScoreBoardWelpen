namespace WelpenScoreboard.WpfUI.Commands.Delegate;

public class DelegateCommand<T> : ICommand
{
    private readonly Predicate<T>? _canExecute;
    private readonly Action<T> _execute;

    public DelegateCommand(Action<T> execute)
     : this(execute, null)
    {
    }

    public DelegateCommand(Action<T> execute, Predicate<T>? canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        if (_canExecute == null) return true;
        if (parameter == null) return true;

        return _canExecute((T)parameter);
    }

    public void Execute(object? parameter)
    {
        if (parameter == null ||
            parameter is not T tParam) return;
        _execute(tParam);
    }

    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
