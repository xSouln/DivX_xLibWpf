using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib
{
    public static class xSupport
    {
        public static ActionAccessUI<object> PointEntryUI;
        public static void ActionThreadUI(xAction action, object arg) { PointEntryUI?.Invoke(action, arg); }
        public static void ActionThreadUI<TRequest>(xAction action, TRequest arg) { PointEntryUI?.Invoke(action, arg); }
        public static void ActionThreadUI(object arg) { if (arg != null) { PointEntryUI?.Invoke((xAction)arg, null); } }
    }
}
