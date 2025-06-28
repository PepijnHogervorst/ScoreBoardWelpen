using WelpenScoreboard.WpfUI.ViewModels.Interfaces;

namespace WelpenScoreboard.WpfUI.ViewModels.Common;
internal abstract class TabViewModelBase : ViewModelBase, ITabViewModel
{
    public abstract string Title { get; }
    public virtual int Order { get; } = 10;
}
