using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xLib.Transceiver
{
    public interface IRequestControl
    {
        bool Transmit();
        bool Comlite { get; set; }
        bool IsTransmit { get; set; }
        unsafe void Accept();
        void TimeOut();
        int RequestTime { get; set; }
        int TryCount { get; set; }
        xAction<bool, byte[]> Transmitter { get; set; }
        xResponse Response { get; set; }
        xRequestBuilderBase Builder { get; set; }
        Timer UpdateTimer { get; set; }
        ManualResetEvent ManualEventHandler { get; set; }
    }

    public class xRequestControl
    {
        //public AutoResetEvent WaitHandler = new AutoResetEvent(true);

        public Thread ThreadControl;

        private List<IRequestControl> _requests = new List<IRequestControl>();
        private List<IRequestControl> _receives = new List<IRequestControl>();

        private List<List<IRequestControl>> _receives_async = new List<List<IRequestControl>>();
        //else { WaitHandler.WaitOne(); Requests.RemoveAt(0); WaitHandler.Set(); }
        private static void requests_control(object arg)
        {
            List<IRequestControl> requests = (List<IRequestControl>)arg;
            IRequestControl request;
            Timer timer = null;

            while (true)
            {
                if (requests.Count > 0)
                {
                    request = requests[0];
                    if (request.Comlite) { requests.RemoveAt(0); }
                    else if (!request.IsTransmit) { request.IsTransmit = true; timer = new Timer(transmit, request, 0, request.RequestTime); }
                }
                Thread.Sleep(1);
            }
        }

        private static void requests_control_async(List<IRequestControl> requests)
        {
            IRequestControl request = null;

            while (requests.Count > 0)
            {
                request = requests[0];

                if (request.Comlite) { request.UpdateTimer?.Dispose(); requests.RemoveAt(0); goto end_while; }

                if (!request.IsTransmit)
                {
                    request.IsTransmit = true;
                    request.ManualEventHandler = new ManualResetEvent(false);
                    request.UpdateTimer = new Timer(transmit, request, 0, request.RequestTime);
                }

                request.ManualEventHandler.WaitOne();
            end_while:;
            }
        }

        private static void transmit(object arg)
        {
            IRequestControl request = (IRequestControl)arg;
            if (request != null && !request.Comlite) { request.Transmit(); }
            else { request.ManualEventHandler?.Set(); request.UpdateTimer?.Dispose(); }
        }

        public void Add(IRequestControl request, xAction<bool, byte[]> transmitter, int try_count)
        {
            if (request != null && try_count > 0 && transmitter != null)
            {
                request.RequestTime = xRequestBuilderBase.DEFAULT_REQUEST_TIME;
                request.TryCount = try_count;
                request.Transmitter = transmitter;

                _receives.Add(request);
                _requests.Add(request);
            }
        }

        public async void AddAsync(IRequestControl request, xAction<bool, byte[]> transmitter, int try_count)
        {
            if (request != null && transmitter != null && try_count > 0)
            {
                request.RequestTime = xRequestBuilderBase.DEFAULT_REQUEST_TIME;
                request.TryCount = try_count;
                request.Transmitter = transmitter;

                List<IRequestControl> receives = new List<IRequestControl> { request };
                List<IRequestControl> requests = new List<IRequestControl> { request };

                _receives_async.Add(receives);
                await Task.Run(() => requests_control_async(requests));
            }
        }

        public async void AddAsync(IRequestControl request)
        {
            if (request != null && request.Transmitter != null && request.TryCount > 0)
            {
                List<IRequestControl> receives = new List<IRequestControl> { request };
                List<IRequestControl> requests = new List<IRequestControl> { request };

                _receives_async.Add(receives);
                await Task.Run(() => requests_control_async(requests));
            }
        }

        public async void AddAsync(List<IRequestControl> requests, xAction<bool, byte[]> transmitter, int try_count)
        {
            if (requests != null && requests.Count > 0 && transmitter != null && try_count > 0)
            {
                List<IRequestControl> receives = new List<IRequestControl>();
                foreach (IRequestControl request in requests)
                {
                    request.RequestTime = xRequestBuilderBase.DEFAULT_REQUEST_TIME;
                    request.TryCount = try_count;
                    request.Transmitter = transmitter;
                    receives.Add(request);
                }
                _receives_async.Add(receives);
                await Task.Run(() => requests_control_async(requests));
            }
        }

        public async void AddAsync(List<IRequestControl> requests)
        {
            if (requests != null)
            {
                List<IRequestControl> receives = new List<IRequestControl>();
                foreach (IRequestControl request in requests)
                {
                    if (request.TryCount > 0 && request.Transmitter != null) { receives.Add(request); }
                    else { return; }
                }
                _receives_async.Add(receives);
                await Task.Run(() => requests_control_async(requests));
            }
        }

        private void requests_update()
        {
            int i = 0;
            lock (_receives_async)
            {
                while (i < _receives_async.Count)
                {
                    int j = 0;
                    List<IRequestControl> requests = _receives_async[i];
                    while (j < requests.Count)
                    {
                        if (requests[j].Comlite) { requests.RemoveAt(j); }
                        else { j++; }
                    }

                    if (requests.Count > 0) { i++; }
                    else { _receives_async.RemoveAt(i); }
                }
            }

            i = 0;
            lock (_receives)
            {
                while (i < _receives.Count)
                {
                    if (_receives[i].Comlite) { _receives.RemoveAt(i); }
                    else { i++; }
                }
            }
        }

        public unsafe bool Identification(xContent content)
        {
            bool accept = false;
            requests_update();

            foreach (List<IRequestControl> requests in _receives_async)
            {
                IRequestControl request = requests[0];
                if (request.Response.Identification(content))
                {
                    request.Accept();
                    accept = true;
                    goto end;
                }
            }

            if (_receives.Count > 0)
            {
                IRequestControl request = _receives[0];
                if (request.Response.Identification(content))
                {
                    request.Accept();
                    accept = true;
                    goto end;
                }
            }

        end:;
            return accept;
        }

        public unsafe bool Accept(xRequestBuilder builder)
        {
            requests_update();
            foreach (List<IRequestControl> requests in _receives_async)
            {
                IRequestControl request = requests[0];
                if (request.Builder == builder) { request.Accept(); return true; }
            }

            if (_receives.Count > 0)
            {
                IRequestControl request = _receives[0];
                if (request.Builder == builder) { request.Accept(); return true; }
            }
            return false;
        }

        public void Start() { ThreadControl = new Thread(requests_control); ThreadControl.Start(_requests); }

        public xRequestControl() { Start(); }

        public void Dispose() { ThreadControl?.Abort(); }
    }
}
