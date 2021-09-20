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

namespace xLib
{
    /// <summary>
    /// Логика взаимодействия для InfoWindow.xaml
    /// </summary>
    public partial class TerminalWindow : Window
    {
        private static TerminalWindow window_terminal;
        public ObservableCollection<ReceivePacketInfo> NoteReceivePacketInfo { get; set; }
        public TerminalWindow()
        {
            NoteReceivePacketInfo = Logger.NoteReceivePacketInfo;

            InitializeComponent();

            DataContext = this;
        }

        public static void Open_Click(object sender, RoutedEventArgs e)
        {
            if (window_terminal == null)
            {
                window_terminal = new TerminalWindow();
                window_terminal.Closed += new EventHandler(Close_Click);
                window_terminal.Show();
            }
            else window_terminal.Activate();
        }

        public static void Close_Click(object sender, EventArgs e)
        {
            window_terminal?.Close();
            window_terminal = null;
        }

        public static void Close_Click()
        {
            window_terminal?.Close();
            window_terminal = null;
        }
    }
}
