using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace xLib
{
    public delegate void ActionAccessUI(xAction request, object arg);
    public delegate void ActionAccessUI<TRequest>(xAction action, TRequest arg);

    public delegate TResult xAction<out TResult, TRequest>(TRequest request);
    public delegate void xAction<TRequest>(TRequest request);
    public delegate void xAction(object request);

    public delegate TResult xEvent<out TResult, TArgument>(TArgument arg);
    public delegate void xEvent<TArgument>(TArgument arg);
    public delegate void xEvent(object arg);

    public delegate void xEventChangeState<TObject, TState>(TObject obj, TState state);

    public enum EErrors : byte
    {
        ACCEPT = 0,
        ERROR_DATA,
        ERROR_CONTENT_SIZE,
        ERROR_REQUEST,
        ERROR_RESOLUTION,
        UNKNOWN_COMMAND,
        BUSY,
        OUTSIDE,
        ERROR_ACTION,
        ERROR_POSITION
    }
    public struct xContent { public unsafe byte* Obj; public int Size; }

    public interface IRequestInfo { ushort Action { get; set; } ushort Size { get; set; } }
    public struct RequestInfoT : IRequestInfo
    {
        public ushort Action { get; set; }
        public ushort Size { get; set; }
    }

    public interface IResponseAction { ushort Action { get; set; } }
    public struct ResponseInfoT : IResponseAction
    {
        public ushort Action { get; set; }
        public ushort Size { get; set; }
    }

    public interface IResponseError { ushort Error { get; set; } }
    public struct ResponseErrorT: IResponseAction, IResponseError
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
