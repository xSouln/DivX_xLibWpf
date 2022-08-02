using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Device
{
    public interface IDevice
    {
        string Name { get; set; }
        Terminal Parent { get; set; }


    }
}
