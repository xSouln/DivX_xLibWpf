using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Transceiver
{
    public class xResponseResult
    {
        public xContent Content;

        public virtual object Recieve(xContent content)
        {
            Content = content;
            return this;
        }
    }
}
