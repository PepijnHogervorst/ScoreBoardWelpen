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
    public sealed partial class Device : Page
    {
        private int counter = 0;
        private List<Classes.Points> GroupPoints;

        public Device()
        {
            this.InitializeComponent();
        }

        #region Page loading
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Load points data from sql storage
            GetPoints();
            GetStartDate();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }        
        #endregion

        #region EVENTS
        private async void IO_ArcadeBtnPressed(Windows.Devices.Gpio.GpioPin sender, Windows.Devices.Gpio.GpioPinValueChangedEventArgs args)
        {
            counter++;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                //UI code here
                this.TxtFeedback.Text = $"Button with pin {sender.PinNumber} number of times pressed: {counter}";
            });

        }

        private void TxtBoxGroup_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox textBox = sender as TextBox;
                int nr = 0;
                if (int.TryParse(textBox.Text, out nr))
                {
                    // Check which textbox changed value
                    string bufNr = textBox.Name.Substring(textBox.Name.Length - 1);
                    int groupNr;
                    if (int.TryParse(bufNr, out groupNr))
                    {
                        // Update sqlite
                        Globals.Storage.ReplacePoints(groupNr, nr);
                    }
                }
            }
        }

        private void TxtBoxGroup_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void DatePickerStartDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            Globals.Storage.SettingsReplace(Classes.Storage.SettingNames.StartDateSummerCamp ,e.NewDate.ToString("d"));
        }

        private void DatePickerCurrentDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            Globals.Storage.SettingsReplace(Classes.Storage.SettingNames.CurrentDate, e.NewDate.ToString("d"));
        }

        private void BtnResetGroupStart_Click(object sender, RoutedEventArgs e)
        {
            Globals.GroupTurn = 1;
        }
        #endregion

        #region Private methods
        private void GetPoints()
        {
            object objToFind = null;
            int count = 1;
            bool grpFound = false;
            GroupPoints = Globals.Storage.GetPoints("*");
            do
            {
                objToFind = this.FindName("TxtBoxGroup" + count.ToString());
                if(objToFind != null)
                {
                    if(objToFind is TextBox)
                    {
                        TextBox textBox = objToFind as TextBox;
                        grpFound = false;
                        // If sql doesn't contain a row for the group create an entrie
                        foreach (Classes.Points points in GroupPoints)
                        {
                            if(points.GroupNr == count)
                            {
                                grpFound = true;
                                textBox.Text = points.GroupPoints.ToString();
                                break;
                            }
                        }
                        if(!grpFound)
                        {
                            // Add entrie of group in database.
                            Globals.Storage.ReplacePoints(count, 0);
                            textBox.Text = "0";
                        }
                    }
                }
                count++;
            } while (objToFind != null);
        }

        private void GetStartDate()
        {
            List<Classes.Setting> settings = Globals.Storage.SettingsGet("*");
            foreach (Classes.Setting setting in settings)
            {
                if (setting.Name == Classes.Storage.SettingNames.StartDateSummerCamp)
                {
                    this.DatePickerStartDate.Date = DateTimeOffset.Parse(setting.Value);
                }
                else if (setting.Name == Classes.Storage.SettingNames.CurrentDate)
                {
                    this.DatePickerCurrentDate.Date = DateTimeOffset.Parse(setting.Value);
                }
            }
        }


        #endregion

        
    }
}
