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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ScoreBoardWelpen.Screens
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GroupEdit : Page
    {
        public GroupEdit()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Event hooks:
            hookEvents();

            // Retrieve sqlite data of groups!
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void BtnNewPerson_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TbNewPerson_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetAddButtonActive();
        }

        private void TbNewPerson_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TbNewPerson_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TbGroup_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TbGroup_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TbGroup_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetAddButtonActive();
        }

        #region EVENTS
        private void hookEvents()
        {
            
        }

        private void unhookEvents()
        {

        }
        #endregion

        #region Private methods
        private void SetAddButtonActive()
        {
            if (!string.IsNullOrEmpty(TbNewPerson.Text)&&
                !string.IsNullOrEmpty(TbGroup.Text))
            {
                BtnNewPerson.IsEnabled = true;
            }
            else
            {
                BtnNewPerson.IsEnabled = false;
            }
        }
        #endregion
    }
}
