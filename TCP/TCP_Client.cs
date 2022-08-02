using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using xLib;
using xLib.Common;
using xLib.UI;

namespace xLib.Net
{
    public class TCP_Client : UINotifyPropertyChanged
    {
        private object background_state;

        public xEvent<TCP_Client> EventDisconnected;
        public xEvent<TCP_Client> EventConnected;

        public xAction<string> Tracer;
        private TcpClient client;
        private NetworkStream stream;
        private Thread client_thread;

        private string last_address;
        private bool is_connected = false;
        private int transmit_deadtime = 1;

        public string Ip;
        public int Port;
        public int UpdatePeriod = 1;

        private List<byte[]> transmit_line = new List<byte[]>();
        private AutoResetEvent transmit_line_synchronizer = new AutoResetEvent(true);
        private Thread transmit_line_thread;

        public xObjectReceiver Receiver;
        private void trace(string note)
        {
            Tracer?.Invoke(note);
            xTracer.Message(note);
        }

        public TCP_Client()
        {
            BackgroundState = UIProperty.RED;
        }

        public object BackgroundState
        {
            set { background_state = value; OnPropertyChanged(nameof(BackgroundState)); }
            get { return background_state; }
        }

        public int TransmitDeadtime
        {
            set { if (value < 1) { return; } transmit_deadtime = value; }
            get => transmit_deadtime;
        }

        public string LastAddress
        {
            get { return last_address; }
            set
            {
                last_address = value;
                OnPropertyChanged(nameof(LastAddress));
            }
        }

        public bool IsConnected
        {
            get { return is_connected; }
            set
            {
                if (is_connected != value)
                {
                    is_connected = value;

                    if (value) { EventConnected?.Invoke(this); }
                    else { EventDisconnected?.Invoke(this); }
                    OnPropertyChanged(nameof(IsConnected));
                }

                if (is_connected && background_state != UIProperty.GREEN) { BackgroundState = UIProperty.GREEN; }
                else if (!is_connected && background_state != UIProperty.RED) { BackgroundState = UIProperty.RED; }
            }
        }

        private void rx_thread()
        {
            if (client == null)
            {
                trace("tcp client: client == null");
                thread_close();
            }
            try
            {
                client.ReceiveBufferSize = 0x10000;
                stream = client.GetStream();
                stream.Flush();
                IsConnected = true;
                trace("tcp client: thread start");
                client.ReceiveBufferSize = 1000000;

                int count = 0;
                byte[] buf = new byte[1000000];
                Receiver.Clear();

                while (true)
                {
                    do
                    {
                        count = stream.Read(buf, 0, buf.Length);
                        for (int i = 0; i < count; i++)
                        {
                            Receiver.Add(buf[i]);
                        }
                    }
                    while ((bool)stream?.DataAvailable);
                }
            }
            catch (Exception e)
            {
                trace(e.ToString());
                thread_close();
            }
        }

        private void thread_close()
        {
            if (stream != null)
            {
                stream.Flush();
                stream.Close();
                stream = null;
            }

            if (client != null)
            {
                client.Client?.Close();
                client.Close();
                client = null;
            }

            try
            {
                client_thread?.Abort();
                transmit_line_thread?.Abort();
            }
            finally
            {
                trace("tcp client: thread close");
                transmit_line.Clear();
                client_thread = null;
                transmit_line_thread = null;
                IsConnected = false;
            }
        }

        private void request_callback(IAsyncResult ar)
        {
            try
            {
                client = (TcpClient)ar.AsyncState;
                if (client != null)
                {
                    trace("tcp: client connected");
                    /*
                    transmit_line.Clear();
                    transmit_line_thread = new Thread(transmit_line_thread_handler);
                    transmit_line_thread.Start();
                    */
                    client_thread = new Thread(new ThreadStart(rx_thread));
                    client_thread.Start();
                }
                else
                {
                    trace("tcp client: client connect error");
                }
            }
            catch (Exception ex)
            {
                trace(ex.ToString());
                trace("tcp client: client connect abort");
                thread_close();
                return;
            }
        }

        public void Connect(string address)
        {
            string[] strs;

            if (address == null) { trace("tcp client: address == null"); return; }

            if (client != null) { trace("tcp client: device is connected"); return; }
            trace("tcp client: request connect");

            if (address.Length < 9) { trace("tcp client: incorrect parameters"); return; }
            strs = address.Split('.');
            if (strs.Length < 4) { trace("tcp client: incorrect parameters"); return; }

            strs = address.Split(':');
            if (strs.Length != 2) { trace("tcp client: incorrect parameters"); return; }

            int port = Convert.ToInt32(strs[1]);
            string ip = strs[0];

            Ip = ip;
            Port = port;
            client = new TcpClient();

            LastAddress = address;

            trace("tcp client: client begin connect");
            IAsyncResult result = client.BeginConnect(Ip, Port, request_callback, client);
        }
        //=====================================================================================================================
        public void Disconnect()
        {
            trace("tcp client: request disconnect");
            thread_close();
        }
        //=====================================================================================================================
        public bool Send(string str)
        {
            if (client != null && stream != null && client.Connected)
            {
                byte[] data = Encoding.UTF8.GetBytes(str + "\r");
                trace("tcp client send: " + str);

                try { stream.Write(data, 0, data.Length); return true; }
                catch { trace("tcp client: невозможно отправить на указаный ip"); return false; }
            }
            //trace("tcp: нет соединения");
            return false;
        }

        public bool Send(byte[] data)
        {
            if (client != null && stream != null && client.Connected && data != null && data.Length > 0)
            {
                try { stream.Write(data, 0, data.Length); return true; }
                catch { trace("tcp client: невозможно отправить на указаный ip"); return false; }
            }
            //trace("tcp: нет соединения");
            return false;
        }
        //=====================================================================================================================
        private void transmit_line_thread_handler()
        {
            while (true)
            {
                if (transmit_line.Count > 0)
                {
                    Send(transmit_line[0]);

                    transmit_line_synchronizer.WaitOne();
                    transmit_line.RemoveAt(0);
                    transmit_line_synchronizer.Set();
                }
                Thread.Sleep(transmit_deadtime);
            }
        }

        public bool InLineAdd(byte[] data)
        {
            if (client != null && stream != null && client.Connected && data != null && data.Length > 0)
            {
                try
                {
                    transmit_line_synchronizer.WaitOne();
                    transmit_line.Add(data);
                    return true;
                }
                finally
                {
                    transmit_line_synchronizer.Set();
                }
            }
            return false;
        }
        //=====================================================================================================================
    }
}
