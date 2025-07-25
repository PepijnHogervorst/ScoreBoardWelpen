namespace WelpenScoreboard.WpfUI.Commands.Delegate;

public class DelegateNoParameterCommand : ICommand
{
    private readonly Action _execute;
    private readonly Predicate<object?>? _canExecute;

    public DelegateNoParameterCommand(Action execute) : this(execute, null) { }
    public DelegateNoParameterCommand(Action execute, Predicate<object?>? canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }


    public bool CanExecute(object? parameter = null)
    {
        if (_canExecute == null) return true;
        return _canExecute(parameter);
    }

    public void Execute(object? parameter = null)
    {
        _execute();
    }

    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
