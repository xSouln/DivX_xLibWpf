using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Ports
{
    [Serializable]
    public class Options
    {
        public int BoadRate = 115200;
        public bool ConnectionState = false;
        public string LastConnectedPortName = "";
    }
}
