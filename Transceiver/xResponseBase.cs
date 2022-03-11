using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.UI_Propertys;

namespace xLib.Transceiver
{
    public abstract class xResponseBase : INotifyPropertyChanged
    {
        public delegate bool ReceiveHandler<TResponse, TResult>(TResponse response, TResult packet);

        protected string name = "";
        protected string header = "";
        protected xResponseResult result;

        public object Context;
        public bool IsAccepted;
        public xEvent<string> Tracer;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual xTransactionBase Parent { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public xResponseBase(List<xResponseBase> responses)
        {
            responses?.Add(this);
        }

        public xResponseBase(xResponseBase response)
        {
            name = response.name;
            header = response.header;
            Tracer = response.Tracer;
        }

        public virtual xResponseResult Result
        {
            get => result;
            set => result = value;
        }

        public virtual string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public virtual string Header
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        public virtual bool Identification(xContent content)
        {
            return false;
        }

        public virtual void Receive(xContent content)
        {

        }
    }
}
