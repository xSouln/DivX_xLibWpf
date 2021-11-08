using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.UI_Propertys;

namespace xLib.Transceiver
{
    public interface IDataProvider
    {
        int GetSize();
        void SetSize(int size);
        void GetData(List<byte> data);
    }

    public abstract class xBuilderBase : NotifyPropertyChanged
    {
        public const char START_CHARECTER = '#';
        public const int DEFAULT_REQUEST_TIME = 100;

        private string header = "";
        private string name = "";
        public string End = "";
        protected xResponse response;

        public xEvent<string> Tracer;
        public xRequestHandler RequestHandler;

        public bool IsNotify;

        protected static unsafe void add_data(List<byte> request, void* obj, int obj_size) { if (request != null && obj != null && obj_size > 0) { for (int i = 0; i < obj_size; i++) { request.Add(((byte*)obj)[i]); } } }
        protected static unsafe void add_data(List<byte> request, string str) { if (request != null && str != null) { for (int i = 0; i < str.Length; i++) { request.Add((byte)str[i]); } } }
        protected static unsafe void add_data(List<byte> request, byte[] data) { if (request != null && data != null) { for (int i = 0; i < data.Length; i++) { request.Add(data[i]); } } }

        public virtual string Header { get { return header; } set { header = value; OnPropertyChanged(nameof(Header)); } }
        public virtual string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        public virtual xResponse Response { get { return response; } set { response = value; } }
    }

    public class xBuilder<TResult> : xBuilderBase where TResult : xResponseResult, new()
    {
        public xBuilder(List<xResponse> responses, xRequestHandler handler, string name) : base()
        {
            response = new xResponse<TResult>(responses) { Name = name, ParseRule = ParseRule };
            Name = name;
            RequestHandler = handler;
        }

        public new xResponse<TResult> Response { get { return (xResponse<TResult>)response; } set { response = value; } }

        public virtual unsafe xRequest<TResult> Prepare(void* obj, int obj_size)
        {
            xRequest<TResult> RequestPacket = new xRequest<TResult>()//(Response)
            {
                Response = new xResponse<TResult>(Response),
                Builder = this,
                Tracer = Tracer,
                Handler = RequestHandler
            };
            List<byte> request_data = new List<byte>();

            add_data(request_data, Header);
            add_data(request_data, obj, obj_size);
            add_data(request_data, End);

            RequestPacket.Data = request_data.ToArray();
            return RequestPacket;
        }

        public virtual xRequest<TResult> Prepare()
        {
            xRequest<TResult> RequestPacket = new xRequest<TResult>()//(Response)
            {
                Response = new xResponse<TResult>(Response),
                Builder = this,
                Tracer = Tracer,
                Handler = RequestHandler
            };
            List<byte> request_data = new List<byte>();

            add_data(request_data, Header);
            add_data(request_data, End);
            RequestPacket.Data = request_data.ToArray();

            return RequestPacket;
        }

        public virtual xRequest<TResult> Prepare(string request)
        {
            xRequest<TResult> RequestPacket = new xRequest<TResult>()//(Response)
            {
                Response = new xResponse<TResult>(Response),
                Builder = this,
                Tracer = Tracer,
                Handler = RequestHandler
            };
            List<byte> request_data = new List<byte>();

            add_data(request_data, Header);
            add_data(request_data, request);
            add_data(request_data, End);

            RequestPacket.Data = request_data.ToArray();
            return RequestPacket;
        }

        protected virtual unsafe object ParseRule(xResponse response, xContent content)
        {
            if (xConverter.Compare(response.Header, content))
            {
                content.Size = content.Size - response.Header.Length;
                content.Obj = content.Obj + response.Header.Length;
                return content;
            }
            return null;
        }
    }

    public class xBuilder<TResult, TRequest> : xBuilderBase where TResult : xResponseResult, new() where TRequest : unmanaged
    {
        public xBuilder(List<xResponse> responses, xRequestHandler handler) : base()
        {
            response = new xResponse<TResult>(responses);
            RequestHandler = handler;
        }

        public new xResponse<TResult> Response { get { return (xResponse<TResult>)response; } set { response = value; } }

