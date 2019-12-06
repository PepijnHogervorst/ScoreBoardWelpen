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
        private int listBoxMemory = 0;
        public GroupEdit()
        {
            this.InitializeComponent();
        }

        #region Page loading methods
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Event hooks:
            hookEvents();

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
            unhookEvents();
        }
        #endregion

        #region Buttons
        private void BtnNewPerson_Click(object sender, RoutedEventArgs e)
        {
            // Add new person to listbox
            object objToFind = this.FindName("LbGroup" + TbGroup.Text);
            if (objToFind is ListBox)
            {
                ListBox lb = objToFind as ListBox;
                lb.Items.Add(TbNewPerson.Text);
            }

            // Add new person to database
            Globals.Storage.AddPerson(Convert.ToInt32(TbGroup.Text), TbNewPerson.Text);

            // Clear entry fields
            this.TbNewPerson.Text = "";
            this.TbGroup.Text = "";
            this.BtnNewPerson.IsEnabled = false;
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
        #endregion

        #region EVENTS
        private void hookEvents()
        {
            HookListBoxEvents(true);
        }

        private void unhookEvents()
        {
            HookListBoxEvents(false);
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
            if (e.OriginalSource is TextBox)
            {
                TextBox textBox = e.OriginalSource as TextBox;
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

        // Prevent user to set any other input then numbers into the grp textbox
        private void TbGroup_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
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
                if(listBoxMemory == nr)
                {
                    listBoxMemory = 0;
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
                            if(lb.SelectedIndex != -1)
                            {
                                listBoxMemory = i + 1;
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
