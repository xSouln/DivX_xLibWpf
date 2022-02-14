using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xLib.Transceiver
{
    public enum ERequestState
    {
        Free,
        Prepare,
        IsTransmit,
        Complite,
        TimeOut,
        ErrorTransmite,
        ErrorTransmiteAction,
        Busy,
        AddError
    }

    public interface ITransmitter
    {
        xAction<bool, byte[]> Transmitter { get; }
    }

    public class xRequestHandler
    {
        protected List<xRequest> requests = new List<xRequest>();
        protected AutoResetEvent read_write_synchronize = new AutoResetEvent(true);
        protected Semaphore queue_size;
        protected Thread thread;

        public xRequestHandler(int line_size)
        {
            if (line_size < 1) { line_size = 10; }
            queue_size = new Semaphore(line_size, line_size);
        }

        public xRequestHandler()
        {
            queue_size = new Semaphore(10, 10);
            //thread = new Thread(thread_handler);
            //thread.Start();
        }

        protected virtual void requests_update()
        {
            int i = 0;
            while (i < requests.Count)
            {
                switch (requests[i].TransmissionState)
                {
                    case ERequestState.Complite: requests[i].Accept(); requests.RemoveAt(i); break;
                    case ERequestState.IsTransmit: i++; break;
                    case ERequestState.Prepare: i++; break;
                    default: requests.RemoveAt(i); break;
                }
            }
        }

        public virtual bool Add(xRequest request)
        {
            try
            {
                read_write_synchronize.WaitOne();

                requests_update();
                if (requests.Count >= 20) { return false; }
                requests.Add(request);
            }
            finally
            {
                read_write_synchronize.Set();
            }
            return true;
        }

        public virtual void Remove(xRequest request)
        {
            try
            {
                read_write_synchronize.WaitOne();

                for (int i = 0; i < requests.Count; i++)
                {
                    if (requests[i] == request) { requests.RemoveAt(i); }
                    else { i++; }
                }
            }
            finally
            {
                read_write_synchronize.Set();
            }
        }

        public bool Identification(xContent content)
        {
            bool result = false;
            try
            {
                read_write_synchronize.WaitOne();

                requests_update();
                for (int i = 0; i < requests.Count; i++)
                {
                    result = requests[i].Response.Identification(content);
                    if (result)
                    {
                        requests[i].Accept();
                        requests.RemoveAt(i);
                        break;
                    }
                }
            }
            finally
            {
                read_write_synchronize.Set();
            }
            return result;
        }

        public bool Accept(xBuilderBase builder)
        {
            bool result = false;
            try
            {
                read_write_synchronize.WaitOne();

                for (int i = 0; i < requests.Count; i++)
                {
                    result = requests[i].Builder == builder;
                    if (result)
                    {
                        requests[i].Accept();
                        requests.RemoveAt(i);
                        break;
                    }
                }
            }
            finally
            {
                read_write_synchronize.Set();
            }
            return result;
        }
    }

    public class xRequest
    {
        protected xAction<bool, byte[]> transmitter;
        protected int try_count = 1;
        protected int try_number = 0;
        protected int response_time_out = 100;
        protected long response_time = 0;
        protected Timer timer_transmiter;
        protected volatile ERequestState transmission_state;

        protected AutoResetEvent transmition_synchronize = new AutoResetEvent(true);

        protected volatile bool is_transmit_action;
        protected volatile bool is_accept;
        protected xResponse response;

        public xRequestHandler Handler;
        public bool IsNotify = true;

        public byte[] Data;

        public xEvent<string> Tracer;
        public xEvent<xRequest> EventTimeOut;

        public xBuilderBase Builder { get; set; }

        public string Name => Builder?.Name;

        public int ResponseTimeOut => response_time_out;

        public long ResponseTime => response_time;

        public int TryCount { get => try_count; set { if (try_count > 0) { try_count = value; } } }

        public int TryNumber => try_number;

        public xAction<bool, byte[]> Transmitter { get => transmitter; set => transmitter = value; }

        public virtual xResponse Response { get => response; set => response = value; }

        public ERequestState TransmissionState => transmission_state;


        public void Accept()
        {
            try
            {
                transmition_synchronize.WaitOne();

                if (transmission_state == ERequestState.IsTransmit)
                {
                    transmission_state = ERequestState.Complite;
                }
            }
            finally { transmition_synchronize.Set(); }
        }

        protected static void transmit_action(xRequest request)
        {
            try
            {
                request.transmition_synchronize.WaitOne();

                if (request.transmission_state == ERequestState.IsTransmit)
                {
                    if (request.try_number < request.try_count)
                    {
                        if (request.transmitter == null || !request.transmitter(request.Data))
                        {
                            request.transmission_state = ERequestState.ErrorTransmite;
                            request.Builder.Tracer?.Invoke("Transmit: " + request.Builder.Name + " " + request.transmission_state);
                            return;
                        }
                        request.try_number++;
                        request.Builder.Tracer?.Invoke("Transmit: " + request.Builder.Name + " try: " + request.try_number);
                    }
                    else
                    {
                        request.transmission_state = ERequestState.TimeOut;
                        request.Builder.Tracer?.Invoke("TimeOut: " + request.Builder.Name);
                        request.EventTimeOut?.Invoke(request);
                    }
                }
            }
            finally { request.transmition_synchronize.Set(); }
        }

        protected virtual xRequest transmition()
        {
            try
            {
                transmition_synchronize.WaitOne();

                if (transmission_state != ERequestState.Free) { return this; }
                transmission_state = ERequestState.Prepare;

                if (!(bool)Handler?.Add(this))
                {
                    transmission_state = ERequestState.Busy;
                    return this;
                }
                else
                {
                    transmission_state = ERequestState.IsTransmit;
                }
            }
            finally
            {
                transmition_synchronize.Set();
            }

            try_number = 0;
            response_time = 0;

            Stopwatch time_transmition = new Stopwatch();
            Stopwatch time_transmit_action = new Stopwatch();

            time_transmition.Start();
            do
            {
                transmit_action(this);
                time_transmit_action.Restart();
                while (transmission_state == ERequestState.IsTransmit && time_transmit_action.ElapsedMilliseconds < response_time_out)
                {
                    Thread.Sleep(1);
                }
            }
            while (transmission_state == ERequestState.IsTransmit);

            time_transmition.Stop();
            time_transmit_action.Stop();
            response_time = time_transmition.ElapsedMilliseconds;
            return this;
        }

        protected virtual async Task<xRequest> transmition_async()
        {
            try
            {
                transmition_synchronize.WaitOne();

                if (transmission_state != ERequestState.Free) { return this; }
                transmission_state = ERequestState.Prepare;

                if (!(bool)Handler?.Add(this))
                {
                    transmission_state = ERequestState.Busy;
                    return this;
                }
                else
                {
                    transmission_state = ERequestState.IsTransmit;
                }
            }
            finally
            {
                transmition_synchronize.Set();
            }

            try_number = 0;
            response_time = 0;

            Stopwatch time_transmition = new Stopwatch();
            Stopwatch time_transmit_action = new Stopwatch();

            time_transmition.Start();
            do
            {
                transmit_action(this);
                time_transmit_action.Restart();
                while (transmission_state == ERequestState.IsTransmit && time_transmit_action.ElapsedMilliseconds < response_time_out)
                {
                    await Task.Delay(1);
                }
            }
            while (transmission_state == ERequestState.IsTransmit);

            time_transmition.Stop();
            time_transmit_action.Stop();
            response_time = time_transmition.ElapsedMilliseconds;
            return this;
        }

        public virtual void Break()
        {
            transmition_synchronize.WaitOne();
            transmission_state = ERequestState.Free;
            transmition_synchronize.Set();

            Handler?.Remove(this);
        }

        public virtual xRequest Transmition(xAction<bool, byte[]> transmitter, int try_count, int response_time_out)
        {
            if (transmitter == null || try_count <= 0 || response_time_out <= 0 || transmission_state != ERequestState.Free) { return null; }
            this.transmitter = transmitter;
            this.try_count = try_count;
            this.response_time_out = response_time_out;
            this.try_number = 0;
            this.response.Result = null;

            var result = transmition();
            return result;
        }

        public static TRequest Transmition<TRequest>(TRequest request, xAction<bool, byte[]> transmitter, int try_count, int response_time_out) where TRequest : xRequest
        {
            if (transmitter == null || try_count <= 0 || response_time_out <= 0 || request.transmission_state != ERequestState.Free) { return null; }

            request.transmitter = transmitter;
            request.try_count = try_count;
            request.response_time_out = response_time_out;
            request.try_number = 0;
            request.response.Result = null;

            var result = request.transmition();
            return (TRequest)result;
        }

        public virtual async Task<xRequest> TransmitionAsync(xAction<bool, byte[]> transmitter, int try_count, int response_time_out)
        {
            if (transmitter == null || try_count <= 0 || response_time_out <= 0 || transmission_state != ERequestState.Free) { return null; }
            this.transmitter = transmitter;
            this.try_count = try_count;
            this.response_time_out = response_time_out;
            this.try_number = 0;
            this.response.Result = null;

            var result = await Task.Run(() => transmition());
            //var result = await Task.Run(() => transmition_async());
            return result;
        }

        public static async Task<TRequest> TransmitionAsync<TRequest>(TRequest request, xAction<bool, byte[]> transmitter, int try_count, int response_time, CancellationTokenSource cancellation) where TRequest : xRequest
        {
            if (transmitter == null || try_count <= 0 || response_time <= 0 || request.transmission_state != ERequestState.Free) { return null; }

            if (cancellation == null) { cancellation = new CancellationTokenSource(); }

            request.transmitter = transmitter;
            request.try_count = try_count;
            request.response_time_out = response_time;
            request.try_number = 0;
            request.response.Result = null;

            var result = await Task.Run(() => request.transmition(), cancellation.Token);
            return (TRequest)result;
        }

        public static async Task<TRequest> TransmitionAsync<TRequest>(TRequest request, xAction<bool, byte[]> transmitter, int try_count, int response_time) where TRequest : xRequest
        {
            if (transmitter == null || try_count <= 0 || response_time <= 0 || request.transmission_state != ERequestState.Free) { return null; }

            request.transmitter = transmitter;
            request.try_count = try_count;
            request.response_time_out = response_time;
            request.try_number = 0;
            request.response.Result = null;

            var result = await Task.Run(() => request.transmition());
            return (TRequest)result;
        }
    }

    public class xRequest<TResult> : xRequest where TResult : xResponseResult, new()
    {
        public new xResponse<TResult> Response { get => (xResponse<TResult>)response; set => response = value; }

        public TResult Result { get => ((xResponse<TResult>)response).Result; }

        public new xRequest<TResult> Transmition(xAction<bool, byte[]> transmitter, int try_count, int response_time)
        {
            if (transmitter == null || try_count <= 0 || response_time_out <= 0 || transmission_state != ERequestState.Free) { return null; }
            this.transmitter = transmitter;
            this.try_count = try_count;
            this.response_time_out = response_time;
            this.try_number = 0;
            this.response.Result = null;

            var result = transmition();
            return (xRequest<TResult>)result;
        }

        public new async Task<xRequest<TResult>> TransmitionAsync(xAction<bool, byte[]> transmitter, int try_count, int response_time)
        {
            if (transmitter == null || try_count <= 0 || response_time <= 0 || transmission_state != ERequestState.Free) { return null; }
            this.transmitter = transmitter;
            this.try_count = try_count;
            this.response_time_out = response_time;
            this.try_number = 0;
            this.response.Result = null;

            var result = await Task.Run(() => transmition());
            //var result = await Task.Run(() => transmition_async());
            return (xRequest<TResult>)result;
        }
    }
}