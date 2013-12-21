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

        private Serial _port = new Serial();
        private Neopixel _pixel;
        private Timer timer = new Timer(3000);
        private myDelegate del;

        private System.Drawing.Color _monitor1 = System.Drawing.Color.FromArgb(255, 255, 255);
        private System.Drawing.Color _monitor2 = System.Drawing.Color.FromArgb(255, 255, 255);
        private int numPixelsM1 = 15;
        private int numPixelsM2 = 15;
        private bool windowLoaded = false;

        public MainWindow()
        {
            InitializeComponent();

            timer.Elapsed += timer_Elapsed;

            del = new myDelegate(setTextBlockHidden);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboPortList.ItemsSource = _port.AvailablePorts;
            windowLoaded = true;
        }

        private void textBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            byte r, g, b;
            TextBox box = sender as TextBox;
            if (windowLoaded)
            {
                try
                {
                    switch (box.Name)
                    {
                        case "textBoxM1R":
                        case "textBoxM1G":
                        case "textBoxM1B":
                            r = Convert.ToByte(textBoxM1R.Text);
                            g = Convert.ToByte(textBoxM1G.Text);
                            b = Convert.ToByte(textBoxM1B.Text);
                            _monitor1 = System.Drawing.Color.FromArgb(r, g, b);
                            rectM1.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
                            break;
                        case "textBoxM2R":
                        case "textBoxM2G":
                        case "textBoxM2B":
                            r = Convert.ToByte(textBoxM2R.Text);
                            g = Convert.ToByte(textBoxM2G.Text);
                            b = Convert.ToByte(textBoxM2B.Text);
                            _monitor2 = System.Drawing.Color.FromArgb(r, g, b);
                            rectM2.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
                            break;
                    }
                }
                catch (System.FormatException ex)
                {

                }
                catch (System.OverflowException ex)
                {

                }
            }
        }

        private void buttonSetPortClick(object sender, RoutedEventArgs e)
        {
            string port = comboPortList.SelectedItem as string;
            _pixel = new Neopixel(port);
        }

        private void buttonRefreshPortClick(object sender, RoutedEventArgs e)
        {
            _port = new Serial();
            comboPortList.ItemsSource = _port.AvailablePorts;
        }

        private void buttonApplyClick(object sender, RoutedEventArgs e)
        {
            if (_pixel != null && _pixel.PortOpen())
            {
                _pixel.SetMonitorColorsSim(_monitor1, _monitor2);
                textBlockFailMsg.Visibility = Visibility.Hidden;
            }
            else
            {
                textBlockFailMsg.Visibility = Visibility.Visible;
                timer.Start();
                
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(del, null);
            timer.Stop();
        }

        void setTextBlockHidden()
        {
            textBlockFailMsg.Visibility = Visibility.Hidden;
        }
    }
}
