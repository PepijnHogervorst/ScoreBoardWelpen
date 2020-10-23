using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
            
        }
        #endregion

        #region Public methods
        public bool OpenSerialPort()
        {
            // Get port from database
            string port = Globals.Storage.GetCommPort();

            if (port == null)
            {
                // Get port that is available
                string[] ports = SerialPort.GetPortNames();
                if (ports.Length > 0)
                {
                    port = ports[0];
                    Globals.Storage.SettingsReplace(SettingNames.ComPort, port);
                }
                else
                {
                    return false;
                }
            }
            try
            {
                serialPort = new SerialPort(port);
                serialPort.BaudRate = 9600;
                serialPort.Parity = Parity.None;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening COM port");
                return false;
            }
            return true;
        }

        public void SetLeds(int group, int points)
        {
            string ledMessage;
            ledMessage = "gg" + group.ToString() + 'p' + points.ToString("D3");
            WriteSerial(ledMessage);
        }

        public void ClearLEDs()
        {
            string msg = "cc00000";
            WriteSerial(msg);
        }

        public void WriteSerial(string message)
        {
            if (serialPort == null)
            {
                OpenSerialPort();
            }
            if (serialPort != null && message != null)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Write(message);
                }
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
                try
                {
                    serialPort.Close();
                    serialPort.Dispose();
                }
                catch {}
                serialPort = null;
            }
        }
        #endregion

        #region Private methods
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.DataReceived?.Invoke(this, new EventArgs());
        }
        #endregion
    }
}
