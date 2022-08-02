using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Common
{
    public class xObjectReceiver
    {
        public enum Result
        {
            Reset,
            Storage,
        }

        public unsafe delegate Result EventPacketReceive(xObjectReceiver rx, byte* data, int data_size);

        public EventPacketReceive PacketReceiver;

        public byte[] EndLine;
        public byte[] Data;
        public int ByteRecived;

        public Result Response = Result.Reset;

        public object Parent;

        public xObjectReceiver(int BufSize, byte[] EndLine)
        {
            this.EndLine = EndLine;
            Data = new byte[BufSize];
            ByteRecived = 0;

            if (this.EndLine == null)
            {
                this.EndLine = new byte[] { (byte)'\r' };
            }
        }
        private unsafe void BufLoaded()
        {
            ByteRecived = 0;
        }

        private unsafe void EndLineIdentify()
        {
            if (PacketReceiver != null)
            {
                fixed (byte* ptr = Data)
                {
                    Response = PacketReceiver(this, ptr, ByteRecived - EndLine.Length);
                }
            }

            if (Response == Result.Reset)
            {
                ByteRecived = 0;
            }
        }

        public void Add(byte data)
        {
            Data[ByteRecived] = data;
            ByteRecived++;

            if (ByteRecived > EndLine.Length)
            {
                int i = EndLine.Length;
                int j = ByteRecived;
                while (i > 0)
                {
                    i--;
                    j--;
                    if (EndLine[i] != Data[j])
                    {
                        goto verify_end;
                    }
                }
                EndLineIdentify();
            }

        verify_end:
            if (ByteRecived >= Data.Length)
            {
                BufLoaded();
            }
        }

        public void Clear()
        {
            Response = Result.Reset;
            ByteRecived = 0;
        }
    }
}
