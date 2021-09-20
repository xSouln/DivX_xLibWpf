using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib
{
    public static class xTracer
    {
        public static ObservableCollection<ReceivePacketInfo> Notes { get; set; } = new ObservableCollection<ReceivePacketInfo>();
        public static ActionAccessUI PointEntryUI;

        public static void Message(string note, string data, string convert_data)
        {
            try
            {
                PointEntryUI?.Invoke((RequestUI) =>
                {
                    if (Notes.Count > 500) Notes.RemoveAt(Notes.Count - 1);

                    Notes.Insert(0, new ReceivePacketInfo
                    {
                        Time = DateTime.Now.ToUniversalTime().ToString(),
                        Note = note,
                        Data = data,
                        ConvertData = convert_data
                    });
                }, null);
            }
            catch { }
        }
        public static void Message(string data) { Message("info:", data, null); }
        public static void Message(string data, string convert_data) { Message("info:", data, convert_data); }

    }
}
