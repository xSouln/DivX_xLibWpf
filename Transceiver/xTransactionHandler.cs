using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xLib.Transceiver
{
    public enum ETransactionState
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

    public class xTransactionHandler
    {
        protected List<xRequest> transactions = new List<xRequest>();
        protected AutoResetEvent read_write_synchronize = new AutoResetEvent(true);
        protected Semaphore queue_size;
        protected Thread thread;

        public xTransactionHandler(int line_size)
        {
            if (line_size < 1) { line_size = 10; }
            queue_size = new Semaphore(line_size, line_size);
        }

        public xTransactionHandler()
        {
            queue_size = new Semaphore(10, 10);
            //thread = new Thread(thread_handler);
            //thread.Start();
        }

        protected virtual void requests_update()
        {
            int i = 0;
            while (i < transactions.Count)
            {
                switch (transactions[i].TransmissionState)
                {
                    case ETransactionState.Complite:
                        transactions[i].Accept();
                        transactions.RemoveAt(i);
                        break;

                    case ETransactionState.IsTransmit:
                        i++;
                        break;

                    case ETransactionState.Prepare:
                        i++;
                        break;

                    default: transactions.RemoveAt(i);
                        break;
                }
            }
        }

        public virtual bool Add(xRequest request)
        {
            try
            {
                read_write_synchronize.WaitOne();

                requests_update();
                if (transactions.Count >= 20) { return false; }
                transactions.Add(request);
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

                for (int i = 0; i < this.transactions.Count; i++)
                {
                    if (this.transactions[i] == request) { this.transactions.RemoveAt(i); }
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
                for (int i = 0; i < transactions.Count; i++)
                {
                    result = transactions[i].Response.Identification(content);
                    if (result)
                    {
                        transactions[i].Accept();
                        transactions.RemoveAt(i);
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

        /*
        public bool Accept(xTransactionBase builder)
        {
            bool result = false;
            try
            {
                read_write_synchronize.WaitOne();

                for (int i = 0; i < request.Count; i++)
                {
                    result = request[i].Builder == builder;
                    if (result)
                    {
                        request[i].Accept();
                        request.RemoveAt(i);
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
        */
    }
}
