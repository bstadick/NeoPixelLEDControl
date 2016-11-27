using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.IO.Ports;

namespace NeopixelSDK
{
    // A delegate type for hooking up saved available notifications.
    public delegate void SavedAvailableEventHandler(object sender, EventArgs e);

    /// <summary>
    /// The different display modes of the Neopixels
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// Pixels are set to a single solid color
        /// </summary>
        Solid = 1,
        /// <summary>
        /// Pixels display several different patterns
        /// </summary>
        Flair,
        /// <summary>
        /// Pixels display a festive Christmas theme
        /// </summary>
        Christmas,
        /// <summary>
        /// Pixels display a rainbow sweep
        /// </summary>
        Rainbow,
        /// <summary>
        /// Pixels display a rainbow sweep on monitors as well
        /// </summary>
        AllRainbow
    }

    public class Neopixel
    {

        /// <summary>
        /// The serial port to communicate over.
        /// </summary>
        private SerialPort _port;

        /// <summary>
        /// The message buffer for incoming data.
        /// </summary>
        List<byte> buffer = new List<byte>();

        /// <summary>
        /// Event triggered when the saved parameters have been read from the microcontroller.
        /// </summary>
        public event SavedAvailableEventHandler SavedAvailable;

        #region Properties

        private Color _monitor1 = Color.FromArgb(255, 255, 255);
        /// <summary>
        /// Gets or sets the color of monitor one.
        /// </summary>
        public Color MonitorOne
        {
            get { return _monitor1; }
            set
            {
                _monitor1 = value;
                sendMessage();
            }
        }

        private Color _monitor2 = Color.FromArgb(255, 255, 255);
        /// <summary>
        /// Gets or sets the color of monitor two.
        /// </summary>
        public Color MonitorTwo
        {
            get { return _monitor2; }
            set
            {
                _monitor2 = value;
                sendMessage();
            }
        }

        private Color _monitor3 = Color.FromArgb(255, 255, 255);
        /// <summary>
        /// Gets or sets the color of monitor three.
        /// </summary>
        public Color MonitorThree
        {
            get { return _monitor3; }
            set
            {
                _monitor3 = value;
                sendMessage();
            }
        }

        private Color _monitor4 = Color.FromArgb(255, 255, 255);
        /// <summary>
        /// Gets or sets the color of monitor four.
        /// </summary>
        public Color MonitorFour
        {
            get { return _monitor4; }
            set
            {
                _monitor4 = value;
                sendMessage();
            }
        }

        private byte _monitor1Count = 15;
        /// <summary>
        /// Gets or sets the number of LED pixels for monitor one.
        /// </summary>
        public byte MonitorOneCount
        {
            get { return _monitor1Count; }
            set
            {
                _monitor1Count = value;
                sendMessage();
            }
        }

        private byte _monitor2Count = 15;
        /// <summary>
        /// Gets or sets the number of LED pixels for monitor two.
        /// </summary>
        public byte MonitorTwoCount
        {
            get { return _monitor2Count; }
            set
            {
                _monitor2Count = value;
                sendMessage();
            }
        }

        private byte _monitor3Count = 15;
        /// <summary>
        /// Gets or sets the number of LED pixels for monitor three.
        /// </summary>
        public byte MonitorThreeCount
        {
            get { return _monitor3Count; }
            set
            {
                _monitor3Count = value;
                sendMessage();
            }
        }

        private byte _monitor4Count = 15;
        /// <summary>
        /// Gets or sets the number of LED pixels for monitor four.
        /// </summary>
        public byte MonitorFourCount
        {
            get { return _monitor4Count; }
            set
            {
                _monitor4Count = value;
                sendMessage();
            }
        }

        private byte _mode = (byte)DisplayMode.Solid;
        /// <summary>
        /// Gets or sets if chassis LED flair should be used.
        /// </summary>
        public DisplayMode Mode
        {
            get { return (DisplayMode)_mode; }
            set
            {
                _mode = (byte)value;
                sendMessage();
            }
        }

        private Color _chassis = Color.FromArgb(255, 255, 255);
        /// <summary>
        /// Gets or sets the solid color state of the chassis LEDs. Requires flair to be off.
        /// </summary>
        public Color Chassis
        {
            get { return _chassis; }
            set
            {
                _chassis = value;
                sendMessage();
            }
        }

        private byte _chassisCount = 30;
        /// <summary>
        /// Gets or sets the number of LED pixels for monitor four.
        /// </summary>
        public byte ChassisCount
        {
            get { return _chassisCount; }
            set
            {
                _chassisCount = value;
                sendMessage();
            }
        }

        #endregion

        /// <summary>
        /// Creates a new Neopixel communication interface.
        /// </summary>
        /// <param name="port">The port to use to communicate to the controlling microcontroller</param>
        public Neopixel(string port)
        {
            SetPort(port);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Neopixel()
        {
            if (_port != null && _port.IsOpen)
                _port.Close();
        }

        /// <summary>
        /// Event handler for incoming data being received over the serial port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (_port.BytesToRead > 0)
            {
                buffer.Add((byte)_port.ReadByte());
                if (buffer.Count >= 18)
                    applyCurrentValues();
            }
        }

        /// <summary>
        /// List of available serial ports.
        /// </summary>
        private static List<string> ports = new List<string>();

        /// <summary>
        /// Lists the available serial ports.
        /// </summary>
        /// <returns></returns>
        public static List<string> AvailablePorts()
        {
            ports.RemoveRange(0, ports.Count);
            foreach (string s in SerialPort.GetPortNames())
            {
                ports.Add(s);
            }
            return ports;
        }

        /// <summary>
        /// Sets the serial port to communicate to the microcontroller
        /// </summary>
        /// <param name="port"></param>
        public void SetPort(string port)
        {
            if(_port != null && _port.IsOpen)
                _port.Close();

            _port = null;

            _port = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
            _port.ReceivedBytesThreshold = 1;
            _port.DataReceived += _port_DataReceived;
            try
            {
                _port.Open();
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Gets if the serial port is open.
        /// </summary>
        /// <returns></returns>
        public bool GetPortOpen()
        {
            if (_port != null)
                return _port.IsOpen;
            else
                return false;
        }

        /// <summary>
        /// Gets the current values stored on the microcontroller
        /// </summary>
        public void GetCurrentValues()
        {
           
            byte[] msg = new byte[2];
            msg[0] = 37;
            msg[1] = 37;

            if (_port != null && _port.IsOpen)
               _port.Write(msg, 0, 2);

        }

        /// <summary>
        /// Applies the current values stored on the microcontroller to the user interface.
        /// </summary>
        private void applyCurrentValues()
        {
            byte r, g, b;
            List<byte> data = new List<byte>(buffer);
            buffer.Clear();

            if (data == null || data.Count < 23)
                return;

            while (data.Count > 0 && data[0] != '$') // find start of message
                data.RemoveAt(0);

            if (data.Count < 22) // not enough data was returned
                return;

            data.RemoveAt(0); // byte indicating length

            r = data[0];
            g = data[1];
            b = data[2];
            _monitor1 = Color.FromArgb(r, g, b);
            _monitor1Count = data[3];

            r = data[4];
            g = data[5];
            b = data[6];
            _monitor2 = Color.FromArgb(r, g, b);
            _monitor2Count = data[7];

            r = data[8];
            g = data[9];
            b = data[10];
            _monitor3 = Color.FromArgb(r, g, b);
            _monitor3Count = data[11];

            r = data[12];
            g = data[13];
            b = data[14];
            _monitor4 = Color.FromArgb(r, g, b);
            _monitor4Count = data[15];

            r = data[16];
            g = data[17];
            b = data[18];
            _chassis = Color.FromArgb(r, g, b);
            _chassisCount = data[19];
            _mode = data[20];

            OnSavedAvailable(EventArgs.Empty);

        }

        /// <summary>
        /// Event invocation of the saved parameters are all read from the microcontroller.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSavedAvailable(EventArgs e)
        {
            if (SavedAvailable != null)
                SavedAvailable(this, e);
        }

        /// <summary>
        /// Sets the monitor LED pixel counts.
        /// </summary>
        /// <param name="monitorOne">The number of pixels for monitor one.</param>
        /// <param name="monitorTwo">The number of pixels for monitor two.</param>
        /// <param name="monitorThree">The number of pixels for monitor three.</param>
        /// <param name="monitorFour">The number of pixels for monitor four.</param>
        public void SetMonitorPixels(byte monitorOne, byte monitorTwo, byte monitorThree, byte monitorFour)
        {
            _monitor1Count = monitorOne;
            _monitor2Count = monitorTwo;
            _monitor3Count = monitorThree;
            _monitor4Count = monitorFour;
            sendMessage();
        }

        /// <summary>
        /// Sets the monitor LED colors.
        /// </summary>
        /// <param name="monitorOne">The color for monitor one.</param>
        /// <param name="monitorTwo">The color for monitor two.</param>
        /// <param name="monitorThree">The color for monitor three.</param>
        /// <param name="monitorFour">The color for monitor four.</param>
        public void SetMonitorColors(Color monitorOne, Color monitorTwo, Color monitorThree, Color monitorFour)
        {
            _monitor1 = monitorOne;
            _monitor2 = monitorTwo;
            _monitor3 = monitorThree;
            _monitor4 = monitorFour;
            sendMessage();
        }

        /// <summary>
        /// Sets the chassis LEDs to be a solid color.
        /// </summary>
        /// <param name="chassis">The color for the chassis.</param>
        public void SetChassisColors(Color chassis, byte chassisCount)
        {
            _chassis = chassis;
            _chassisCount = chassisCount;
            _mode = (byte)DisplayMode.Solid;
            sendMessage();
        }

        /// <summary>
        /// Sends the message to the microcontroller.
        /// </summary>
        private void sendMessage()
        {
            List<byte> msg = new List<byte>(14);

            // populate the message
            msg.Add((byte)'$');
            msg.Add(_monitor1.R);
            msg.Add(_monitor1.G);
            msg.Add(_monitor1.B);
            msg.Add(_monitor1Count);
            msg.Add(_monitor2.R);
            msg.Add(_monitor2.G);
            msg.Add(_monitor2.B);
            msg.Add((byte)(_monitor1Count + _monitor2Count));
            msg.Add(_monitor3.R);
            msg.Add(_monitor3.G);
            msg.Add(_monitor3.B);
            msg.Add((byte)(_monitor2Count + _monitor3Count));
            msg.Add(_monitor4.R);
            msg.Add(_monitor4.G);
            msg.Add(_monitor4.B);
            msg.Add((byte)(_monitor3Count + _monitor4Count));
            msg.Add(_chassis.R);
            msg.Add(_chassis.G);
            msg.Add(_chassis.B);
            msg.Add(_chassisCount);
            msg.Add(_mode);
            msg.Insert(1, (byte)(msg.Count - 1)); // insert number of bytes in message after leading character

            if (_port != null && _port.IsOpen)
                _port.Write(msg.ToArray(), 0, msg.Count); // write message to serial port
        }

    }
}
