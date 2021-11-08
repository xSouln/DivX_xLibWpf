using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Transceiver
{
    public class xResponseBuilder
    {
        protected static unsafe void add_data(List<byte> request, void* obj, int obj_size) { if (request != null && obj != null && obj_size > 0) { for (int i = 0; i < obj_size; i++) { request.Add(((byte*)obj)[i]); } } }
        protected static unsafe void add_data(List<byte> request, string str) { if (request != null && str != null) { for (int i = 0; i < str.Length; i++) { request.Add((byte)str[i]); } } }
        protected static unsafe void add_data(List<byte> request, byte[] data) { if (request != null && data != null) { for (int i = 0; i < data.Length; i++) { request.Add(data[i]); } } }
    }
}
