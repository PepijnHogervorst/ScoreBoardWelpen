using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
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

    private void OnSelectedItemChanged(object sender, DependencyPropertyChangedEventArgs e)
        => MainScrollViewer.ScrollToHome();

    private void DemoItemsListBox_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        //until we had a StaysOpen flag to Drawer, this will help with scroll bars
        var dependencyObject = Mouse.Captured as DependencyObject;

        while (dependencyObject != null)
        {
            if (dependencyObject is ScrollBar) return;
            dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
        }

        MenuToggleButton.IsChecked = false;
    }
}