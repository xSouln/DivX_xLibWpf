﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xLib.UI_Propertys;

namespace xLib.Transceiver
{
    public class xRequestLine : NotifyPropertyChanged
    {
        public class Result
        {
            public List<xRequest> Requests;
            public ERequestState State;
            public long ResponseTime;
        }

        public xAction<string> Tracer;

        private xAction<bool, byte[]> transmitter;
        private int try_count = 1;
        private int response_time_out = 300;
        private int update_period = 1000;
        private bool update_enable;
        private List<xRequest> requests;

        public Func<xAction<bool, byte[]>> RequstTransmitter;

        private CancellationTokenSource cancel_token_source;

        public List<xRequest> Requests
        {
            set { requests = value; }
            get { return requests; }
        }

        public xAction<bool, byte[]> Transmitter
        {
            get => transmitter;
            set { if (value != null) { transmitter = value; } }
        }

        public int TryCount
        {
            get => try_count;
            set { if (value > 0) { try_count = value; } }
        }

        public int ResponseTimeOut
        {
            get => response_time_out;
            set { if (value >= 100) { response_time_out = value; } }
        }

        public bool UpdateEnable
        {
            get { return update_enable; }
            set { update_enable = value; OnPropertyChanged(nameof(UpdateEnable)); }
        }

        public xRequestLine() { Dispose(); }

        public xRequestLine(List<xRequest> requests, xAction<bool, byte[]> transmitter, int try_count, int response_time_out)
        {
            Dispose();
            this.requests = requests;
            this.transmitter = transmitter;
            this.try_count = try_count;
            this.response_time_out = response_time_out;
        }

        public async Task<Result> Transmit()
        {
            Result result = null;

            if (requests != null && requests.Count > 0 && transmitter != null && try_count > 0)
            {
                result = new Result { Requests = requests };
                Stopwatch stop_watch = new Stopwatch();

                if (response_time_out < 100) { response_time_out = 100; }

                stop_watch.Start();
                foreach (xRequest request in requests)
                {
                    var res = await request.TransmitionAsync(transmitter, try_count, response_time_out);

                    if (res != null)
                    {
                        result.State = res.TransmissionState;
                        Tracer?.Invoke("Transmition result: " + res.Name + " " + res.TransmissionState +
                            ", response time: " + res.ResponseTime + "ms"
                            );
                    }
                    else { Tracer?.Invoke("Transmition result: " + "null"); break; }
                }
                stop_watch.Stop();
                result.ResponseTime = stop_watch.ElapsedMilliseconds;
            }
            return result;
        }

        public static async Task<Result> Transmit(List<xRequest> requests, xAction<bool, byte[]> transmitter, int try_count, int response_time_out, xAction<string> tracer)
        {
            Result result = null;

            if (requests != null && requests.Count > 0 && transmitter != null && try_count > 0)
            {
                result = new Result { Requests = requests };
                Stopwatch stop_watch = new Stopwatch();

                if (response_time_out < 100) { response_time_out = 100; }

                stop_watch.Start();
                foreach (xRequest request in requests)
                {
                    var res = await request.TransmitionAsync(transmitter, try_count, response_time_out);

                    if (res != null)
                    {
                        result.State = res.TransmissionState;
                        tracer?.Invoke("Transmition result: " + res.Name + " " + res.TransmissionState +
                            ", response time: " + res.ResponseTime + "ms"
                            );
                    }
                    else { tracer?.Invoke("Transmition result: " + "null"); break; }
                }
                stop_watch.Stop();
                result.ResponseTime = stop_watch.ElapsedMilliseconds;
            }
            return result;
        }

        public void StartUpdate(int period)
        {
            cancel_token_source?.Cancel();
            cancel_token_source = new CancellationTokenSource();

            if (period < 100) { period = 100; }
            update_period = period;
            try
            {
                _ = Task.Factory.StartNew(async () =>
                {
                    Stopwatch stop_watch = new Stopwatch();
                    xAction<bool, byte[]> action_transmitter = transmitter;
                    long delay;
                    UpdateEnable = true;

                    while (true)
                    {
                        delay = update_period;

                        if (!UpdateEnable) { goto end_while; }
                        if (RequstTransmitter != null) { action_transmitter = RequstTransmitter(); }

                        stop_watch.Restart();
                        foreach (xRequest request in requests)
                        {
                            if (!request.IsNotify) { goto end_foreach; }

                            request.Break();
                            stop_watch.Start();
                            var transmition_result = await request.TransmitionAsync(action_transmitter, try_count, response_time_out);
                            stop_watch.Stop();

                            if (transmition_result != null)
                            {
                                Tracer?.Invoke(
                                    "Transmition result: " + transmition_result.Name + " " + transmition_result.TransmissionState +
                                    ", response time: " + transmition_result.ResponseTime + "ms"
                                    );
                            }
                            else { Tracer?.Invoke("Transmition result: " + "null"); break; }
                            end_foreach:;
                        }

                        delay -= stop_watch.ElapsedMilliseconds;
                        end_while: if (delay > 0) { await Task.Delay((int)delay); }
                    }
                },
                cancel_token_source.Token);
            }
            catch { }
        }

        public void Dispose()
        {
            cancel_token_source?.Cancel();
        }
    }
}
