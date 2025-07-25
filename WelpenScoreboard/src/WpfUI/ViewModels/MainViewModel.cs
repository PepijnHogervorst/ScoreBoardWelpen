using WelpenScoreboard.Application.Led;
using WelpenScoreboard.WpfUI.ViewModels.Interfaces;

namespace WelpenScoreboard.WpfUI.ViewModels;
public class MainViewModel : ViewModelBase
{
    private ITabViewModel? _selectedItem;

    public MainViewModel(IEnumerable<ITabViewModel> tabs,
                         ILedApplicationStore ledApplicationStore)
    {
        Tabs = [.. tabs.OrderBy(t => t.Order)];
        SelectedItem = Tabs.FirstOrDefault();
        LedApplicationStore = ledApplicationStore;
    }

    public IEnumerable<ITabViewModel> Tabs { get; }
    public int SelectedIndex { get; set; }
    public ITabViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            OnPropertyChanged(nameof(SelectedItem));
        }
    }

    public string SoftwareVersion => "Not finished";

    public ILedApplicationStore LedApplicationStore { get; }
}
