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
using System.Windows.Threading;

namespace WpfScoreboard.Screens
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : Page
    {
        #region Private properties
        private DispatcherTimer CloseTimer = null;
        private int ListBoxMemory = 0;
        #endregion

        public Users()
        {
            InitializeComponent();
        }

        #region Page loading methods
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Event hooks:
            HookEvents();

            // Clear entry fields
            this.TbNewPerson.Text = "";
            this.TbGroup.Text = "";
            this.BtnNewPerson.IsEnabled = false;
            this.BtnRemove.IsEnabled = false;
            this.SetAddButtonActive();

            // Clear data dynamically
            object obj = null;
            int loop = 1;
            do
            {
                obj = this.FindName("LbGroup" + loop.ToString());
                if (obj is ListBox)
                {
                    ListBox lb = obj as ListBox;
                    lb.Items.Clear();
                }
                loop++;
            } while (obj != null);

            // Retrieve sqlite data of groups!
            List<Classes.Groups> groups = Globals.Storage.GetGroups("*");
            foreach (Classes.Groups group in groups)
            {
                obj = this.FindName("LbGroup" + group.GroupNr.ToString());
                if (obj is ListBox)
                {
                    ListBox lb = obj as ListBox;
                    lb.Items.Add(group.Name);
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            UnhookEvents();
        }
        #endregion

        #region Buttons
        private void BtnNewPerson_Click(object sender, RoutedEventArgs e)
        {
            // Add new person to database
            if (!Globals.Storage.AddPerson(Convert.ToInt32(TbGroup.Text), TbNewPerson.Text))
            {
                // Person not unique:
                this.PopupNameUnique.Visibility = Visibility.Visible;

                // Start close popup timer 
                if (CloseTimer == null)
                {
                    CloseTimer = new DispatcherTimer();
                    CloseTimer.Interval = TimeSpan.FromSeconds(5);
                    CloseTimer.Tick += CloseTimer_Tick;
                    CloseTimer.Start();
                }
            }
            else
            {
                // Add new person to listbox
                object objToFind = this.FindName("LbGroup" + TbGroup.Text);
                if (objToFind is ListBox)
                {
                    ListBox lb = objToFind as ListBox;
                    lb.Items.Add(TbNewPerson.Text);
                }

                // Clear entry fields
                this.TbNewPerson.Text = "";
                this.TbGroup.Text = "";
                this.BtnNewPerson.IsEnabled = false;
            }

            this.SetAddButtonActive();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Loop through all listboxes to find selected and remove
            for (int i = 0; i < Globals.MaxNrOfGroups; i++)
            {
                object objToFind = this.FindName("LbGroup" + (i + 1).ToString());
                if (objToFind is ListBox)
                {
                    ListBox lb = objToFind as ListBox;
                    if (lb.SelectedIndex >= 0)
                    {
                        // Remove from database
                        Globals.Storage.RemovePerson(lb.SelectedItem.ToString());

                        //Remove from listbox
                        lb.Items.RemoveAt(lb.SelectedIndex);

                        break;
                    }
                }
            }
            this.BtnRemove.IsEnabled = false;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.PopupNameUnique.Visibility = Visibility.Collapsed;

            if (CloseTimer != null)
            {
                CloseTimer.Stop();
                CloseTimer.Tick -= CloseTimer_Tick;
                CloseTimer = null;
            }
        }
        #endregion

        #region EVENTS
        private void HookEvents()
        {
            HookListBoxEvents(true);
        }

        private void UnhookEvents()
        {
            HookListBoxEvents(false);
        }

        private void CloseTimer_Tick(object sender, object e)
        {
            this.PopupNameUnique.Visibility = Visibility.Collapsed;

            if (CloseTimer != null)
            {
                CloseTimer.Stop();
                CloseTimer.Tick -= CloseTimer_Tick;
                CloseTimer = null;
            }
        }
        #endregion

        #region Textbox Add New Person
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
        #endregion

        #region Textbox Choose Group
        private void TbGroup_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetAddButtonActive();
            if (sender is TextBox textBox)
            {
                if (String.IsNullOrEmpty(textBox.Text)) return;

                int nr = Convert.ToInt32(textBox.Text);
                // Check on input limits if higher or lower set limit value.
                if (nr > Globals.MaxNrOfGroups)
                {
                    nr = Globals.MaxNrOfGroups;
                }
                else if (nr <= 0)
                {
                    nr = 1;
                }
                TbGroup.Text = nr.ToString();
            }
        }

        private void TbGroup_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TbGroup_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TbGroup_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string str = ((TextBox)sender).Text + e.Text;
            // Only numeric input
            e.Handled = !Decimal.TryParse(str, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out _);
        }
        #endregion

        #region Listbox methods
        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox)
            {
                ListBox lb = sender as ListBox;
                int nr = Convert.ToInt32(lb.Name.Substring(lb.Name.Length - 1));

                // Set button enabled because a listbox selection is pressed
                this.BtnRemove.IsEnabled = true;

                // Prevent double event loop cancelling both selection indexs dissapearing
                if (ListBoxMemory == nr)
                {
                    ListBoxMemory = 0;
                    return;
                }

                // Loop through all listboxes 
                for (int i = 0; i < Globals.MaxNrOfGroups; i++)
                {
                    object objToFind = this.FindName("LbGroup" + (i + 1).ToString());
                    if (objToFind is ListBox)
                    {
                        lb = objToFind as ListBox;
                        if ((i + 1) != nr)
                        {
                            if (lb.SelectedIndex != -1)
                            {
                                ListBoxMemory = i + 1;
                            }
                            lb.SelectedIndex = -1;
                        }
                    }
                }
            }
        }
        #endregion

        #region Private methods
        private void SetAddButtonActive()
        {
            if (!string.IsNullOrEmpty(TbNewPerson.Text) &&
                !string.IsNullOrEmpty(TbGroup.Text))
            {
                BtnNewPerson.IsEnabled = true;
            }
            else
            {
                BtnNewPerson.IsEnabled = false;
            }
        }


        private void HookListBoxEvents(bool subscribe)
        {
            int i = 1;
            object objToFind = null;
            ListBox lb = null;
            do
            {
                objToFind = this.FindName("LbGroup" + i.ToString());
                if (objToFind is ListBox)
                {
                    lb = objToFind as ListBox;
                    if (subscribe)
                    {
                        lb.SelectionChanged += ListBoxSelectionChanged;
                    }
                    else
                    {
                        lb.SelectionChanged -= ListBoxSelectionChanged;
                    }
                }

                i++;
            } while (objToFind != null);
        }

        #endregion

       
    }
}