        public unsafe xRequest<TResult> Prepare(TRequest request)
        {
            xRequest<TResult> RequestPacket = new xRequest<TResult>()//(Response)
            {
                Response = new xResponse<TResult>(Response),
                Builder = this,
                Tracer = Tracer,
                Handler = RequestHandler
            };
            List<byte> request_data = new List<byte>();

            add_data(request_data, Header);
            add_data(request_data, &request, sizeof(TRequest));
            add_data(request_data, End);

            RequestPacket.Data = request_data.ToArray();
            return RequestPacket;
        }

        protected virtual unsafe object ParseRule(xResponse response, xContent content)
        {
            if (xConverter.Compare(response.Header, content))
            {
                content.Size = content.Size - response.Header.Length;
                content.Obj = content.Obj + response.Header.Length;
                return content;
            }
            return null;
        }
    }

    public class xBuilderPacket<TResult, TAction> : xBuilderBase where TResult : xResponseResult, new() where TAction : unmanaged
    {
        public xBuilderPacket(List<xResponse> responses, xRequestHandler handler, TAction action)
        {
            response = new xResponse<TResult, TAction>(responses, action) { Name = "" + action, ParseRule = ParseRule };
            Name = "" + action;
            Action = action;
            RequestHandler = handler;
        }

        public new xResponse<TResult, TAction> Response { get { return (xResponse<TResult, TAction>)response; } set { response = value; } }

        public readonly TAction Action;

        public virtual unsafe xRequest<TResult> Prepare()
        {
            xRequest<TResult> RequestPacket = new xRequest<TResult>()//(Response)
            {
                Response = new xResponse<TResult, TAction>(null, Response),
                Builder = this,
                Tracer = Tracer,
                Handler = RequestHandler
            };
            RequestInfoT info = new RequestInfoT { Action = (ushort)(object)Action };
            List<byte> request_data = new List<byte>();

            add_data(request_data, Header);
            add_data(request_data, &info, sizeof(ResponseInfoT));
            add_data(request_data, End);

            RequestPacket.Data = request_data.ToArray();
            return RequestPacket;
        }

        protected virtual unsafe object ParseRule(xResponse response, xContent content)
        {
            ResponseT *packet = (ResponseT*)content.Obj;
            if (xConverter.Compare(response.Header, &packet->Prefix, sizeof(ResponsePrefixT))
                && response is IResponseAction<TAction> value
                && (ushort)(object)value.Action == packet->Info.Action)
            {
                content.Obj += sizeof(ResponseT);
                content.Size -= sizeof(ResponseT);
                return content;
            }
            return null;
        }
    }

    public class xBuilderPacket<TResult, TAction, TRequest> : xBuilderBase where TResult : xResponseResult, new() where TRequest : unmanaged
    {
        public xBuilderPacket(List<xResponse> responses, xRequestHandler handler, TAction action)
        {
            response = new xResponse<TResult, TAction>(responses, action) { Name = "" + action, ParseRule = ParseRule };
            Name = "" + action;
            Action = action;
            RequestHandler = handler;
        }

        public new xResponse<TResult, TAction> Response { get { return (xResponse<TResult, TAction>)response; } set { response = value; } }

        public readonly TAction Action;

        public virtual unsafe xRequest<TResult> Prepare(TRequest request)
        {
            xRequest<TResult> RequestPacket = new xRequest<TResult>()//(Response)
            {
                Response = new xResponse<TResult, TAction>(null, Response),
                Builder = this,
                Tracer = Tracer,
                Handler = RequestHandler
            };

            RequestInfoT info = new RequestInfoT { Action = (ushort)(object)Action, Size = (ushort)sizeof(TRequest) };
            List<byte> request_data = new List<byte>();

            add_data(request_data, Header);
            add_data(request_data, &info, sizeof(ResponseInfoT));
            add_data(request_data, &request, sizeof(TRequest));
            add_data(request_data, End);

            RequestPacket.Data = request_data.ToArray();
            return RequestPacket;
        }

        protected virtual unsafe object ParseRule(xResponse response, xContent content)
        {
            ResponseT* packet = (ResponseT*)content.Obj;
            if (xConverter.Compare(response.Header, &packet->Prefix, sizeof(ResponsePrefixT))
                && response is IResponseAction<TAction> value
                && (ushort)(object)value.Action == packet->Info.Action)
            {
                content.Obj += sizeof(ResponseT);
                content.Size -= sizeof(ResponseT);
                return content;
            }
            return null;
        }
    }
}
