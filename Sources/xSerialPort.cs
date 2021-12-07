using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using xLib.Transceiver;
using xLib.UI_Propertys;

namespace xLib
{
    [Serializable]
    public class xSerialPortOptions
    {
        public int BoadRate = 115200;
        public bool ConnectionState = false;
        public string LastConnectedPortName = "";
    }

    public class xSerialPort : NotifyPropertyChanged
    {
        public const string FILE_NAME_COMPORT_OPTIONS = "ComPortOption.xDat";
        public xAction<string> Tracer;

        public event xEventChangeState<xSerialPort, bool> ConnectionStateChanged;
        public event xEventChangeState<xSerialPort, string> EventFindeLastConnectedPortName;

        //private Thread RxThread;
        private Timer timer_finde_ports;
        private Timer timer_update_rx;

        public SerialPort Port;
        public ObservableCollection<string> PortList { get; set; } = new ObservableCollection<string>();

        protected bool is_connected;
        protected int boad_rate = 115200;
        protected string port_name = "";
        protected string last_selected_port_name = "";
        protected Brush background_state = UI_Property.RED;

        public xReceiver Receiver;

        public List<int> BaudRateList { get; set; } = new List<int>() { 9600, 38400, 115200, 128000, 256000, 521600, 840000, 900000, 921600 };

        public xSerialPort() { timer_finde_ports = new Timer(finde_ports, null, 1000, 1000); }

        private void trace(string note) { Tracer?.Invoke(note); xTracer.Message(note); }

        public bool SelectIsEnable => !IsConnected;

        public xSerialPortOptions SerialPortOptions
        {
            get { return new xSerialPortOptions { BoadRate = boad_rate, LastConnectedPortName = last_selected_port_name, ConnectionState = IsConnected }; }
            set
            {
                if (value != null)
                {
                    BoadRate = value.BoadRate;
                    PortName = value.LastConnectedPortName;
                }
            }
        }

        public Brush BackgroundState
        {
            get { return background_state; }
            set { background_state = value; OnPropertyChanged(nameof(BackgroundState)); }
        }

        public bool IsConnected
        {
            get { return is_connected; }
            set
            {
                if (value && background_state != UI_Property.GREEN) { BackgroundState = UI_Property.GREEN; }
                else if (!value && background_state != UI_Property.RED) { BackgroundState = UI_Property.RED; }

                if (is_connected != value)
                {
                    is_connected = value;
                    OnPropertyChanged(nameof(IsConnected));
                    OnPropertyChanged(nameof(SelectIsEnable));
                    ConnectionStateChanged?.Invoke(this, value);
                }
            }
        }

        public string PortName
        {
            get { return port_name; }
            set
            {
                port_name = value;
                if (port_name.Length > 0) { last_selected_port_name = value; }
                OnPropertyChanged(nameof(PortName));
            }
        }

        public int BoadRate
        {
            get { return boad_rate; }
            set
            {
                if (boad_rate != value)
                {
                    if (Port != null)
                    {
                        Port.BaudRate = value;
                    }
                    boad_rate = value;
                    trace("" + PortName + "(boad rate changed at " + boad_rate + ")");
                    OnPropertyChanged(nameof(BoadRate));
                }
            }
        }

        private static void read_data(object arg)
        {
            xSerialPort serial_port = (xSerialPort)arg;

            if (serial_port != null && serial_port.Port != null && serial_port.Port.IsOpen)
            {
                while (serial_port.Port.BytesToRead > 0) { serial_port.Receiver.Add((byte)serial_port.Port.ReadByte()); }
                return;
            }
            serial_port.trace(serial_port.PortName + "(boadrate: " + serial_port.BoadRate + "): error read data");
            serial_port.Disconnect();
        }

        public bool Connect(string name)
        {
            if (name == null || name.Length < 3) { return false; }
            if (Port != null && Port.IsOpen) { return false; }

            try
            {
                if (Receiver == null) { Receiver = new xReceiver(10000, new byte[] { (byte)'\r', (byte)'\n' }); }
                Port = new SerialPort(name, BoadRate, Parity.None, 8, StopBits.One);
                Port.Encoding = Encoding.GetEncoding("iso-8859-1");
                Port.ReadBufferSize = 100000;
                Port.WriteBufferSize = 100000;

                Port.Open();
                PortName = name;
                Receiver.Clear();
                trace(name + "(boadrate: " + BoadRate + "): rx thred started");

                IsConnected = true;
                timer_update_rx = new Timer(read_data, this, 1000, 10);
                trace(name + "(boadrate: " + BoadRate + "): connected");
            }
            catch (Exception ex)
            {
                trace(name + "(boadrate: " + BoadRate + "): error connect");
                Disconnect();
            }

            return IsConnected;
        }

        public void Disconnect()
        {
            timer_update_rx?.Dispose();
            Port?.Close();
            Port = null;
            IsConnected = false;
            trace(PortName + "(boadrate: " + BoadRate + "): disconnected");
        }

        public void Dispose() { timer_finde_ports.Dispose(); Disconnect(); }

        private static void update_port_list(object arg)
        {
            (ObservableCollection<string> ports, List<string> remove, List<string> add) res = ((ObservableCollection<string> ports, List<string> remove, List<string> add))arg;

            foreach (string name in res.remove)
            {
                int i = 0;
                while (i < res.ports.Count) { if (xConverter.Compare(name, res.ports[i])) { res.ports.RemoveAt(i); } else { i++; } }
            }

            foreach (string name in res.add) { res.ports.Add(name); }
        }

        private void finde_ports(object obj)
        {
            List<string> Ports = SerialPort.GetPortNames().ToList<string>();
            List<string> TotalPorts = new List<string>();

            int count = TotalPorts.Count;

            foreach (string name in PortList)
            {
                TotalPorts.Add(name);
                if (PortName.Length == 0 && xConverter.Compare(last_selected_port_name, name))
                {
                    PortName = name;
                    trace(PortName + "(finde last selected port name)");
                    EventFindeLastConnectedPortName?.Invoke(this, name);
                }
            }

            int i = 0;
            while (i < TotalPorts.Count && Ports.Count > 0)
            {
                int j = 0;
                while (j < Ports.Count)
                {
                    if (xConverter.Compare(TotalPorts[i], Ports[j])) { TotalPorts.RemoveAt(i); Ports.RemoveAt(j); goto end_while; }
                    j++;
                }
                i++;
            end_while:;
            }

            if (TotalPorts.Count != Ports.Count || count != TotalPorts.Count) { xSupport.ActionThreadUI<(ObservableCollection<string>, List<string>, List<string>)>(update_port_list, (PortList, TotalPorts, Ports)); }
        }

        public bool Send(string str)
        {
            if (Port == null || !Port.IsOpen || str == null || str.Length == 0) { return false; }
            Port.Write(str);
            return true;
        }

        public bool Send(byte[] data)
        {
            if (Port != null && Port.IsOpen && data != null && data.Length > 0) { Port.Write(data, 0, data.Length); return true; }
            return false;
        }
    }
}
