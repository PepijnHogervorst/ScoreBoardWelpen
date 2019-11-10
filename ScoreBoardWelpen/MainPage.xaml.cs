﻿using System;
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
        public Classes.GPIO IO;

        private int counter = 0;

        public MainPage()
        {
            this.InitializeComponent();

            IO = new Classes.GPIO();
            //
            if (IO.HasGPIO)
            {
                IO.ArcadeBtnPressed += IO_ArcadeBtnPressed;
            }
        }

        private async void IO_ArcadeBtnPressed(Windows.Devices.Gpio.GpioPin sender, Windows.Devices.Gpio.GpioPinValueChangedEventArgs args)
        {
            counter++;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                //UI code here
                this.TxtPressed.Text = $"Button with pin {sender.PinNumber} number of times pressed: {counter}";
            });
            
        }
    }
}
