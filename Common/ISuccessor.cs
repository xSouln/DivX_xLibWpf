using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Common
{
    public interface ISuccessor<T>
    {
        T Parent { get; set; }
    }

    public interface ISuccessor
    {

    }
}
