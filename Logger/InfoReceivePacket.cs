using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib
{
    public class ReceivePacketInfo
    {
        private string time = "";
        private string note = "";
        private string data = "";
        private string convert_data = "";

        public string Time
        {
            get { return time; }
            set { time = value; }
        }

        public string Note
        {
            get { return note; }
            set { note = value; }
        }

        public string Data
        {
            get { return data; }
            set { data = value; }
        }

        public string ConvertData
        {
            get { return convert_data; }
            set { convert_data = value; }
        }
    }
}
