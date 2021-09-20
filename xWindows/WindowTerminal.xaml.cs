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
    public partial class WindowTerminal : Window
    {
        private static WindowTerminal window;
        private static ObservableCollection<ReceivePacketInfo> note_info;
        private static ObservableCollection<ReceivePacketInfo> note_loge;

        public static xAction<bool, byte[]> Transmitter;

        public WindowTerminal()
        {
            InitializeComponent();
            ListViewInfo.ItemsSource = note_info;
            ListViewLog.ItemsSource = note_loge;
        }

        public static ObservableCollection<ReceivePacketInfo> NoteInfo
        {
            get { return note_info; }
            set { note_info = value; }
        }

        public static ObservableCollection<ReceivePacketInfo> NoteLog
        {
            get { return note_loge; }
            set { note_loge = value; }
        }

        public static void OpenClick(object sender, RoutedEventArgs e)
        {
            if (window == null)
            {
                window = new WindowTerminal();
                window.Closed += new EventHandler(Close_Click);
                window.Show();
            }
            else window.Activate();
        }

        public static void Close_Click(object sender, EventArgs e) { window?.Close(); window = null; }
        public static void Dispose() { window?.Close(); window = null; }

        private void ButSend_Click(object sender, RoutedEventArgs e)
        {
            //xTcp.Send(TextBoxData.Text);
        }

        private void ButClear_Click(object sender, RoutedEventArgs e)
        {
            TabItem item = (TabItem)TabControl.SelectedItem;
            if (item.Header.ToString() == "Log" && note_loge != null) { note_loge.Clear(); return; }
            if (item.Header.ToString() == "Info" && note_info != null) { note_info.Clear(); return; }
        }

        private void ButPause_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
