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
using System.IO.Ports;

namespace WpfScoreboard.Screens
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Page
    {
        private List<Classes.Points> GroupPoints;

        public Config()
        {
            InitializeComponent();
        }

        #region Page loading
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Load points data from sql storage
            GetPoints();
            GetStartDate();

            // Get comm ports
            UpdateCommPorts();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region UI Event methods
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

        private void BtnResetGroupStart_Click(object sender, RoutedEventArgs e)
        {
            Globals.GroupTurn = 1;
        }

        private void DatePickerStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DatePicker picker)
            {
                if (picker.SelectedDate == null)
                {
                    return;
                }
                Globals.Storage.StartDateSummerCamp = DateTime.SpecifyKind(picker.SelectedDate.GetValueOrDefault(), DateTimeKind.Utc);
                Globals.Storage.SettingsReplace(Classes.SettingNames.StartDateSummerCamp, Globals.Storage.StartDateSummerCamp.ToString("d"));
            }
        }

        private void DatePickerCurrentDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DatePicker picker)
            {
                if (picker.SelectedDate == null)
                {
                    return;
                }
                Globals.Storage.CurrentDate = DateTime.SpecifyKind(picker.SelectedDate.GetValueOrDefault(), DateTimeKind.Utc);
                Globals.Storage.SettingsReplace(Classes.SettingNames.CurrentDate, Globals.Storage.CurrentDate.ToString("D"));
            }
        }

        private void CbCommPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded) return;

            if (sender is ComboBox comboBox)
            {
                string item = comboBox.SelectedItem.ToString();

                if (String.IsNullOrEmpty(item)) return;

                Globals.Storage.SettingsReplace(Classes.SettingNames.ComPort, item);
            }
        }
        #endregion


        #region Private methods
        private void GetPoints()
        {
            object objToFind;
            int count = 1;
            bool grpFound;
            GroupPoints = Globals.Storage.GetPoints("*");
            do
            {
                objToFind = this.FindName("TxtBoxGroup" + count.ToString());
                if (objToFind != null)
                {
                    if (objToFind is TextBox textBox)
                    {
                        grpFound = false;
                        // If sql doesn't contain a row for the group create an entrie
                        foreach (Classes.Points points in GroupPoints)
                        {
                            if (points.GroupNr == count)
                            {
                                grpFound = true;
                                textBox.Text = points.GroupPoints.ToString();
                                break;
                            }
                        }
                        if (!grpFound)
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
                if (setting.Name == Classes.SettingNames.StartDateSummerCamp)
                {
                    this.DatePickerStartDate.DisplayDate = DateTime.Parse(setting.Value);
                }
                else if (setting.Name == Classes.SettingNames.CurrentDate)
                {
                    this.DatePickerCurrentDate.DisplayDate = DateTime.Parse(setting.Value);
                }
            }
        }

        private void UpdateCommPorts()
        {
            string[] ports = SerialPort.GetPortNames();

            this.CbCommPorts.Items.Clear();

            foreach (string port in ports)
            {
                this.CbCommPorts.Items.Add(port);
            }

            // Get commport setting from settings 
            string activePort = Globals.Storage.GetCommPort();
            CbCommPorts.SelectedItem = activePort;
        }

        #endregion

        
    }
}
