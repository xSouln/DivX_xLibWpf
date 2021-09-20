using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib
{
    public static class Logger
    {
        public delegate void MessagerReceivePacketInfo(string note, string data, string convert_data );
        public static ObservableCollection<ReceivePacketInfo> NoteReceivePacketInfo { get; set; } = new ObservableCollection<ReceivePacketInfo>();

        public static class Messagers
        {
            public static MessagerReceivePacketInfo ReceivePacketInfo;
        }

        public static class Message
        {
            public static void ReceivePacket(string note, string data, string convert_data)
            {
                string _note = "null";
                string _data = "null";
                string _convert_data = "null";

                if (note != null) _note = note;
                if (data != null) _data = data;
                if (_convert_data != null) { _convert_data = convert_data; }

                Messagers.ReceivePacketInfo?.Invoke(_note, _data, _convert_data);
            }
        }        
    }
}
