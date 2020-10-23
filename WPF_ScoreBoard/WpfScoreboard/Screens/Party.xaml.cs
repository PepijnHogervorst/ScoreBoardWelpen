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

            // Set brightness slider
            if (Globals.MQTTClient.Brightness != -1)
            {
                SliderBrightness.Value = Globals.MQTTClient.Brightness;
            }
        }

        #region Screen loading event methods
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Globals.MQTTClient.ClearLEDs();
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

            Globals.MQTTClient.SetBrightness((int)SliderBrightness.Value);
        }

        private void SliderStrobe_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }
            // Publish mqtt message
            //Globals.MQTTClient.SetStrobe((int)SliderStrobe.Value);
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
            Globals.MQTTClient.SetParty((int)program, (int)SliderSpeed.Value);
            ActiveProgram = program;
        }
        #endregion

    }
}
