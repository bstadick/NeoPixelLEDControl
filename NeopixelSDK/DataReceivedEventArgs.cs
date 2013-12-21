using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeopixelSDK
{
    /// <summary>
    /// Event arguments associated with the Serial Class
    /// </summary>
    public class DataRecievedEventArgs : EventArgs
    {
        private string message = "";
        private int length = 0;
        private DateTime timestamp = new DateTime();

        public DataRecievedEventArgs()
        {

        }

        /// <summary>
        /// Event arguments associated with the Serial class
        /// </summary>
        /// <param name="msg">The message received on the serial port</param>
        public DataRecievedEventArgs(string msg)
        {
            message = msg;
            length = msg.Length;
            timestamp = DateTime.Now;
        }

        /// <summary>
        /// Event arguments associated with the Serial class
        /// </summary>
        /// <param name="msg">The message received on the serial port</param>
        /// <param name="time">The time the message was received</param>
        public DataRecievedEventArgs(string msg, DateTime time)
        {
            message = msg;
            length = msg.Length;
            timestamp = time;
        }

        /// <summary>
        /// Gets the latest message received by the serial port
        /// </summary>
        public string Message
        {
            get
            {
                return message;
            }
        }

        /// <summary>
        /// Gets the length of the latest message received by the serial port
        /// </summary>
        public int Length
        {
            get
            {
                return length;
            }
        }

        /// <summary>
        /// Gets the time the latest message was received
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                return this.timestamp;
            }
        }
    }
}
