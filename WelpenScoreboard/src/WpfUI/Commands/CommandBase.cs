namespace WelpenScoreboard.WpfUI.Commands;

internal abstract class CommandBase : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public virtual bool CanExecute(object? parameter = null) => true;

    public abstract void Execute(object? parameter = null);

    protected void OnCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
