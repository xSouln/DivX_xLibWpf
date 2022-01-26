using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using xLib.UI_Propertys;

namespace xLib.Sources
{
    public class xTcpServer : NotifyPropertyChanged
    {
        public class ClientStream
        {
            private xReceiver receiver;
            private Thread thread;
            private TcpClient client;
            private Semaphore semaphore;
            private NetworkStream stream;
            private xList<ClientStream> list;

            public bool IsConnected;

            public void thred_handler()
            {
                int count;
                byte[] buf = new byte[0xffff];
                try
                {
                    stream = client.GetStream();
                    IsConnected = true;
                    list?.Add(this);
                    xTracer.Message("tcp client: thread start");

                    while (true)
                    {
                        do
                        {
                            count = stream.Read(buf, 0, buf.Length);
                            if (count == 0) { return; }
                            else { for (int i = 0; i < count; i++) { receiver.Add(buf[i]); } }
                        }
                        while ((bool)stream?.DataAvailable);
                    }
                }
                catch (Exception e) { xTracer.Message(e.ToString()); }
                finally
                {
                    xTracer.Message("tcp client: thread close");
                    Dispose();
                }
            }

            public ClientStream(Semaphore semaphore, xList<ClientStream> list, TcpClient client, xReceiver receiver)
            {
                Dispose();

                this.client = client;
                this.semaphore = semaphore;
                this.receiver = receiver;
                this.list = list;

                receiver.Context = this;

                thread = new Thread(thred_handler);
                thread.Start();
            }

            public void Dispose()
            {
                try
                {
                    IsConnected = false;
                    if (stream != null) { stream.Flush(); stream.Close(); stream = null; }
                    if (client != null) { client.Client?.Close(); client.Close(); client = null; }
                    if (thread != null) { thread.Abort(); thread = null; }
                }
                finally
                {
                    list?.Remove(this);
                    if (semaphore != null && !semaphore.SafeWaitHandle.IsClosed) { semaphore.Release(); semaphore = null; }
                }
            }

            public bool Send(byte[] data)
            {
                if (client != null && stream != null && client.Connected && data != null && data.Length > 0)
                {
                    try { stream.Write(data, 0, data.Length); return true; }
                    catch { return false; }
                }
                return false;
            }
        }
        //=====================================================================================================================
        public class UIPropertys
        {
            public UI_Property<string, string> Ip = new UI_Property<string, string>() { Name = nameof(Ip) };
            public UI_Property<int, int> Port = new UI_Property<int, int>() { Name = nameof(Port) };
            public UI_Property<int, int> Connections = new UI_Property<int, int>() { Name = nameof(Connections) };
            public UI_Property<int, int> QueueSize = new UI_Property<int, int>() { Name = nameof(QueueSize) };
            public UI_Property<bool, bool> IsStarted = new UI_Property<bool, bool>() { Name = nameof(IsStarted) };
            public UI_Property<string, string> ButConnection = new UI_Property<string, string>() { Name = nameof(ButConnection) };
        }
        //=====================================================================================================================
        private TcpListener server;
        private Thread server_thread;

        private xList<ClientStream> clients = new xList<ClientStream>();
        private Semaphore semaphore_queue_size;

        //private string ip = "127.0.0.10";
        //private int port = 10000;
        private bool is_started;
        //private Brush state_background;
        //private string request_address;
        //private int queue_size = 3;
        private int connection_count = 0;

        public xAction<string> Tracer;
        public xEvent<xReceiverData> EventReceivePacket;

        public UIPropertys Propertys;

        //private void trace(string note) { Tracer?.Invoke(note); xTracer.Message(note); }
        public xTcpServer()
        {
            Propertys = new UIPropertys()
            {
                Ip =
                {
                    Value = "127.0.0.10",
                    ValueParseRule = (last, request) =>
                    {
                        if (request.Split('.').Length != 4) { return last; }
                        try { IPAddress.Parse(request); }
                        catch { return last; }
                        return request;
                    }
                },

                Port = { Value = 10000 },
                QueueSize = { Value = 3 },
                IsStarted = { BackgroundValueRule = (property) => { return property.Value ? UI_Property.GREEN : UI_Property.RED; } },
                ButConnection = { Value = "Start", BackgroundValue = UI_Property.GREEN }
            };
            clients.EventCountChanged += (arg) => { Propertys.Connections.Value = arg; };
        }

        public string Ip
        {
            get { return Propertys.Ip.Value; }
            protected set
            {
                try { IPAddress.Parse(value); }
                catch { return; }

                Propertys.Ip.Value = value;
            }
        }

        public int Port
        {
            get { return Propertys.Port.Value; }
            protected set { Propertys.Port.Value = value; }
        }

        public int QueueSize
        {
            get { return Propertys.QueueSize.Value; }
            protected set { Propertys.QueueSize.Value = value; }
        }

        public int Connections => connection_count;

        public string Address => Ip + ":" + Port;

        public bool IsStarted
        {
            get { return is_started; }
            set
            {
                is_started = value;

                if (value) { Propertys.ButConnection.Value = "Stop"; Propertys.ButConnection.BackgroundValue = UI_Property.RED; }
                else { Propertys.ButConnection.Value = "Start"; Propertys.ButConnection.BackgroundValue = UI_Property.GREEN; }
            }
        }

        public void server_thred_handler()
        {
            semaphore_queue_size = new Semaphore(QueueSize, QueueSize);
            xTracer.Message("tcp server: server started");
            IPAddress localAddr = IPAddress.Parse(Ip);
            server = new TcpListener(localAddr, Port);

            try
            {
                server.Start();
                IsStarted = true;
                while (true)
                {
                    semaphore_queue_size.WaitOne();
                    TcpClient client = server.AcceptTcpClient();

                    xTracer.Message("tcp server: client accept");
                    new ClientStream(semaphore_queue_size, clients, client, new xReceiver(10000, new byte[] { (byte)'\r' }) { EventPacketReceive = EventReceivePacket });
                }
            }
            catch (Exception ex)
            {
                xTracer.Message("tcp server: " + ex);
            }
            finally
            {
                server?.Stop();
                server = null;

                server_thread?.Abort();
                server_thread = null;

                semaphore_queue_size?.Close();
                semaphore_queue_size?.Dispose();
                semaphore_queue_size = null;

                IsStarted = false;
            }
        }

        public bool Start()
        {
            if (server != null) { return false; }

            server_thread = new Thread(server_thred_handler);
            server_thread.Start();
            return false;
        }

        public bool Stop()
        {
            server?.Stop();
            server_thread?.Abort();
            server_thread = null;

            while (clients.Values.Count > 0) { clients.Values[0].Dispose(); }
            return false;
        }
    }
}
