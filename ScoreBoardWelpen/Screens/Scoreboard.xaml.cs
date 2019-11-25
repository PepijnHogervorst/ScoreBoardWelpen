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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ScoreBoardWelpen.Screens
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scoreboard : Page
    {
        public Scoreboard()
        {
            this.InitializeComponent();
        }

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
