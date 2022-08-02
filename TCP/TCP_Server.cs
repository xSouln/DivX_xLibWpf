using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using xLib.Common;
using xLib.UI;

namespace xLib.Net
{
    public class TCP_Server : UINotifyPropertyChanged
    {
        public class ClientStream
        {
            private xObjectReceiver receiver;
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

            public ClientStream(Semaphore semaphore, xList<ClientStream> list, TcpClient client, xObjectReceiver receiver)
            {
                Dispose();

                this.client = client;
                this.semaphore = semaphore;
                this.receiver = receiver;
                this.list = list;

                receiver.Parent = this;

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
        public class Property<T> : UIProperty<T>
        {
            protected Brush background;

            public Brush Background
            {
                get => background;
                set
                {
                    background = value;
                    OnPropertyChanged(nameof(Background));
                }
            }
        }

        public class PropertyIp : Property<string>
        {
            public override string Value
            {
                get => (string)_value;
                set
                {
                    if (value != null && value.Split('.').Length == 4)
                    {
                        try
                        {
                            IPAddress.Parse(value);
                            _value = value;
                        }
                        catch { }
                        OnPropertyChanged(nameof(Value));
                    }
                }
            }
        }

        public class UIPropertys
        {
            public Property<string> Ip = new Property<string>() { Name = nameof(Ip) };
            public Property<int> Port = new Property<int>() { Name = nameof(Port) };
            public Property<int> Connections = new Property<int>() { Name = nameof(Connections) };
            public Property<int> QueueSize = new Property<int>() { Name = nameof(QueueSize) };
            public Property<bool> IsStarted = new Property<bool>() { Name = nameof(IsStarted) };
            public Property<string> ButConnection = new Property<string>() { Name = nameof(ButConnection) };
        }
        //=====================================================================================================================
        private TcpListener server;
        private Thread server_thread;

        private xList<ClientStream> clients = new xList<ClientStream>();
        private Semaphore semaphore_queue_size;

        private bool is_started;
        private int connection_count = 0;

        public xAction<string> Tracer;
        public xObjectReceiver.EventPacketReceive PacketReceiver;

        public UIPropertys Propertys;

        public TCP_Server()
        {
            Propertys = new UIPropertys()
            {
                Ip = { Value = "127.0.0.10" },
                Port = { Value = 10000 },
                QueueSize = { Value = 3 },
                IsStarted = { EventValueChanged = (property, evt) => { ((Property<bool>)property).Background = property.Value ? UIProperty.GREEN : UIProperty.RED; } },
                ButConnection = { Value = "Start", Background = UIProperty.GREEN }
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

                if (value) { Propertys.ButConnection.Value = "Stop"; Propertys.ButConnection.Background = UIProperty.RED; }
                else { Propertys.ButConnection.Value = "Start"; Propertys.ButConnection.Background = UIProperty.GREEN; }
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
                    new ClientStream(semaphore_queue_size, clients, client, new xObjectReceiver(10000, new byte[] { (byte)'\r' })
                    {
                        PacketReceiver = PacketReceiver
                    });
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
