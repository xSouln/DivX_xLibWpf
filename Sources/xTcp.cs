using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using xLib;
using xLib.UI_Propertys;

namespace xLib
{
    public class xTcp : NotifyPropertyChanged
    {
        private object state_background;

        public xEvent EventDisconnected;
        public xEvent EventConnected;

        public xAction<string> Tracer;
        private TcpClient client;
        private NetworkStream stream;
        private Thread server_thread;

        public string LastAddress;
        public string Ip;
        public int Port;
        public bool is_connected = false;
        public int UpdatePeriod = 1;

        public xReceiver xRx;
        private void trace(string note) { Tracer?.Invoke(note); xTracer.Message(note); }

        public xTcp()
        {
            Background = UI_Property.RED;
            xRx = new xReceiver(10000, new byte[] { (byte)'\r' });
            xRx.Clear();
        }

        public object Background
        {
            set { state_background = value; OnPropertyChanged(nameof(Background)); }
            get { return state_background; }
        }

        public bool IsConnected
        {
            get { return is_connected; }
            set
            {
                if (is_connected != value)
                {
                    is_connected = value;

                    if (is_connected) { Background = UI_Property.GREEN; EventConnected?.Invoke(this); }
                    else { Background = UI_Property.RED; EventDisconnected?.Invoke(this); }
                    OnPropertyChanged(nameof(IsConnected));
                }                
            }
        }

        private void rx_thread()
        {
            if (client == null) { trace("tcp: client == null"); thread_close(); }
            try
            {
                stream = client.GetStream();
                IsConnected = true;
                trace("tcp: thread start");

                int count = 0;
                byte[] buf = new byte[100000];
                xRx.Clear();

                while (true)
                {
                    do
                    {
                        count = stream.Read(buf, 0, buf.Length);
                        if (count > 0) { for (int i = 0; i < count; i++) xRx.Add(buf[i]); }
                    }
                    while ((bool)stream?.DataAvailable);
                }
            }
            catch (Exception e) { trace(e.ToString()); thread_close(); }
        }

        private bool thread_close()
        {
            if (stream != null) { stream.Flush(); stream.Close(); stream = null; }
            if (client != null)
            {
                client.Client?.Close();
                client.Close();
                client = null;
            }
            server_thread?.Abort();
            server_thread = null;
            trace("tcp: thread close");

            IsConnected = false;
            return true;
        }

        private void request_callback(IAsyncResult ar)
        {
            try
            {
                client = (TcpClient)ar.AsyncState;
                if (client != null) { trace("tcp: client connected"); server_thread = new Thread(new ThreadStart(rx_thread)); server_thread.Start(); }
                else trace("tcp: client connect error");
            }
            catch (Exception ex)
            {
                trace(ex.ToString());
                trace("tcp: client connect abort");
                thread_close();
                return;
            }
        }

        public void Connect(string address)
        {
            string[] strs;

            if (client != null) { trace("tcp: device is connected"); return; }
            trace("tcp: request connect");

            if (address.Length < 9) { trace("tcp: incorrect parameters"); return; }
            strs = address.Split('.');
            if (strs.Length < 4) { trace("tcp: incorrect parameters"); return; }

            strs = address.Split(':');
            if (strs.Length != 2) { trace("tcp: incorrect parameters"); return; }

            int port = Convert.ToInt32(strs[1]);
            string ip = strs[0];

            Ip = ip;
            Port = port;
            client = new TcpClient();

            LastAddress = address;

            trace("tcp: client begin connect");
            IAsyncResult result = client.BeginConnect(Ip, Port, request_callback, client);
        }
        //=====================================================================================================================
        public void Disconnect()
        {
            trace("tcp: request disconnect");
            thread_close();
        }
        //=====================================================================================================================
        public bool Send(string str)
        {
            if (client != null && stream != null && client.Connected)
            {
                byte[] data = Encoding.UTF8.GetBytes(str + "\r");
                trace("tcp send: " + str);

                try { stream.Write(data, 0, data.Length); return true; }
                catch { trace("tcp: невозможно отправить на указаный ip"); return false; }
            }
            //trace("tcp: нет соединения");
            return false;
        }
        //=====================================================================================================================
        public bool Send(byte[] data)
        {
            if (client != null && stream != null && client.Connected && data != null && data.Length > 0)
            {
                try { stream.Write(data, 0, data.Length); return true; }
                catch { trace("tcp: невозможно отправить на указаный ip"); return false; }
            }
            //trace("tcp: нет соединения");
            return false;
        }
        //=====================================================================================================================
    }
}
