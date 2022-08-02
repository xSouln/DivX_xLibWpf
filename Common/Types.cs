using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using xLib.Transceiver;

namespace xLib
{
    public delegate void ActionAccessUI(xAction request, object arg);
    public delegate void ActionAccessUI<TRequest>(xAction action, TRequest arg);

    public delegate TResult xAction<TResult, TRequest>(TRequest request);
    public delegate void xAction<TRequest>(TRequest request);
    public delegate void xAction(object request);

    public delegate void UIAction<TContext, TRequest>(TContext context, TRequest request);

    public delegate TResult xEvent<out TResult, TArgument>(TArgument arg);
    public delegate void xEvent<TArgument>(TArgument arg);
    public delegate void xEvent(object arg);

    public delegate void xEventChangeState<TObject, TState>(TObject obj, TState state);

    public interface IWindow
    {
        Window Window { get; set; }

        void Open();

        void Close();
    }

    public struct xContent
    {
        public unsafe byte* Data;
        public int DataSize;
    }

    public interface IRequestInfo
    {
        ushort Action { get; set; }
        ushort Size { get; set; }
        //ushort PacketId { get; set; }
    }

    public struct RequestInfoT : IRequestInfo, IDataProvider
    {
        public ushort Action { get; set; }
        public ushort Size { get; set; }
        //public ushort PacketId { get; set; }

        public unsafe int GetSize() => sizeof(RequestInfoT);
        public void SetSize(int size) { Size = (ushort)size; }

        public void GetData(List<byte> data)
        {
            unsafe
            {
                RequestInfoT total = this;
                byte* ptr = (byte*)&total;
                for (int i = 0; i < sizeof(ResponseInfoT); i++) { data.Add(ptr[i]); }
            }
        }
    }

    public interface IResponseInfo
    {
        ushort Action { get; set; }
        ushort Size { get; set; }
        //ushort PacketId { get; set; }
    }

    public interface IResponseAction<TAction> { TAction Action { get; set; } }

    public struct ResponseInfoT : IResponseAction<ushort>, IDataProvider
    {
        public ushort Action { get; set; }
        public ushort Size { get; set; }
        //ushort PacketId { get; set; }
        public unsafe int GetSize() => sizeof(RequestInfoT);
        public void SetSize(int size) { Size = (ushort)size; }

        public void GetData(List<byte> data)
        {
            unsafe
            {
                ResponseInfoT total = this;
                byte* ptr = (byte*)&total;
                for (int i = 0; i < sizeof(ResponseInfoT); i++) { data.Add(ptr[i]); }
            }
        }
    }

    public interface IResponseError { ushort Error { get; set; } }
    public struct ResponseErrorT: IResponseAction<ushort>, IResponseError
    {
        public ushort Action { get; set; }
        public ushort Error { get; set; }
    }

    public struct ResponsePrefixT
    {
        public byte Start;
        public byte Ch1;
        public byte Ch2;
        public byte Ch3;
        public byte Ch4;
        public byte End;
    }

    public struct RequestPrefixT
    {
        public byte Start;
        public byte Ch1;
        public byte Ch2;
        public byte Ch3;
        public byte Ch4;
        public byte End;
    }

    public struct ResponseT
    {
        public ResponsePrefixT Prefix;
        public ResponseInfoT Info;
    }

    public struct RequestT
    {
        public RequestPrefixT Prefix;
        public RequestInfoT Info;
    }
}
