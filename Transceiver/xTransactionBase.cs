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

    public abstract class xTransactionBase : NotifyPropertyChanged
    {
        private string name = "";

        protected xResponseBase response;
        protected xRequest request;

        public xEvent<string> Tracer;

        public bool IsNotify;

        public virtual string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public virtual xResponseBase Response
        {
            get => response;
            set => response = value;
        }

        public virtual xRequest Request
        {
            get => request;
            set => request = value;
        }
    }
}
