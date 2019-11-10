using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;

namespace ScoreBoardWelpen.Classes
{
    public class GPIO
    {
        public event TypedEventHandler<GpioPin, GpioPinValueChangedEventArgs> ArcadeBtnPressed;

        private GpioController gpio;
        private const int ARCADE_BUTTON_PIN = 2;
        private const int ARCADE_LED_PIN = 3;
        private GpioPin ArcadeBtn;
        private GpioPin ArcadeLED;

        public bool HasGPIO
        {
            get { return (gpio != null); }
        }

        public GPIO()
        {
            // On creation check if GPIO is available
            gpio = GpioController.GetDefault();

            if(gpio == null)
            {
                //No gpio controller found so 
                return;
            }
            //Open and setup pins that will be used:
            ArcadeBtn = gpio.OpenPin(ARCADE_BUTTON_PIN);
            ArcadeBtn.Write(GpioPinValue.Low);                  //Make sure pin is low before opening it as input
            ArcadeBtn.SetDriveMode(GpioPinDriveMode.Input);
            ArcadeBtn.ValueChanged += ArcadeBtn_ValueChanged;

            ArcadeLED = gpio.OpenPin(ARCADE_LED_PIN);
            ArcadeLED.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void ArcadeBtn_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            ArcadeBtnPressed?.Invoke(sender, args);
        }

        ~GPIO()
        {
            this.ArcadeBtn.Dispose();
            this.ArcadeLED.Dispose();
        }
    }
}
