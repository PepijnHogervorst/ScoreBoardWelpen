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
        public Scoreboard()
        {
            this.InitializeComponent();
        }
        
        #region Page loading methods
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Globals.GPIO.ArcadeBtnPressed += GPIO_ArcadeBtnPressed;
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Stop();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unhook events
            Globals.GPIO.ArcadeBtnPressed -= GPIO_ArcadeBtnPressed;
            timer.Tick -= Timer_Tick;

            // Dispose objects
            timer.Stop();
            timer = null;
        }
        #endregion

        #region Events
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
            timer.Stop();
        }
        #endregion

        private void ButtonWrite_Click(object sender, RoutedEventArgs e)
        {
            if(Globals.Communication.IsOpen)
            {
                Globals.Communication.WriteSerial(sender.ToString());
            }
        }

        private async void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            Globals.Communication.OpenSerialPort();
            if (!Globals.Communication.IsOpen)
            {
                
            }
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            if(Globals.Communication.IsOpen)
            {
                Globals.Communication.CloseDevice();
            }
        }

        
    }
}
