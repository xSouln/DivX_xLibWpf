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

    public class xReceiver
    {
        public struct xReceiverBufT
        {
            public byte[] Data;
            public int ByteRecived;
        }

        public xEvent<xReceiverData> EventPacketReceive;

        public byte[] EndLine;
        public xReceiverBufT Buf = new xReceiverBufT();
        public xRxResponse Response = xRxResponse.Accept;
        public object Context;

        public xReceiver(int BufSize, byte[] EndLine)
        {
            this.EndLine = EndLine;
            Buf.Data = new byte[BufSize];
            Buf.ByteRecived = 0;

            if (this.EndLine == null) { this.EndLine = new byte[] { (byte)'\r' }; }
        }
        private unsafe void BufLoaded()
        {
            if (EventPacketReceive != null)
            {
                xReceiverData data = new xReceiverData();
                data.xRx = this;
                data.Content.Size = Buf.ByteRecived;
                fixed (byte* ptr = Buf.Data) { data.Content.Obj = ptr; EventPacketReceive(data); }
            }
            Buf.ByteRecived = 0;
        }

        private unsafe void EndLineIdentify()
        {
            if (EventPacketReceive != null)
            {
                Response = xRxResponse.Accept;
                xReceiverData data = new xReceiverData();
                data.xRx = this;
                data.Content.Size = Buf.ByteRecived - EndLine.Length;
                fixed (byte* ptr = Buf.Data) { data.Content.Obj = ptr; EventPacketReceive(data); }
            }

            if (Response == xRxResponse.Accept) { Buf.ByteRecived = 0; }
        }

        public void Add(byte data)
        {
            Buf.Data[Buf.ByteRecived] = data;
            Buf.ByteRecived++;

            if (Buf.ByteRecived > EndLine.Length) {
                int i = EndLine.Length;
                int j = Buf.ByteRecived;
                while (i > 0)
                {
                    i--;
                    j--;
                    if (EndLine[i] != Buf.Data[j]) { goto verify_end; }
                }
                EndLineIdentify();
            }

        verify_end:
            if (Buf.ByteRecived == Buf.Data.Length) { BufLoaded(); }
        }

        public void Clear()
        {
            Response = xRxResponse.Accept;
            Buf.ByteRecived = 0;
        }
    }
}
