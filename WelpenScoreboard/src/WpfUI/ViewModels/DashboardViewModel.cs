using WelpenScoreboard.Application.Led;
using WelpenScoreboard.WpfUI.Commands.Delegate;

namespace WelpenScoreboard.WpfUI.ViewModels;
internal class DashboardViewModel : TabViewModelBase
{
    private readonly ILedApplicationStore _ledApplicationStore;

    private bool _isArduinoDone;

    public DashboardViewModel(ILedApplicationStore ledApplicationStore)
    {
        _ledApplicationStore = ledApplicationStore;
        StartCommand = new DelegateNoParameterCommand(OnStart);
        RetryCommand = new DelegateNoParameterCommand(OnRetry);
    }

    public override string Title => "Dashboard";
    public override int Order => 1;

    public bool AreAllGroupsDone => _ledApplicationStore.GroupTurn >= 6;

    public bool IsArduinoDone
    {
        get => _isArduinoDone;
        set
        {
            _isArduinoDone = value;
            OnPropertyChanged(nameof(IsArduinoDone));
        }
    }

    public ICommand StartCommand { get; }
    public ICommand RetryCommand { get; }
    public bool CanStart { get; private set; }


    protected override void OnViewLoad()
    {
        HookEvents(true);
    }

    protected override void OnViewUnload()
    {
        HookEvents(false);
    }

    private void OnStart()
    {
        throw new NotImplementedException();
    }

    private void OnRetry()
    {
        throw new NotImplementedException();
    }

    private void HookEvents(bool hook)
    {
        if (hook)
        {
            _ledApplicationStore.OnArduinoReady += OnArduinoReady;
            return;
        }
        _ledApplicationStore.OnArduinoReady -= OnArduinoReady;
    }

    private void OnArduinoReady(object? sender, EventArgs e)
    {
        IsArduinoDone = true;

        Task.Run(async () =>
        {
            // Delay to ensure UI updates before showing the message
            await Task.Delay(TimeSpan.FromSeconds(7));
            UpdateGroupTurn();
        });
    }

    private void UpdateGroupTurn()
    {
        _ledApplicationStore.GroupTurn++;
    }
}

