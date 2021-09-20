using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib
{
    public enum xRxResponse { Accept = 0, Storage = 1, BanTransition = 2 }

    public unsafe class xReceiverData
    {
        public xReceiver xRx;
        public xContent Content;
    }

    public delegate void xReceiverCallback(xReceiverData ReceiverData);

    public class xReceiver
    {
        public struct xReceiverBufT
        {
            public byte[] Data;
            public int ByteRecived;
        }

        public xEvent<xReceiverData> EventPacketReceive;

        public byte[] end_line;
        public xReceiverBufT Buf = new xReceiverBufT();

        public xRxResponse Response = xRxResponse.Accept;
        public bool Loock = false;

        public xReceiver(int BufSize, byte[] EndLine)
        {
            end_line = EndLine;
            Buf.Data = new byte[BufSize];
            Buf.ByteRecived = 0;

            if (end_line == null) end_line = new byte[] { (byte)'\r' };
        }
        private unsafe void BufLoaded()
        {
            if (EventPacketReceive != null)
            {
                Response = xRxResponse.Accept;
                xReceiverData data = new xReceiverData();
                data.xRx = this;
                data.Content.Size = Buf.ByteRecived;
                fixed (byte* ptr = Buf.Data) { data.Content.Obj = ptr; EventPacketReceive(data); }
            }
            if (Response == xRxResponse.Accept) { Buf.ByteRecived = 0; }
        }

        private unsafe void EndLineIdentify()
        {
            if (EventPacketReceive != null)
            {
                Response = xRxResponse.Accept;
                xReceiverData data = new xReceiverData();
                data.xRx = this;
                data.Content.Size = Buf.ByteRecived - end_line.Length;
                fixed (byte* ptr = Buf.Data) { data.Content.Obj = ptr; EventPacketReceive(data); }
            }
            if (Response == xRxResponse.Accept) { Buf.ByteRecived = 0; }
            else { }
        }

        public void Add(byte data)
        {
            Buf.Data[Buf.ByteRecived] = data;
            Buf.ByteRecived++;

            if (Buf.ByteRecived > end_line.Length) {
                int i = end_line.Length;
                int j = Buf.ByteRecived;
                while (i > 0)
                {
                    i--;
                    j--;
                    if (end_line[i] != Buf.Data[j]) { goto verify_end; }
                }
                EndLineIdentify();
            }

        verify_end:
            if (Buf.ByteRecived == Buf.Data.Length) { BufLoaded(); Buf.ByteRecived = 0; }
        }

        public void Clear()
        {
            Response = xRxResponse.Accept;
            Buf.ByteRecived = 0;
            Loock = false;
        }
    }
}
