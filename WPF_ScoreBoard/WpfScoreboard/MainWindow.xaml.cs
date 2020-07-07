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
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Screen event methods
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Get settings and group points from database
            Globals.Storage.RetrieveData();

            // Set date to current date on pc
            Globals.Storage.CurrentDate = DateTimeOffset.Now;
            Globals.Storage.SettingsReplace(Classes.SettingNames.CurrentDate, DateTimeOffset.Now.ToString());
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Globals.Communication.CloseDevice();
        }
        #endregion



        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void MenuToggleButton_OnClick(object sender, RoutedEventArgs e)
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

                    case "LbItemParty":
                        FrameMain.Navigate(new Uri("Screens/Party.xaml", UriKind.Relative));
                        break;

                    default:
                        FrameMain.Navigate(new Uri("Screens/Dashboard.xaml", UriKind.Relative));
                        break;
                }

                DrawerMenuLeft.IsLeftDrawerOpen = false;
            }
            
        }

        
    }
}
