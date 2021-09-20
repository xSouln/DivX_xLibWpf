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
using xLib.Transceiver;

namespace xLib
{
    /// <summary>
    /// Логика взаимодействия для WindowComPortConnection.xaml
    /// </summary>
    public partial class WindowComPortConnection : Window
    {
        public static WindowComPortConnection window;
        public static xSerialPort SerialPort;

        public WindowComPortConnection()
        {
            InitializeComponent();

            //FindComPortsBox.ItemsSource = SerialPort.PortList;
            GridPropertys.DataContext = SerialPort;
            DataContext = this;
        }

        private void ConnectBut_Click(object sender, RoutedEventArgs e)
        {
            if (FindComPortsBox.SelectedIndex != -1)
            {
                SerialPort.Connect((string)FindComPortsBox.SelectedValue);
            }
        }

        private void DisconnectBut_Click(object sender, RoutedEventArgs e)
        {
            SerialPort.Disconnect();
        }

        private void TransmitBut_Click(object sender, RoutedEventArgs e)
        {
            if (TransmitDataTextBox.Text.Length > 0) { SerialPort.Send(TransmitDataTextBox.Text); }
        }

        public static void OpenClick(object sender, RoutedEventArgs e)
        {
            if (window == null)
            {
                window = new WindowComPortConnection();
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
            SerialPort?.Disconnect();
            window?.Close();
            window = null;
        }
    }
}
