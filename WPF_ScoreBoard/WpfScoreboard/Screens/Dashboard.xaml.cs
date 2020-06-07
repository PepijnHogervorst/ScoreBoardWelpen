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

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            this.UnhookEvents();
        }
        #endregion

        #region Event handling methods
        private void HookEvents()
        {
            Globals.Communication.DataReceived += Communication_DataReceived;
        }

        private void UnhookEvents()
        {
            Globals.Communication.DataReceived -= Communication_DataReceived;
        }

        private void Communication_DataReceived(object sender, EventArgs e)
        {
            // When reply is received, set the points total visible,
            // Arduino is done when serial is send
            this.TxtPoints.Visibility = Visibility.Visible;
            if (Globals.GroupTurn >= Globals.MaxNrOfGroups)
            {
                Globals.GroupTurn = 1;
            }
            else
            {
                Globals.GroupTurn++;
            }
            // Start timer to show new group after x seconds
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            timer.Stop();
            displayPerson();
            this.TxtPressBtn.Visibility = Visibility.Visible;

            // Update group and send info to arduino
            if (Globals.GroupTurn >= Globals.MaxNrOfGroups)
            {
                this.BtnStart.Visibility = Visibility.Visible;
                this.BtnRetry.Visibility = Visibility.Collapsed;
                return;
            }
            Globals.GroupTurn++;
            SendPointsInfo();
        }

        #region Button events
        private void BtnRetry_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {

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
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Stop();
        }

        private void displayPerson()
        {
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
                days_diff = System.Math.Abs(days_diff);
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
                if (setting.Name == Classes.Storage.SettingNames.StartDateSummerCamp)
                {
                    startSummerCamp = DateTimeOffset.Parse(setting.Value);
                }
                else if (setting.Name == Classes.Storage.SettingNames.CurrentDate)
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
            Globals.Communication.SetLeds(Globals.GroupTurn, pointsToWrite);
            this.TxtPoints.Visibility = Visibility.Collapsed;
            this.TxtPoints.Text = pointsToWrite.ToString();

            //Disable the button 
            this.TxtPressBtn.Visibility = Visibility.Collapsed;
            this.TxtPoints.Visibility = Visibility.Collapsed;
        }
        #endregion


    }
}
