using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NeopixelSDK
{
    public class Neopixel
    {
        
        private Color _monitor1 = Color.FromArgb(255, 255, 255);
        public Color MonitorOne
        {
            get { return _monitor1; }
            set
            {
                _monitor1 = value;
                setMonitorColors();
            }
        }

        private Color _monitor2 = Color.FromArgb(255, 255, 255);
        public Color MonitorTwo
        {
            get { return _monitor2; }
            set
            {
                _monitor2 = value;
                setMonitorColors();
            }
        }

        private Serial _port;

        public Neopixel()
        {

        }

        public Neopixel(string port)
        {
            _port = new Serial(port);
            _port.Open();
        }

        ~Neopixel()
        {
            if (_port != null && _port.IsOpen)
                _port.Close();
        }

        public List<string> AvailablePorts()
        {
            return _port.AvailablePorts;
        }

        public void SetPort(string port)
        {
            if(_port != null && _port.IsOpen)
                _port.Close();

            _port = new Serial(port);
            _port.Open();
        }

        public bool PortOpen()
        {
            if (_port != null)
                return _port.IsOpen;
            else
                return false;
        }

        private void setMonitorColors()
        {
            byte[] msg = new byte[8];

            msg[0] = (byte)'$';
            msg[1] = _monitor1.R;
            msg[2] = _monitor1.G;
            msg[3] = _monitor1.B;
            msg[4] = _monitor2.R;
            msg[5] = _monitor2.G;
            msg[6] = _monitor2.B;
            msg[7] = 3;

            if (_port != null && _port.IsOpen)
                _port.SendData(msg);
        }

        public void SetMonitorColors(Color monitorOne, Color monitorTwo)
        {
            MonitorOne = monitorOne;
            MonitorTwo = monitorTwo;
        }

        public void SetMonitorColorsSim(Color monitorOne, Color monitorTwo)
        {
            _monitor1 = monitorOne;
            _monitor2 = monitorTwo;
            setMonitorColors();
        }

    }
}
