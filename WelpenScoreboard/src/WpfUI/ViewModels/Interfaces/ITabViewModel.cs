namespace WelpenScoreboard.WpfUI.ViewModels.Interfaces;
public interface ITabViewModel
{
    /// <summary>
    /// Gets the title of the tab.
    /// </summary>
    string Title { get; }
    int Order { get; }
}
