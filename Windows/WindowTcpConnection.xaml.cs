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
using System.Windows.Shapes;
using xLib.Transceiver;

namespace xLib
{
    /// <summary>
    /// Логика взаимодействия для WindowTcpConnection.xaml
    /// </summary>
    public partial class WindowTcpConnection : Window
    {
        public static WindowTcpConnection window;
        public static xTcp Tcp;
        public WindowTcpConnection()
        {
            InitializeComponent();

            TextBoxAddress.Text = Tcp?.LastAddress;
            TextBoxAddress.DataContext = Tcp;
        }

        private void BotTcpConnect_Click(object sender, RoutedEventArgs e)
        {
            Tcp?.Connect(TextBoxAddress.Text);
        }

        private void BotTcpDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Tcp?.Disconnect();
        }

        public static void OpenClick(object sender, RoutedEventArgs e)
        {
            if (window == null)
            {
                window = new WindowTcpConnection();
                window.Closed += new EventHandler(Close_Click);
                window.Show();
            }
            else
            {
                window.Activate();
            }
        }

        public static void Close_Click(object sender, EventArgs e)
        {
            window?.Close();
            window = null;
        }

        public static void Dispose()
        {
            window?.Close();
            window = null;
        }
    }
}
