using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.UI.Popups;


namespace ScoreBoardWelpen.Classes
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
        private SerialDevice serialPort = null;
        DataWriter dataWriteObject = null;
        DataReader dataReaderObject = null;

        private CancellationTokenSource ReadCancellationTokenSource;

        public bool IsOpen
        {
            get { return serialPort != null; }
        }

        public Communication()
        {
            OpenSerialPort();
        }

        #region Public methods
        public async void OpenSerialPort()
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector();
                var deviceInfoList = await DeviceInformation.FindAllAsync(aqs);

                if (deviceInfoList.Count <= 0)
                {
                    return;
                }

                DeviceInformation entry = (DeviceInformation)deviceInfoList[0];

                try
                {
                    serialPort = await SerialDevice.FromIdAsync(entry.Id);
                    if (serialPort == null) return;

                    // Configure serial settings
                    serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                    serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                    serialPort.BaudRate = 9600;
                    serialPort.Parity = SerialParity.None;
                    serialPort.StopBits = SerialStopBitCount.One;
                    serialPort.DataBits = 8;
                    serialPort.Handshake = SerialHandshake.None;

                    // Create cancellation token object to close I/O operations when closing the device
                    ReadCancellationTokenSource = new CancellationTokenSource();

                    //Start listening task:
                    this.Listen();
                }
                catch
                {

                }
            }
            catch //(Exception ex)
            {

            }
        }

        public async void SetLeds(int group, int points)
        {
            string ledMessage = string.Empty;
            ledMessage = 'g' + group.ToString() + 'p' + points.ToString("D3");
            WriteSerial(ledMessage);
        }

        public async void WriteSerial(string message)
        {
            try
            {
                if (serialPort != null && message != null)
                {
                    // Create the DataWriter object and attach to OutputStream
                    dataWriteObject = new DataWriter(serialPort.OutputStream);

                    //Launch the WriteAsync task to perform the write
                    await WriteAsync(message);

                }
            }
            finally
            {
                // Cleanup once complete
                if (dataWriteObject != null)
                {
                    dataWriteObject.DetachStream();
                    dataWriteObject = null;
                }
            }
            
        }

        private async Task WriteAsync(string message)
        {
            Task<UInt32> storeAsyncTask;

            if (message.Length > 0)
            {
                dataWriteObject.WriteString(message);
                storeAsyncTask = dataWriteObject.StoreAsync().AsTask();

                UInt32 bytesWritten = await storeAsyncTask;
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
        private async void Listen()
        {
            try
            {
                if (serialPort != null)
                {
                    dataReaderObject = new DataReader(serialPort.InputStream);

                    // keep reading the serial input
                    while (true)
                    {
                        await ReadAsync(ReadCancellationTokenSource.Token);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                CloseDevice();
            }
            catch (Exception)
            {
                CloseDevice();
            }
            finally
            {
                // Cleanup once complete
                if (dataReaderObject != null)
                {
                    dataReaderObject.DetachStream();
                    dataReaderObject = null;
                }
            }
        }

        /// <summary>
        /// ReadAsync: Task that waits on data and reads asynchronously from the serial device InputStream
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;

            uint ReadBufferLength = 1024;

            // If task cancellation was requested, comply
            cancellationToken.ThrowIfCancellationRequested();

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            using (var childCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                // Create a task object to wait for data on the serialPort.InputStream
                loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(childCancellationTokenSource.Token);

                // Launch the task and wait
                UInt32 bytesRead = await loadAsyncTask;
                if (bytesRead > 0)
                {
                    // Fire event with the read data:
                    string data = dataReaderObject.ReadString(bytesRead);
                    //rcvdText.Text = dataReaderObject.ReadString(bytesRead);
                }
            }
        }

        /// <summary>
        /// CancelReadTask:
        /// - Uses the ReadCancellationTokenSource to cancel read operations
        /// </summary>
        private void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
        }
        #endregion
    }
}
