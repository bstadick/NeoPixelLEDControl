using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;

namespace NeopixelSDK
{
    /// <summary>
    /// Event is triggered with the serial port has received more bytes then the specified threshold
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DataReceiveEventHandler(object sender, DataRecievedEventArgs e);

    /// <summary>
    /// Creates and opens a new connection to the serial port
    /// </summary>
    public class Serial
    {

        private SerialPort _serial = new SerialPort();
        private string receivedMessage = "";
        private string messageBuffer = "";
        private List<string> ports = new List<string>();
        private bool eventsEnabled = false;
        private bool newMessage = false;
        private bool internalCall = false;

        /// <summary>
        /// Possible baud rates to communicate at
        /// </summary>
        public enum BaudRateEnum {b75 = 75, b110 = 110, b300 = 300, b1200 = 1200, b2400 = 2400, b4800 = 4800, b9600 = 9600, b19200 = 19200, b38400 = 38400, b57600 = 57600, b115200 = 115200};

        #region Properties
        /// <summary>
        /// Gets the status of the port connection
        /// </summary>
        public bool IsOpen
        {
            get
            {
                if(_serial != null)
                    return this._serial.IsOpen;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets or sets the name of the port to connect to
        /// </summary>
        public string PortName
        {
            get
            {
                return this._serial.PortName;
            }
            set
            {
                if(value != "" && value != null && ports.Contains(value))
                    this._serial.PortName = value;
            }
        }

        /// <summary>
        /// Gets the messages received over the serial connection
        /// </summary>
        public string ReceivedMessage
        {
            get
            {
                return this.receivedMessage;
            }
            private set
            {
                LastReceivedMessage = receivedMessage;
                receivedMessage = value;
            }
        }

        /// <summary>
        /// Gets or sets the message buffer
        /// </summary>
        private string MessageBuffer
        {
            get
            {
                string temp = this.messageBuffer;
                messageBuffer = "";
                return temp;
            }
            set
            {
                messageBuffer += value;
            }
        }

        /// <summary>
        /// Gets the previously received message
        /// </summary>
        public string LastReceivedMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the number of bytes to receive before an event is triggered
        /// </summary>
        public int ReceivedBytesThreshold
        {
            get { return _serial.ReceivedBytesThreshold; }
            set
            {
                if (value <= 0)
                    _serial.ReceivedBytesThreshold = 1;
                else
                    _serial.ReceivedBytesThreshold = value;
            }
        }

        /// <summary>
        /// Gets the serial ports available on the current machine
        /// </summary>
        public List<string> AvailablePorts
        {
            get
            {
                ports.RemoveRange(0, ports.Count);
                foreach (string s in SerialPort.GetPortNames())
                {
                    ports.Add(s);
                }

                return ports;
            }
        }

        /// <summary>
        /// Gets or sets if events should be used in the class
        /// </summary>
        public bool EnableEvents
        {
            get
            {
                return eventsEnabled;
            }
            set
            {
                if (value && !eventsEnabled && !internalCall)
                {
                    eventsEnabled = true;
                    _serial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(receive);
                }
                else if(!value && eventsEnabled && !internalCall)
                {
                    eventsEnabled = false;
                    _serial.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(receive);
                }
            }
        }

        /// <summary>
        /// Gets the number of bytes in the receive buffer
        /// </summary>
        public int BytesToReceive
        {
            get { return _serial.BytesToRead; }
        }

        /// <summary>
        /// Gets the number of bytes in the send buffer
        /// </summary>
        public int BytesToSend
        {
            get { return _serial.BytesToWrite; }
        }

        /// <summary>
        /// Gets or sets whether the port should disregard null characters
        /// </summary>
        public bool DiscardNull
        {
            get { return _serial.DiscardNull; }
            set { _serial.DiscardNull = value; }
        }

        /// <summary>
        /// Gets or sets the baud rate
        /// </summary>
        public BaudRateEnum BaudRate
        {
            get { return (BaudRateEnum)_serial.BaudRate; }
            set { _serial.BaudRate = (int)value; }
        }

        /// <summary>
        /// Gets or set the serial port handshake method
        /// </summary>
        public Handshake Handshake
        {
            get { return _serial.Handshake; }
            set { _serial.Handshake = value; }
        }

        /// <summary>
        /// Gets or set the serial port parity
        /// </summary>
        public Parity Parity
        {
            get { return _serial.Parity; }
            set { _serial.Parity = value; }
        }

        /// <summary>
        /// Get or sets the serial port stop bit
        /// </summary>
        public StopBits StopBits
        {
            get { return _serial.StopBits; }
            set { _serial.StopBits = value; }
        }

        /// <summary>
        /// Gets or set the serial port receive timeout
        /// </summary>
        public int ReadTimeout
        {
            get { return _serial.ReadTimeout; }
            set { _serial.ReadTimeout = value; }
        }

        /// <summary>
        /// Gets or set the serial port send timeout
        /// </summary>
        public int WriteTimeout
        {
            get { return _serial.WriteTimeout; }
            set { _serial.WriteTimeout = value; }
        }
    #endregion

        #region Constructors
        /// <summary>
        /// Makes a new port connection without a port associated with it
        /// </summary>
        public Serial()
        {
            createSerialPort("", BaudRateEnum.b9600, Handshake.None, Parity.None, 8, StopBits.One, 200, 50);
        }

        /// <summary>
        /// Makes a new port connection using 9600 baud and 8N1
        /// </summary>
        /// <param name="port">The name of the port</param>
        public Serial(string port)
        {
            createSerialPort(port, BaudRateEnum.b9600, Handshake.None, Parity.None, 8, StopBits.One, 200, 50);
        }

        /// <summary>
        /// Makes a new port connection using 8N1
        /// </summary>
        /// <param name="port">The name of the port</param>
        /// <param name="baud">The baud rate</param>
        public Serial(string port, BaudRateEnum baud)
        {
            createSerialPort(port, baud, Handshake.None, Parity.None, 8, StopBits.One, 200, 50);
        }

        /// <summary>
        /// Make a fully customized port connection
        /// </summary>
        /// <param name="port">The name of the port</param>
        /// <param name="baud">The baud rate</param>
        /// <param name="shake">The shake method</param>
        /// <param name="parity">The parity method</param>
        /// <param name="dataBits">The number of bits per piece of data</param>
        /// <param name="stopBits">The stop bit</param>
        /// <param name="readTimeout">The timeout, in milliseconds, before a receive operation terminates</param>
        /// <param name="writeTimeout">The timeout, in milliseconds, before a send operation terminates</param>
        public Serial(string port, BaudRateEnum baud, Handshake shake, Parity parity, int dataBits, StopBits stopBits, int readTimeout, int writeTimeout)
        {
            createSerialPort(port, baud, shake, parity, dataBits, stopBits, readTimeout, writeTimeout);
        }
        
        /// <summary>
        /// The universal serial port creator
        /// </summary>
        /// <param name="port">The name of the port</param>
        /// <param name="baud">The baud rate</param>
        /// <param name="shake">The shake method</param>
        /// <param name="parity">The parity method</param>
        /// <param name="dataBits">The number of bits per piece of data</param>
        /// <param name="stopBits">The stop bit</param>
        /// <param name="readTimeout">The timeout, in milliseconds, before a receive operation terminates</param>
        /// <param name="writeTimeout">The timeout, in milliseconds, before a send operation terminates</param>
        private void createSerialPort(string port, BaudRateEnum baud, Handshake shake, Parity par, int dataBits, StopBits stop, int readTimeout, int writeTimeout)
        {
            _serial = new SerialPort();

            if (port != "" && port != null && AvailablePorts.Contains(port))
                _serial.PortName = port;
            _serial.BaudRate = (int)baud; //COM Port Sp
            _serial.Handshake = shake;
            _serial.Parity = par;
            _serial.DataBits = dataBits;
            _serial.StopBits = stop;
            _serial.ReadTimeout = readTimeout;
            _serial.WriteTimeout = writeTimeout;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Serial()
        {
            this.Close();
        }
               
        #endregion

        #region Methods

        /// <summary>
        /// Opens the serial port connection
        /// </summary>
        /// <returns>if the connection was successful</returns>
        public bool Open()
        {
            try
            {
                _serial.Open();
            }
            catch (Exception e)
            {
                return false;
            }

            return this.IsOpen;
        }

        /// <summary>
        /// Closes the serial port connection
        /// </summary>
        public bool Close()
        {
            try
            {
                _serial.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Clears the received data buffer
        /// </summary>
        /// <returns>if the buffer was successfully cleared</returns>
        public bool DiscardReceiveBuffer()
        {
            try
            {
                _serial.DiscardInBuffer();
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Clears the buffer of data to send
        /// </summary>
        /// <returns>if the buffer was successfully cleared</returns>
        public bool DiscardSendBuffer()
        {
            try
            {
                _serial.DiscardOutBuffer();
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Converts an integer to a baud rate
        /// </summary>
        /// <param name="baud">The integer to be converted</param>
        /// <returns>The integer as a BaudRateEnum</returns>
        public static BaudRateEnum IntToBaud(int baud)
        {
            return (BaudRateEnum)baud;
        }

        /// <summary>
        /// Converts a baud rate to an integer
        /// </summary>
        /// <param name="baud">The baud rate to be converted</param>
        /// <returns>The baud rate as an integer</returns>
        public static int BaudToInt(BaudRateEnum baud)
        {
            return (int)baud;
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is called when the number of bytes in the receive buffer reaches the threshold set
        /// </summary>
        public event DataReceiveEventHandler Received;

        /// <summary>
        /// Executes the Received event
        /// </summary>
        /// <param name="e">Arguments associated with the event</param>
        protected virtual void OnReceived(DataRecievedEventArgs e)
        {
            if (Received != null)
            {
                Received(this, e);
            }
        }

        /// <summary>
        /// The SerialPort class event for received bytes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void receive(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // Collecting the characters received to our 'buffer' (string).
            MessageBuffer = ((SerialPort)sender).ReadExisting();
            ReceivedMessage = MessageBuffer;
            newMessage = true;
            if (!internalCall)
                OnReceived(new DataRecievedEventArgs(ReceivedMessage, DateTime.Now));
        }
        #endregion

        #region Async

        /// <summary>
        /// Asynchronously receives data from the serial port, until the pre-set threshold is met
        /// </summary>
        /// <returns>If data was successfully received</returns>
        public Task<bool> ReceiveAsync()
        {
            return new Task<bool>(() => this.receiveAsyncRunner());
        }

        /// <summary>
        /// Asynchronously receives data from the serial port, until the specified threshold is met
        /// </summary>
        /// <param name="threshold">The number of bytes to receive before returning</param>
        /// <returns>If data was successfully received</returns>
        public Task<bool> ReceiveAsync(int threshold)
        {
            return new Task<bool>(() => this.receiveAsyncNumberRunner(threshold));
        }

        /// <summary>
        /// Asynchronously receives data from the serial port, until the specified string has been received
        /// </summary>
        /// <param name="delimiter">The string to receive till</param>
        /// <returns>If data was successfully received</returns>
        public Task<bool> ReceiveToAsync(string delimiter)
        {
            if (delimiter != "" && delimiter != null)
            {
                return new Task<bool>(() => this.receiveToAsyncRunner(delimiter));
            }
            else
                return new Task<bool>(() => false);
        }

        /// <summary>
        /// Receives characters until the system's new line characters has been reached.
        /// "\r\n" for non-Unix and "\n" for Unix
        /// </summary>
        /// <returns>If the new data up upto the new line character</returns>
        public Task<bool> ReceiveNewLineAsync()
        {
            return new Task<bool>(() => receiveNewLineAsyncRunner());
        }

        /// <summary>
        /// Asynchronously receives one byte from the serial port
        /// </summary>
        /// <returns>The byte that was received</returns>
        public Task<int> ReceiveByteAsync()
        {
            return new Task<int>(() => _serial.ReadByte());
        }

        /// <summary>
        /// Asynchronously receives one ASCII character from the serial port
        /// </summary>
        /// <returns>The char that was received</returns>
        public Task<char> ReceiveCharAsync()
        {
            return new Task<char>(() => (char)_serial.ReadChar());
        }

        /// <summary>
        /// Asynchronously sends data through the serial port
        /// </summary>
        /// <param name="data">The data to send to the serial port</param>
        /// <returns>The asynchronous task</returns>
        public Task SendDataAsync(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                Action<object> action = (object dat) =>
                {
                    this.sendDataRunner(dat);
                };

                Task task = Task.Factory.StartNew(action, data);
                return task;
            }
            else
                return null;
        }

        /// <summary>
        /// Asynchronously sends data through the serial port
        /// </summary>
        /// <param name="data">The data to send to the serial port</param>
        /// <returns>The asynchronous task</returns>
        public Task SendDataAsync(int[] data)
        {
            if (data != null)
            {
                byte[] array = new byte[data.Length];
                int i = 0;
                foreach (int val in data)
                {
                    array[i++] = Convert.ToByte(val % 256);
                }
                return SendDataAsync(array);
            }
            else
                return null;
        }

        /// <summary>
        /// Asynchronously sends data through the serial port
        /// </summary>
        /// <param name="data">The data to send to the serial port</param>
        /// <returns>The asynchronous task</returns>
        public Task SendDataAsync(string data)
        {
            if (data != null && data != "")
            {
                byte[] hexstring = Encoding.ASCII.GetBytes(data);
                return SendDataAsync(hexstring);
            }
            else
                return null;
        }

        /// <summary>
        /// Asynchronously sends data through the serial port
        /// </summary>
        /// <param name="data">The data to send to the serial port</param>
        /// <returns>The asynchronous task</returns>
        public Task SendDataAsync(char[] data)
        {

            if (data != null && data.Length > 0)
            {
                return SendDataAsync(Encoding.ASCII.GetBytes(data));
            }
            else
                return null;
        }

        #endregion

        #region Not Async

        /// <summary>
        /// Receives data from the serial port, until the pre-set threshold is met
        /// </summary>
        /// <returns>If data was successfully received</returns>
        public bool Receive()
        {
            return receiveAsyncRunner();
        }

        /// <summary>
        /// Receives data from the serial port, until the specified threshold is met
        /// </summary>
        /// <param name="threshold">The number of bytes to receive before returning</param>
        /// <returns>If data was successfully received</returns>
        public bool Receive(int threshold)
        {
            return receiveAsyncNumberRunner(threshold);
        }

        /// <summary>
        /// Receives data from the serial port, until the specified string has been received
        /// </summary>
        /// <param name="delimiter">The string to receive till</param>
        /// <returns>If data was successfully received</returns>
        public bool ReceiveTo(string delimiter)
        {
            if (delimiter != "" && delimiter != null)
            {
                return receiveToAsyncRunner(delimiter);
            }
            else
                return false;
        }

        /// <summary>
        /// Receives one byte from the serial port
        /// </summary>
        /// <returns>The byte that was received</returns>
        public int ReceiveByte()
        {
            if(this.IsOpen)
                return _serial.ReadByte();
            return -1;
        }

        /// <summary>
        /// Receives one ASCII character from the serial port
        /// </summary>
        /// <returns>The char that was received</returns>
        public char ReceiveChar()
        {
            if(this.IsOpen)
                return (char)_serial.ReadChar();
            return '\0';
        }

        /// <summary>
        /// Receives characters until the system's new line characters has been reached.
        /// "\r\n" for non-Unix and "\n" for Unix
        /// </summary>
        /// <returns>If the new data up upto the new line character</returns>
        public bool ReceiveNewLine()
        {
            return receiveNewLineAsyncRunner();
        }

        /// <summary>
        /// Sends data through the serial port
        /// </summary>
        /// <param name="data">The data to send to the serial port</param>
        public void SendData(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                this.sendDataRunner(data);
            }
        }

        /// <summary>
        /// Sends data through the serial port
        /// </summary>
        /// <param name="data">The data to send to the serial port</param>
        public void SendData(byte data)
        {
            byte[] dat = { data };
            this.sendDataRunner(dat);
        }

        /// <summary>
        /// Sends data through the serial port
        /// </summary>
        /// <param name="data">The data to send to the serial port</param>
        public void SendData(int[] data)
        {
            if (data != null)
            {
                byte[] array = new byte[data.Length];
                int i = 0;
                foreach (int val in data)
                {
                    array[i++] = Convert.ToByte(val % 256);
                }
                SendData(array);
            }
        }

        /// <summary>
        /// Sends data through the serial port
        /// </summary>
        /// <param name="data">The data to send to the serial port</param>
        public void SendData(string data)
        {
            if (data != null && data != "")
            {
                byte[] hexstring = Encoding.ASCII.GetBytes(data);
                SendData(hexstring);
            }
        }

        /// <summary>
        /// Sends data through the serial port
        /// </summary>
        /// <param name="data">The data to send to the serial port</param>
        public void SendData(char[] data)
        {
            if (data != null && data.Length > 0)
            {
                SendData(Encoding.ASCII.GetBytes(data));
            }
        }

        /// <summary>
        /// Sends data through the serial port
        /// </summary>
        /// <param name="data">The data to send to the serial port</param>
        public void SendData(char data)
        {
            char[] dat = { data };
            SendData(dat);
        }

        #endregion

        #region Data Runners

        /// <summary>
        /// Does the work that the task sets forth, for a pre-set threshold
        /// </summary>
        /// <returns>if data was successfully received</returns>
        private bool receiveAsyncRunner()
        {
            if (this.IsOpen)
            {
                try
                {
                    bool eventsPrevEnabled = EnableEvents;
                    EnableEvents = true;
                    newMessage = false;
                    internalCall = true;

                    while (newMessage) ;

                    internalCall = false;
                    if (eventsPrevEnabled)
                        EnableEvents = true;

                    if (ReceivedMessage != null && ReceivedMessage != "")
                        return true;
                    else
                        return false;
                }
                catch (System.Exception ex)
                {
                    return false;
                }
            }
            else
                return false;

        }

        /// <summary>
        /// Does the work that the task sets forth, for a specified threshold
        /// </summary>
        /// <param name="threshold">The number of bytes to receive before returning</param>
        /// <returns>if data was successfully received</returns>
        private bool receiveAsyncNumberRunner(int threshold)
        {
            int previousThreshold = this.ReceivedBytesThreshold;
            this.ReceivedBytesThreshold = threshold;

            bool status = receiveAsyncRunner();

            this.ReceivedBytesThreshold = previousThreshold;
            return status;
        }

        /// <summary>
        /// Does the work that the task sets forth, until the delimiter is received
        /// </summary>
        /// <param name="delimiter">The string to receive till</param>
        /// <returns>if data was successfully received</returns>
        private bool receiveToAsyncRunner(string delimiter)
        {
            if (this.IsOpen)
            {
                bool eventsPrevEnabled = EnableEvents;
                EnableEvents = false;
                try
                {
                    MessageBuffer = _serial.ReadTo(delimiter);
                }
                catch (System.Exception ex)
                {
                    return false;
                }


                if (messageBuffer != "" && messageBuffer != null)
                {
                    ReceivedMessage = MessageBuffer;
                    if (eventsPrevEnabled)
                        EnableEvents = true;
                    return true;
                }
                else
                {
                    if (eventsPrevEnabled)
                        EnableEvents = true;
                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// The task for running a read up to a new line character
        /// </summary>
        /// <returns>If the new line was received</returns>
        private bool receiveNewLineAsyncRunner()
        {
            if (this.IsOpen)
            {
                try
                {
                    bool eventsPrevEnabled = EnableEvents;
                    EnableEvents = false;

                    _serial.NewLine = Environment.NewLine;
                    MessageBuffer = _serial.ReadLine();
                    ReceivedMessage = MessageBuffer;

                    newMessage = true;
                    internalCall = false;
                    if (eventsPrevEnabled)
                        EnableEvents = true;

                    if (ReceivedMessage != null && ReceivedMessage != "")
                        return true;
                    else
                        return false;
                }
                catch (System.Exception ex)
                {
                    return false;
                }
            }
            else
                return false;

        }

        /// <summary>
        /// Does the work that the task sets forth
        /// </summary>
        /// <param name="data">The data to send to the serial port</param>
        /// <returns>if the data was successfully sent</returns>
        private bool sendDataRunner(object dat)
        {
            byte[] data = dat as byte[];
            if (this.IsOpen && data.Length > 0)
            {
                try
                {
                    //Thread.Sleep(10); // wait 1ms to make sure previous message has been processed by receiver
                    foreach (byte hexval in data)
                    {
                        byte[] _hexval = new byte[] { hexval }; // need to convert byte 
                        // to byte[] to write
                        _serial.Write(_hexval, 0, 1);
                        //Thread.Sleep(5);
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Does the work that the task sets forth
        /// </summary>
        /// <param name="data">The string to send to the serial port</param>
        /// <returns>if the data was successfully sent</returns>
        public bool sendStringRunner(string data)
        {
            if (data != null && data != "")
            {
                try
                {
                    _serial.Write(data);
                    return true;
                }
                catch (System.Exception ex)
                {
                    return false;
                }
            }
            else
                return false;
        }

        #endregion

    }

}
