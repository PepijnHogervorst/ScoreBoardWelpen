using System.Windows;
using WelpenScoreboard.WpfUI.ViewModels;

namespace WelpenScoreboard.WpfUI.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
    }
}