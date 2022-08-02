using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
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
using System.Windows.Shapes;
using xLib.Net;
using xLib.Transceiver;

namespace xLib.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowComPortConnection.xaml
    /// </summary>
    public partial class WindowSerialPort : Window
    {
        public static WindowSerialPort window;
        public static Ports.xSerialPort SerialPort { get; set; }

        public WindowSerialPort()
        {
            InitializeComponent();

            GridPropertys.DataContext = SerialPort;
            ButConnection.DataContext = SerialPort?.ButtonConnection;

            DataContext = this;

            Closed += WindowSerialPortClosed;
        }

        private void WindowSerialPortClosed(object sender, EventArgs e)
        {

        }

        private void ConnectBut_Click(object sender, RoutedEventArgs e)
        {
            if (FindComPortsBox.SelectedIndex != -1)
            {
                if ((bool)SerialPort?.IsConnected)
                {
                    SerialPort?.Disconnect();
                }
                else
                {
                    SerialPort?.Connect((string)FindComPortsBox.SelectedValue);
                }
            }
        }

        private void DisconnectBut_Click(object sender, RoutedEventArgs e)
        {
            SerialPort?.Disconnect();
        }

        public static void OpenClick(object sender, RoutedEventArgs e)
        {
            if (window == null)
            {
                window = new WindowSerialPort();
                window.Closed += new EventHandler(Close_Click);
                window.Show();
            }
            else window.Activate();
        }

        public static void Close_Click(object sender, EventArgs e)
        {
            window?.Close();
            window = null;
        }

        public static void Dispose()
        {
            if (SerialPort != null)
            {
                Properties.Settings.Default.SerialPortOptions = SerialPort.SerialPortOptions;
                Properties.Settings.Default.Save();
                SerialPort.Disconnect();
            }
            window?.Close();
            window = null;
        }
    }
}
