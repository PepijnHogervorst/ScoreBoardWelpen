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

namespace WpfScoreboard.Screens
{
    /// <summary>
    /// Interaction logic for Party.xaml
    /// </summary>
    public partial class Party : Page
    {
        
        public enum PartyProgram
        {
            FullRainbow = 0,
            RandomRain,
            ColorWipe,
        }

        private PartyProgram ActiveProgram = PartyProgram.FullRainbow;

        public Party()
        {
            InitializeComponent();
        }

        #region Screen loading event methods
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Globals.Communication.OpenSerialPort();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Globals.Communication.ClearLEDs();
        }
        #endregion

        #region Radio button event methods
        private void RbFullRainbow_Click(object sender, RoutedEventArgs e)
        {
            SetPartyProgram(PartyProgram.FullRainbow);
        }

        private void RbRain_Click(object sender, RoutedEventArgs e)
        {
            SetPartyProgram(PartyProgram.RandomRain);
        }

        private void RbColorWipe_Click(object sender, RoutedEventArgs e)
        {
            SetPartyProgram(PartyProgram.ColorWipe);
        }
        #endregion

        #region Slider event methods
        private void SliderBrightness_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            string value = ((int)SliderBrightness.Value).ToString("D4");
            // Write value to controller
            Globals.Communication.WriteSerial($"bxx{value}");
        }

        private void SliderStrobe_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            string value = ((int)SliderStrobe.Value).ToString("D3");
            // Write value to controller
            Globals.Communication.WriteSerial($"SSxx{value}");
        }

        private void SliderSpeed_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            // Check what program is selected and send that with the new speed to arduino
            SetPartyProgram(ActiveProgram);
        }
        #endregion

        #region Private methods
        private void SetPartyProgram(PartyProgram program)
        {
            string msg = "p" + ((int)program).ToString("D1") + ((int)SliderSpeed.Value).ToString("D3");
            msg = msg.PadRight(7, '0');
            Globals.Communication.WriteSerial(msg);
            ActiveProgram = program;
        }
        #endregion

    }
}
