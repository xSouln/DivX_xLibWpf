using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using xLib.Sources;

namespace xLib.xWindows
{
    /// <summary>
    /// Логика взаимодействия для WindowTcpServer.xaml
    /// </summary>
    public partial class WindowTcpServer : Window
    {
        public static WindowTcpServer window;
        public static xTcpServer TcpServer;
        public static ObservableCollection<object> Propertys { get; set; }

        public WindowTcpServer()
        {
            InitializeComponent();
            ListViewPtopertys.ItemsSource = Propertys;
        }

        public static void OpenClick(object sender, RoutedEventArgs e)
        {
            if (window == null)
            {
                window = new WindowTcpServer();
                window.Closed += new EventHandler(Close_Click);
                window.Show();
            }
            else window.Activate();
        }

        public static void Close_Click(object sender, EventArgs e) { window?.Close(); window = null; }
        public static void Dispose() { TcpServer?.Stop(); window?.Close(); window = null; }

        private void ButConnection_Click(object sender, RoutedEventArgs e)
        {
            if (TcpServer != null)
            {
                if (TcpServer.IsStarted) { TcpServer.Stop(); }
                else { TcpServer.Start(); }
            }
        }
    }
}
