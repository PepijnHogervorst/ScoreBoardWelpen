using WelpenScoreboard.WpfUI.Commands.Delegate;
using WelpenScoreboard.WpfUI.ViewModels.Interfaces;

namespace WelpenScoreboard.WpfUI.ViewModels.Common;
internal abstract class TabViewModelBase : ViewModelBase, ITabViewModel
{
    protected TabViewModelBase()
    {
        ViewLoadCommand = new DelegateNoParameterCommand(OnViewLoad);
        ViewUnloadCommand = new DelegateNoParameterCommand(OnViewUnload);
    }

    public abstract string Title { get; }
    public virtual int Order { get; } = 10;
    public ICommand ViewLoadCommand { get; }
    public ICommand ViewUnloadCommand { get; }

    protected virtual void OnViewLoad()
    {

    }

    protected virtual void OnViewUnload()
    {

    }
}
