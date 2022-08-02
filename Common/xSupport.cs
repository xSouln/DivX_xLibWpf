using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace xLib.Common
{
    public class xSupport
    {
        //public static ActionAccessUI<object> PointEntryUI;
        public static DispatcherObject Context;
        public static void ActionThreadUI(xAction action, object arg) { RequestThreadUI(action, arg); }
        public static void ActionThreadUI<TRequest>(xAction action, TRequest arg) { RequestThreadUI(action, arg); }

        public static void ActionThreadUI(object arg)
        {
            if (arg != null)
            {
                RequestThreadUI((xAction)arg, null);
            }
        }

        private static void RequestThreadUI(xAction request, object arg)
        {
            if (Context != null)
            {
                try
                {
                    Context.Dispatcher.Invoke(() =>
                    {
                        request?.Invoke(arg);
                    });
                }
                catch { }
            }
        }
    }
}
