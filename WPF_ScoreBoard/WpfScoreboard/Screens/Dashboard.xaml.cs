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
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        #region Private properties
        private DispatcherTimer timer = null;
        private DateTimeOffset startSummerCamp;
        private DateTimeOffset currentDate;
        private List<Classes.Groups> groups;
        private List<Classes.Points> points;
        #endregion

        #region Public properties

        #endregion

        #region Constructor
        public Dashboard()
        {
            InitializeComponent();
        }
        #endregion

        #region Page loading events
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.HookEvents();

            this.InitTimer();

            displayPerson();

            // Set UI elements
            this.TxtPoints.Visibility = Visibility.Collapsed;
            this.SpOverview.Visibility = Visibility.Collapsed;

            Globals.MQTTClient.ClearLEDs();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            this.UnhookEvents();
        }
        #endregion

        #region Event handling methods
        private void HookEvents()
        {
            Globals.MQTTClient.OnReadyReceived += MQTTClient_OnReadyReceived;
        }

        private void UnhookEvents()
        {
            Globals.MQTTClient.OnReadyReceived -= MQTTClient_OnReadyReceived;
        }

        private void MQTTClient_OnReadyReceived(object sender, EventArgs e)
        {
            // When ready is received, set the points total visible,
            // Arduino is done when serial is send
            Dispatcher.Invoke(() =>
            {
                this.TxtPoints.Visibility = Visibility.Visible;
                this.TxtPressBtn.Visibility = Visibility.Collapsed;
            });

            // Start timer to show new group after x seconds
            timer.Start();
        }

        private void Communication_DataReceived(object sender, EventArgs e)
        {
            // When reply is received, set the points total visible,
            // Arduino is done when serial is send
            Dispatcher.Invoke(() =>
            {
                this.TxtPoints.Visibility = Visibility.Visible;
                this.TxtPressBtn.Visibility = Visibility.Collapsed;
            });

            // Start timer to show new group after x seconds
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            timer.Stop();
            // Update group turn count
            if (Globals.GroupTurn >= Globals.MaxNrOfGroups)
            {
                this.TxtPoints.Visibility = Visibility.Hidden;
                this.BtnStart.Visibility = Visibility.Visible;
                this.BtnRetry.Visibility = Visibility.Collapsed;
                this.TxtPressBtn.Visibility = Visibility.Visible;
                this.TxtGroup.Visibility = Visibility.Collapsed;
                this.TxtName.Visibility = Visibility.Collapsed;
                this.TxtGroupTxt.Visibility = Visibility.Collapsed;

                TimeSpan difference = currentDate - startSummerCamp;
                int days_diff = (difference.TotalDays > 0 ?
                    (int)Math.Floor(difference.TotalDays) : (int)Math.Ceiling(difference.TotalDays));
                // If the week is over (friday night, 6 days done) show a text who won
                if (days_diff >= 6)
                {
                    this.TxtPressBtn.Text = ShowWinner();
                }
                else
                {
                    this.TxtPressBtn.Text = "Succes met punten verdienen!";
                }
                
                Globals.GroupTurn = 1;
                // Toggle overview points 
                ShowPointsOverview();
                return;
            }
            else
            {
                Globals.GroupTurn++;
            }

            // Display the person that is on that team
            displayPerson();
            this.TxtPressBtn.Visibility = Visibility.Visible;
            // Update group and send info to arduino
            SendPointsInfo();
        }

        #region Button events
        private void BtnRetry_Click(object sender, RoutedEventArgs e)
        {
            SendPointsInfo();
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Globals.GroupTurn = 1;
            // Clear LEDS
            Globals.MQTTClient.ClearLEDs();

            await Task.Delay(1000);

            displayPerson();
            SendPointsInfo();
            this.BtnStart.Visibility = Visibility.Collapsed;
            this.TxtPressBtn.Visibility = Visibility.Visible;
            this.BtnRetry.Visibility = Visibility.Visible;
            this.SpOverview.Visibility = Visibility.Collapsed;
            this.TxtPressBtn.Text = "Druk op de knop!";
        }


        #endregion

        #endregion

        #region Public methods

        #endregion

        #region Private methods
        private void InitTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(7);
            timer.Stop();
        }

        private void displayPerson()
        {
            this.TxtGroupTxt.Visibility = Visibility.Visible;
            this.TxtGroup.Visibility = Visibility.Visible;
            this.TxtName.Visibility = Visibility.Visible;

            // Update group nr text
            this.TxtGroup.Text = Globals.GroupTurn.ToString();

            updateDates();
            updateGroups();
            updatePoints();

            // Select person from that group with offset depending on what day it is
            TimeSpan difference = currentDate - startSummerCamp;
            int days_diff = (difference.TotalDays > 0 ?
                (int)Math.Floor(difference.TotalDays) : (int)Math.Ceiling(difference.TotalDays));

            if (days_diff < 0)
            {
                days_diff = Math.Abs(days_diff);
            }
            List<Classes.Groups> myGroup = GetGroup(Globals.GroupTurn);
            if (myGroup.Count == 0)
            {
                this.TxtName.Text = "";
                this.TxtGroup.Text = "";
                return;
            }
            int offset = days_diff % myGroup.Count;


            this.TxtName.Text = myGroup[offset].Name;

        }

        private void updateDates()
        {
            List<Classes.Setting> settings = Globals.Storage.SettingsGet("*");
            foreach (Classes.Setting setting in settings)
            {
                if (setting.Name == Classes.SettingNames.StartDateSummerCamp)
                {
                    startSummerCamp = DateTimeOffset.Parse(setting.Value);
                }
                else if (setting.Name == Classes.SettingNames.CurrentDate)
                {
                    currentDate = DateTimeOffset.Parse(setting.Value);
                }
            }
        }

        private void updateGroups()
        {
            groups = Globals.Storage.GetGroups("*");
        }

        private void updatePoints()
        {
            points = Globals.Storage.GetPoints("*");
        }

        private int TotalPersonsInGroup(int group)
        {
            int count = 0;

            foreach (Classes.Groups mygroup in groups)
            {
                if (mygroup.GroupNr == group)
                {
                    count++;
                }
            }
            return count;
        }

        private List<Classes.Groups> GetGroup(int groupNr)
        {
            List<Classes.Groups> mygroups = new List<Classes.Groups>();

            foreach (Classes.Groups group in groups)
            {
                if (group.GroupNr == groupNr)
                {
                    mygroups.Add(group);
                }
            }

            return mygroups;
        }

        private void SendPointsInfo()
        {
            // Write points to arduino
            int pointsToWrite = 0;
            foreach (Classes.Points point in points)
            {
                if (point.GroupNr == Globals.GroupTurn)
                {
                    pointsToWrite = point.GroupPoints;
                    break;
                }
            }
            // Write score to arduino over MQTT
            Globals.MQTTClient.SetScore(Globals.GroupTurn, pointsToWrite);

            this.TxtPoints.Visibility = Visibility.Collapsed;
            this.TxtPoints.Text = pointsToWrite.ToString();
        }

        private void ShowPointsOverview()
        {
            // Set points
            foreach (Classes.Points groupPoints in points)
            {
                switch (groupPoints.GroupNr)
                {
                    case 1:
                        this.TxtOverviewGrp1.Text = groupPoints.GroupPoints.ToString("D2");
                        break;
                    case 2:
                        this.TxtOverviewGrp2.Text = groupPoints.GroupPoints.ToString("D2");
                        break;
                    case 3:
                        this.TxtOverviewGrp3.Text = groupPoints.GroupPoints.ToString("D2");
                        break;
                    case 4:
                        this.TxtOverviewGrp4.Text = groupPoints.GroupPoints.ToString("D2");
                        break;
                    case 5:
                        this.TxtOverviewGrp5.Text = groupPoints.GroupPoints.ToString("D2");
                        break;
                    case 6:
                        this.TxtOverviewGrp6.Text = groupPoints.GroupPoints.ToString("D2");
                        break;
                    default:
                        break;
                }
            }
            // Set UI object visible
            this.SpOverview.Visibility = Visibility.Visible;
        }

        private string ShowWinner()
        {
            Classes.Points highestPoints = new Classes.Points
            {
                GroupPoints = 0
            };
            // Loop through all groups to get points
            foreach (Classes.Points groupPoints in points)
            {
                if (groupPoints.GroupPoints > highestPoints.GroupPoints)
                {
                    highestPoints = groupPoints;
                }
            }

            return $"Gefeliciteerd groepje {highestPoints.GroupNr}!!!";
        }
        #endregion
    }
}
