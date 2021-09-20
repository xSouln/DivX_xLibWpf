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
        void GetData(List<byte> data);
    }

    public abstract class xRequestBuilderBase : NotifyPropertyChanged
    {
        public const char START_CHARECTER = '#';
        public const int DEFAULT_REQUEST_TIME = 100;

        private string header = "";
        private string name = "";
        public string End = "";
        protected xResponse response;

        public xEvent Tracer;

        public bool IsNotify;

        protected static unsafe void add_data(List<byte> request, void* obj, int obj_size) { if (request != null && obj != null && obj_size > 0) { for (int i = 0; i < obj_size; i++) { request.Add(((byte*)obj)[i]); } } }
        protected static unsafe void add_data(List<byte> request, string str) { if (request != null && str != null) { for (int i = 0; i < str.Length; i++) { request.Add((byte)str[i]); } } }
        protected static unsafe void add_data(List<byte> request, byte[] data) { if (request != null && data != null) { for (int i = 0; i < data.Length; i++) { request.Add(data[i]); } } }

        public virtual string Header { get { return header; } set { header = value; OnPropertyChanged(nameof(Header)); } }
        public virtual string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        public virtual xResponse Response { get { return response; } set { response = value; } }
    }

    public class xRequestBuilder : xRequestBuilderBase
    {
        public virtual unsafe xRequest Prepare(void* obj, int obj_size)
        {
            xRequest RequestPacket = new xRequest { Response = Response, Builder = this };
            List<byte> request_data = new List<byte>();
            add_data(request_data, Header);
            add_data(request_data, obj, obj_size);
            add_data(request_data, End);
            RequestPacket.Data = request_data.ToArray();
            return RequestPacket;
        }

        public virtual xRequest Prepare()
        {
            xRequest RequestPacket = new xRequest { Response = Response, Builder = this };
            List<byte> request_data = new List<byte>();
            add_data(request_data, Header);
            add_data(request_data, End);
            RequestPacket.Data = request_data.ToArray();
            return RequestPacket;
        }

        public virtual xRequest Prepare(string request)
        {
            xRequest RequestPacket = new xRequest { Response = Response, Builder = this };
            List<byte> request_data = new List<byte>();
            add_data(request_data, Header);
            add_data(request_data, request);
            add_data(request_data, End);
            RequestPacket.Data = request_data.ToArray();
            return RequestPacket;
        }

        public virtual xRequest Prepare(byte[] request)
        {
            xRequest RequestPacket = new xRequest { Response = Response, Builder = this };
            List<byte> request_data = new List<byte>();
            add_data(request_data, Header);
            add_data(request_data, request);
            add_data(request_data, End);
            RequestPacket.Data = request_data.ToArray();
            return RequestPacket;
        }
    }

    public class xRequestBuilder<TResponse, TRequestInfo> : xRequestBuilderBase where TRequestInfo : IDataProvider, IRequestInfo where TResponse : xResponse
    {
        public TRequestInfo Info { get; set; }
        public new TResponse Response { get { return (TResponse)response; } set { response = value; } }

        public virtual unsafe xRequest Prepare(void* obj, int obj_size)
        {
            xRequest RequestPacket = new xRequest { Response = Response, Builder = this };
            List<byte> request_data = new List<byte>();

            add_data(request_data, Header);
            Info.Size = (ushort)obj_size;
            Info.GetData(request_data);
            //add_data(request_data, &info, sizeof(TRequestInfo));
            add_data(request_data, obj, obj_size);
            add_data(request_data, End);
            RequestPacket.Data = request_data.ToArray();
            return RequestPacket;
        }

        public virtual unsafe xRequest Prepare()
        {
            xRequest RequestPacket = new xRequest { Response = Response, Builder = this };
            List<byte> request_data = new List<byte>();

            add_data(request_data, Header);
            Info.Size = 0;
            Info.GetData(request_data);
            //add_data(request_data, &info, sizeof(TRequestInfo));
            add_data(request_data, End);
            RequestPacket.Data = request_data.ToArray();
            return RequestPacket;
        }
    }

    public class xRequestBuilder<TResponse, TRequestInfo, TRequest> : xRequestBuilderBase where TRequest : unmanaged where TResponse : xResponse where TRequestInfo : IDataProvider, IRequestInfo
    {
        public TRequestInfo Info { get; set; }
        public new TResponse Response { get { return (TResponse)response; } set { response = value; } }

        public unsafe xRequest Prepare(TRequest request)
        {
            xRequest RequestPacket = new xRequest { Response = Response, Builder = this };
            List<byte> request_data = new List<byte>();
            TRequestInfo info = Info;
            info.Size = (ushort)sizeof(TRequest);

            add_data(request_data, Header);
            Info.Size = (ushort)sizeof(TRequest);
            Info.GetData(request_data);
            //add_data(request_data, &info, sizeof(TRequestInfo));
            add_data(request_data, &request, sizeof(TRequest));
            add_data(request_data, End);
            RequestPacket.Data = request_data.ToArray();
            return RequestPacket;
        }
    }
}
