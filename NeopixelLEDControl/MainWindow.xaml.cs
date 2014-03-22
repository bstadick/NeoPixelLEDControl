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
using NeopixelSDK;
using System.Drawing;
using System.Timers;

namespace NeopixelLEDControl
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void myDelegate();

        private Neopixel _pixel;
        private Timer timer = new Timer(3000); // timer for handling temporary message displaying
        private myDelegate timerFinish; // delegate to handle hiding temproary message

        // monitor backlight parameters
        private System.Drawing.Color _monitor1 = System.Drawing.Color.FromArgb(255, 255, 255);
        private System.Drawing.Color _monitor2 = System.Drawing.Color.FromArgb(255, 255, 255);
        private System.Drawing.Color _monitor3 = System.Drawing.Color.FromArgb(255, 255, 255);
        private System.Drawing.Color _monitor4 = System.Drawing.Color.FromArgb(255, 255, 255);
        private System.Drawing.Color _chassis = System.Drawing.Color.FromArgb(255, 255, 255);
        private byte _numPixelsM1 = 15;
        private byte _numPixelsM2 = 15;
        private byte _numPixelsM3 = 15;
        private byte _numPixelsM4 = 15;
        private byte _numPixelsC = 30;
        private bool _flair = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            timer.Elapsed += timerElapsed;

            timerFinish = new myDelegate(setTextBlockHidden);
        }

        /// <summary>
        /// Sets some basic parameters of the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboPortList.ItemsSource = Neopixel.AvailablePorts();
            RadioButtonClick(radio2, null);
        }

        /// <summary>
        /// Event to handle the user changing a value in a text box. Updates the pixel values and creates a preview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            byte r, g, b;
            if (sender == null)
                return;

            TextBox box = sender as TextBox;
            try
            {
                switch (box.Name)
                {
                    case "textBoxM1R":
                    case "textBoxM1G":
                    case "textBoxM1B":
                    case "textBoxM1P":
                        r = Convert.ToByte(textBoxM1R.Text);
                        g = Convert.ToByte(textBoxM1G.Text);
                        b = Convert.ToByte(textBoxM1B.Text);
                        _monitor1 = System.Drawing.Color.FromArgb(r, g, b);
                        rectM1.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
                        _numPixelsM1 = Convert.ToByte(textBoxM1P.Text);
                        break;
                    case "textBoxM2R":
                    case "textBoxM2G":
                    case "textBoxM2B":
                    case "textBoxM2P":
                        r = Convert.ToByte(textBoxM2R.Text);
                        g = Convert.ToByte(textBoxM2G.Text);
                        b = Convert.ToByte(textBoxM2B.Text);
                        _monitor2 = System.Drawing.Color.FromArgb(r, g, b);
                        rectM2.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
                        _numPixelsM2 = Convert.ToByte(textBoxM2P.Text);
                        break;
                    case "textBoxM3R":
                    case "textBoxM3G":
                    case "textBoxM3B":
                    case "textBoxM3P":
                        r = Convert.ToByte(textBoxM3R.Text);
                        g = Convert.ToByte(textBoxM3G.Text);
                        b = Convert.ToByte(textBoxM3B.Text);
                        _monitor3 = System.Drawing.Color.FromArgb(r, g, b);
                        rectM3.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
                        _numPixelsM3 = Convert.ToByte(textBoxM3P.Text);
                        break;
                    case "textBoxM4R":
                    case "textBoxM4G":
                    case "textBoxM4B":
                    case "textBoxM4P":
                        r = Convert.ToByte(textBoxM4R.Text);
                        g = Convert.ToByte(textBoxM4G.Text);
                        b = Convert.ToByte(textBoxM4B.Text);
                        _monitor4 = System.Drawing.Color.FromArgb(r, g, b);
                        rectM4.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
                        _numPixelsM4 = Convert.ToByte(textBoxM4P.Text);
                        break;
                    case "textBoxCR":
                    case "textBoxCG":
                    case "textBoxCB":
                    case "textBoxCP":
                        r = Convert.ToByte(textBoxCR.Text);
                        g = Convert.ToByte(textBoxCG.Text);
                        b = Convert.ToByte(textBoxCB.Text);
                        _chassis = System.Drawing.Color.FromArgb(r, g, b);
                        rectC.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
                        _numPixelsC = Convert.ToByte(textBoxCP.Text);
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            
        }

        /// <summary>
        /// Sets the port to use. Gets the current values from the microcontroller and displays the settings on the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetPortClick(object sender, RoutedEventArgs e)
        {
            string port = comboPortList.SelectedItem as string;
            _pixel = new Neopixel(port);
            _pixel.SavedAvailable += pixelSavedAvailable;

            // get current settings
            try
            {
                _pixel.GetCurrentValues(); // TODO - can't receive data for some reason
            }
            catch (Exception ex)
            {
                setUserMessage("Failed to receive previous settings.", System.Drawing.Color.Red);
            }
        }

        /// <summary>
        /// Event handler for when the saved values on the microcontroller have been made available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pixelSavedAvailable(object sender, EventArgs e)
        {
            // apply current settings to the UI
            textBoxM1R.Text = _pixel.MonitorOne.R.ToString();
            textBoxM1G.Text = _pixel.MonitorOne.G.ToString();
            textBoxM1B.Text = _pixel.MonitorOne.B.ToString();
            rectM1.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(_pixel.MonitorOne.R, _pixel.MonitorOne.G, _pixel.MonitorOne.B));
            textBoxM1P.Text = _pixel.MonitorOneCount.ToString();

            textBoxM2R.Text = _pixel.MonitorTwo.R.ToString();
            textBoxM2G.Text = _pixel.MonitorTwo.G.ToString();
            textBoxM2B.Text = _pixel.MonitorTwo.B.ToString();
            rectM2.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(_pixel.MonitorTwo.R, _pixel.MonitorTwo.G, _pixel.MonitorTwo.B));
            textBoxM2P.Text = _pixel.MonitorTwoCount.ToString();

            textBoxM3R.Text = _pixel.MonitorThree.R.ToString();
            textBoxM3G.Text = _pixel.MonitorThree.G.ToString();
            textBoxM3B.Text = _pixel.MonitorThree.B.ToString();
            rectM3.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(_pixel.MonitorThree.R, _pixel.MonitorThree.G, _pixel.MonitorThree.B));
            textBoxM3P.Text = _pixel.MonitorThreeCount.ToString();

            textBoxM4R.Text = _pixel.MonitorFour.R.ToString();
            textBoxM4G.Text = _pixel.MonitorFour.G.ToString();
            textBoxM4B.Text = _pixel.MonitorFour.B.ToString();
            rectM4.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(_pixel.MonitorFour.R, _pixel.MonitorFour.G, _pixel.MonitorFour.B));
            textBoxM4P.Text = _pixel.MonitorFourCount.ToString();

            textBoxCR.Text = _pixel.Chassis.R.ToString();
            textBoxCG.Text = _pixel.Chassis.G.ToString();
            textBoxCB.Text = _pixel.Chassis.B.ToString();
            rectC.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(_pixel.Chassis.R, _pixel.Chassis.G, _pixel.Chassis.B));
            textBoxCP.Text = _pixel.ChassisCount.ToString();

            _flair = _pixel.Flair;
            buttonFlair.Content = (_flair ? "On" : "Off");

            if (_pixel.MonitorFourCount != 0)
            {
                RadioButtonClick(radio4, null);
            }
            else if (_pixel.MonitorThreeCount != 0)
            {
                RadioButtonClick(radio3, null);
            }
            else if (_pixel.MonitorTwoCount != 0)
            {
                RadioButtonClick(radio2, null);
            }
            else
            {
                RadioButtonClick(radio1, null);
            }
        }

        /// <summary>
        /// Refreshes the available serial port list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRefreshPortClick(object sender, RoutedEventArgs e)
        {
            comboPortList.ItemsSource = Neopixel.AvailablePorts();
        }

        /// <summary>
        /// Applies the current settings and sends the changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonApplyClick(object sender, RoutedEventArgs e)
        {
            if (_pixel != null && _pixel.GetPortOpen())
            {
                _pixel.SetMonitorPixels(_numPixelsM1, _numPixelsM2, _numPixelsM3, _numPixelsM4);
                _pixel.SetMonitorColors(_monitor1, _monitor2, _monitor3, _monitor4);
                if (!_flair)
                    _pixel.SetChassisColors(_chassis, _numPixelsC);
                else
                    _pixel.Flair = true;
                textBlockFailMsg.Visibility = Visibility.Hidden;
            }
            else
            {
                setUserMessage("Failed to send, no connection.", System.Drawing.Color.Red);                
            }
        }

        /// <summary>
        /// Event handler for timer elapsed event. Used to hide the temporary message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerElapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(timerFinish, null);
            timer.Stop();
        }

        /// <summary>
        /// Called when timer finishes to hide the temporary message.
        /// </summary>
        void setTextBlockHidden()
        {
            textBlockFailMsg.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Sets the temporary message in the bottom right hand corner of the UI.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        void setUserMessage(string msg, System.Drawing.Color color)
        {
            textBlockFailMsg.Text = msg;
            textBlockFailMsg.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            textBlockFailMsg.Visibility = Visibility.Visible;
            timer.Start();
        }

        /// <summary>
        /// Changes the number of monitors displayed on the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonClick(object sender, RoutedEventArgs e)
        {
            RadioButton button = sender as RadioButton;

            if (button.IsChecked == null || !(bool)button.IsChecked) // button is unchecked, only handle checked button
                return;

            switch(button.Name)
            {
                case "radio1":
                    textBoxM2P.Text = "0";
                    textBoxM3P.Text = "0";
                    textBoxM4P.Text = "0";
                    stackM2.Visibility = System.Windows.Visibility.Collapsed;
                    stackM3.Visibility = System.Windows.Visibility.Collapsed;
                    stackM4.Visibility = System.Windows.Visibility.Collapsed;

                    stackM1.SetValue(Grid.ColumnSpanProperty, 4);

                    break;
                case "radio2":
                    textBoxM2P.Text = "15";
                    stackM2.Visibility = System.Windows.Visibility.Visible;

                    textBoxM3P.Text = "0";
                    textBoxM4P.Text = "0";
                    stackM3.Visibility = System.Windows.Visibility.Collapsed;
                    stackM4.Visibility = System.Windows.Visibility.Collapsed;

                    stackM1.SetValue(Grid.ColumnSpanProperty, 2);
                    stackM2.SetValue(Grid.ColumnProperty, 4);
                    stackM2.SetValue(Grid.ColumnSpanProperty, 2);

                    break;
                case "radio3":
                    textBoxM2P.Text = "15";
                    textBoxM3P.Text = "15";
                    stackM2.Visibility = System.Windows.Visibility.Visible;
                    stackM3.Visibility = System.Windows.Visibility.Visible;

                    textBoxM4P.Text = "0";
                    stackM4.Visibility = System.Windows.Visibility.Collapsed;

                    stackM1.SetValue(Grid.ColumnSpanProperty, 1);
                    stackM2.SetValue(Grid.ColumnProperty, 3);
                    stackM2.SetValue(Grid.ColumnSpanProperty, 2);
                    stackM3.SetValue(Grid.ColumnProperty, 5);

                    break;
                case "radio4":
                    textBoxM2P.Text = "15";
                    textBoxM3P.Text = "15";
                    textBoxM4P.Text = "15";
                    stackM2.Visibility = System.Windows.Visibility.Visible;
                    stackM3.Visibility = System.Windows.Visibility.Visible;
                    stackM4.Visibility = System.Windows.Visibility.Visible;

                    stackM1.SetValue(Grid.ColumnSpanProperty, 1);
                    stackM2.SetValue(Grid.ColumnProperty, 3);
                    stackM2.SetValue(Grid.ColumnSpanProperty, 1);
                    stackM3.SetValue(Grid.ColumnProperty, 4);

                    break;
            }
            this.UpdateLayout();
        }

        /// <summary>
        /// Toggles case (chassis) LED flair on and off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFlairClick(object sender, RoutedEventArgs e)
        {
            _flair = !_flair;
            buttonFlair.Content = (_flair ? "On" : "Off");
        }
    }
}
