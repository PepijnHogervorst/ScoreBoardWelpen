using WelpenScoreboard.WpfUI.ViewModels.Interfaces;

namespace WelpenScoreboard.WpfUI.ViewModels;
public class MainViewModel : ViewModelBase
{
    public MainViewModel(IEnumerable<ITabViewModel> tabs)
    {
        Tabs = [.. tabs.OrderBy(t => t.Order)];
    }

    public IEnumerable<ITabViewModel> Tabs { get; }
}
