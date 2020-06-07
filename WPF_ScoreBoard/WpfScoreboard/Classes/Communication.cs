using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfScoreboard.Classes
{
    /// <summary>
    /// Communication class that uses on board gpio of raspberry pi to communicate to a arduino
    /// Connections on raspberry pi: 
    /// pin  8 = TX     = BLUE
    /// pin 10 = RX     = PURPLE
    /// pin 14 = gnd    = GRAY
    /// </summary>
    public class Communication
    {
        #region Event raising
        public event EventHandler<EventArgs> DataReceived;
        #endregion

        #region Private properties
        private SerialPort serialPort = null;
        #endregion

        #region Public properties
        public bool IsOpen
        {
            get { return serialPort != null; }
        }
        #endregion

        #region Constructor
        public Communication()
        {
            OpenSerialPort();
        }
        #endregion

        #region Public methods
        public void OpenSerialPort()
        {
            try
            {
                
            }
            catch //(Exception ex)
            {

            }
        }

        public void SetLeds(int group, int points)
        {
            string ledMessage = string.Empty;
            ledMessage = 'g' + group.ToString() + 'p' + points.ToString("D3");
            WriteSerial(ledMessage);
            // Start task to listen to incoming reply from arduino
            // Arduino sends DONE when leds are set!
            
        }

        public void WriteSerial(string message)
        {
            if (serialPort != null && message != null)
            {
                // Write message over serial port
            }
        }

        /// <summary>
        /// CloseDevice:
        /// - Disposes SerialDevice object
        /// - Clears the enumerated device Id list
        /// </summary>
        public void CloseDevice()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
            }
            serialPort = null;
        }
        #endregion

        #region Private methods
        
        #endregion
    }
}
