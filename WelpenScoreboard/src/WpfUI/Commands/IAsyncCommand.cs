namespace WelpenScoreboard.WpfUI.Commands;
internal interface IAsyncCommand : ICommand
{
    Task ExecuteAsync(object? parameter);
}
