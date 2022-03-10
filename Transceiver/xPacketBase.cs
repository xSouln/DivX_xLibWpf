using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Transceiver
{
    public class xPacketBase
    {
        public static unsafe void Add<TData>(List<byte> packet, TData data) where TData : unmanaged
        {
            if (packet != null)
            {
                byte* ptr = (byte*)&data;

                for (int i = 0; i < sizeof(TData); i++)
                {
                    packet.Add(ptr[i]);
                }
            }
        }

        public static unsafe void Add(List<byte> packet, void* ptr, int size)
        {
            if (packet != null)
            {
                byte* _ptr = (byte*)ptr;

                while (size > 0)
                {
                    packet.Add(*_ptr);
                    size--;
                }
            }
        }

        public static void Add(List<byte> packet, string data)
        {
            if (packet != null && data != null)
            {
                foreach (byte ch in data)
                {
                    packet.Add(ch);
                }
            }
        }

        public static void Add(List<byte> packet, byte[] data)
        {
            if (packet != null && data != null)
            {
                foreach (byte ch in data)
                {
                    packet.Add(ch);
                }
            }
        }
    }
}
