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
    public sealed partial class Scoreboard : Page
    {
        private DispatcherTimer timer = null;
        private DateTimeOffset startSummerCamp;
        private DateTimeOffset currentDate;
        private List<Classes.Groups> groups;
        private List<Classes.Points> points;

        private bool timerFlag = false;

        public Scoreboard()
        {
            this.InitializeComponent();
        }
        
        #region Page loading methods
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            hookEvents();

            InitTimer();

            displayPerson();

            this.TxtPoints.Visibility = Visibility.Collapsed;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            unhookEvents();
            
            timer.Tick -= Timer_Tick;

            // Dispose objects
            timer.Stop();
            timer = null;
        }
        #endregion

        #region Events
        private void hookEvents()
        {
            Globals.GPIO.ArcadeBtnPressed += GPIO_ArcadeBtnPressed;

        }
        private void unhookEvents()
        {
            Globals.GPIO.ArcadeBtnPressed -= GPIO_ArcadeBtnPressed;
        }

        private void GPIO_ArcadeBtnPressed(Windows.Devices.Gpio.GpioPin sender, Windows.Devices.Gpio.GpioPinValueChangedEventArgs args)
        {
            this.TxtPoints.Text = "";
            this.TxtPressBtn.Text = "Knop ingedrukt! Punten totaal:";

            // Write serial to the arduino the group nr and the points total
            Globals.Communication.SetLeds(1, 99);

            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            if (!timerFlag)
            {
                this.TxtPoints.Visibility = Visibility.Visible;
                if(Globals.GroupTurn >= Globals.MaxNrOfGroups)
                {
                    Globals.GroupTurn = 1;
                }
                else
                {
                    Globals.GroupTurn++;
                }
                timerFlag = true;
            }
            else
            {
                timer.Stop();
                displayPerson();
                timerFlag = false;
                this.TxtPressBtn.Visibility = Visibility.Visible;
                this.BtnArcade.IsEnabled = true;
            }
        }
        #endregion

        #region Button events
        private void BtnArcade_Click(object sender, RoutedEventArgs e)
        {
            // Write points to arduino
            int pointsToWrite = 0;
            foreach (Classes.Points point in points)
            {
                if(point.GroupNr == Globals.GroupTurn)
                {
                    pointsToWrite = point.GroupPoints;
                    break;
                }
            }
            Globals.Communication.SetLeds(Globals.GroupTurn, pointsToWrite);
            this.TxtPoints.Visibility = Visibility.Collapsed;
            this.TxtPoints.Text = pointsToWrite.ToString();

            //Disable the button 
            this.BtnArcade.IsEnabled = false;
            this.TxtPressBtn.Visibility = Visibility.Collapsed;
            this.TxtPoints.Visibility = Visibility.Collapsed;
            // Start timer that shows points after interval
            timer.Start();
        }
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
            
            if(days_diff < 0)
            {
                days_diff = System.Math.Abs(days_diff);
            }
            List<Classes.Groups> myGroup = GetGroup(Globals.GroupTurn);
            if(myGroup.Count == 0 )
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
                if(mygroup.GroupNr == group)
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
                if(group.GroupNr == groupNr)
                {
                    mygroups.Add(group);
                }
            }

            return mygroups;
        }
        #endregion

        
    }
}
