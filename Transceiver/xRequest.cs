using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xLib.UI_Propertys;

namespace xLib.Transceiver
{
    public interface IRequestBuilder
    {
        unsafe xRequest Prepare(void* obj, int obj_size);
        xRequest Prepare(byte[] request);
        xRequest Prepare();

    }
    public interface IRequestInfo<TRequestInfo> where TRequestInfo : unmanaged, IRequestInfo
    {
        TRequestInfo Info { get; set; }
    }

    public class xRequest : IRequestControl
    {
        public xRequest() { }

        public xRequest(xRequest request)
        {
            Response = new xResponse(request.Response);
            Builder = request.Builder;
            Data = request.Data;
            Tracer = request.Tracer;
        }

        protected xAction<bool, byte[]> transmitter;
        protected xEvent<xRequest> EventTimeOut;
        protected bool is_transmit = false;
        protected bool complite = false;
        protected int try_count = 1;
        protected int request_time = 100;

        public byte[] Data;

        public xResponse Response { get; set; }
        public xRequestBuilderBase Builder { get; set; }
        public xEvent Tracer;

        public string Name { get; set; }
        public int RequestTime { get { return request_time; } set { if (request_time != value) { request_time = value; } } }
        public int TryCount { get { return try_count; } set { if (try_count != value) { try_count = value; } } }
        public bool Comlite { get { return complite; } set { if (complite != value) { complite = value; } } }
        public bool IsTransmit { get { return is_transmit; } set { if (is_transmit != value) { is_transmit = value; } } }
        public xAction<bool, byte[]> Transmitter { get { return transmitter; } set { transmitter = value; } }
        public ManualResetEvent ManualEventHandler { get; set; }
        public Timer UpdateTimer { get; set; }
        public void Accept() { try_count = 0; complite = true; ManualEventHandler?.Set(); }

        public void TimeOut() { EventTimeOut?.Invoke(this); }

        public bool Transmit()
        {
            if (try_count > 0) { transmitter?.Invoke(Data); is_transmit = true; Builder.Tracer?.Invoke("Transmit: " + Builder.Name + " try: " + try_count); try_count--; }
            else if (!complite) { complite = true; Builder.Tracer?.Invoke("TimeOut: " + Builder.Name); TimeOut(); }
            return is_transmit;
        }
    }
}
