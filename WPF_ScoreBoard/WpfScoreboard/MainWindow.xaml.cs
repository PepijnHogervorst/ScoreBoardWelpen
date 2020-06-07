using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfScoreboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void MenuToggleButton_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void MenuDarkModeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LbMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsInitialized) return;

            if (LbMenu.SelectedItem is ListBoxItem item)
            {
                switch (item.Name)
                {
                    case "LbItemHome":
                        FrameMain.Navigate(new Uri("Screens/Dashboard.xaml", UriKind.Relative));
                        break;

                    case "LbItemUsers":
                        FrameMain.Navigate(new Uri("Screens/Users.xaml", UriKind.Relative));
                        break;

                    case "LbItemConfig":
                        FrameMain.Navigate(new Uri("Screens/Config.xaml", UriKind.Relative));
                        break;

                    default:
                        FrameMain.Navigate(new Uri("Screens/Dashboard.xaml", UriKind.Relative));
                        break;
                }
            }
            
        }
    }
}
