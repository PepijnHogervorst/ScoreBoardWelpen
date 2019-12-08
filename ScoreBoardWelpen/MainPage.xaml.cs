using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ScoreBoardWelpen
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            //EVENTS
            this.nvMenu.ItemInvoked += NvMenu_ItemInvoked;
        }

        private void NvMenu_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                contentFrame.Navigate(typeof(Screens.SettingsPage));
            }
            else
            {
                if (args.InvokedItemContainer is NavigationViewItem ItemContent)
                {
                    switch (ItemContent.Tag)
                    {
                        case "Scoreboard":
                            contentFrame.Navigate(typeof(Screens.Scoreboard));
                            break;

                        case "GroupEdit":
                            contentFrame.Navigate(typeof(Screens.GroupEdit));
                            break;

                        case "Device":
                            contentFrame.Navigate(typeof(Screens.Device));
                            break;
                    }
                }
            }
            

        }

        private void nvMenu_Loaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem
            foreach (NavigationViewItemBase item in nvMenu.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "Scoreboard")
                {
                    nvMenu.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(Screens.Scoreboard));
        }

        private void nvMenu_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {

        }
    }
}
