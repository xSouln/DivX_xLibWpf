using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xLib.Sources
{
    public class xList<TObject>
    {
        private AutoResetEvent thred_control = new AutoResetEvent(true);
        public List<TObject> Values = new List<TObject>();

        public xEvent<int> EventCountChanged;

        public void Add(TObject arg)
        {
            thred_control.WaitOne();
            Values.Add(arg);
            EventCountChanged(Values.Count);
            thred_control.Set();
        }

        public void Remove(TObject arg)
        {
            thred_control.WaitOne();
            int count = Values.Count;
            for (int i = 0; i < Values.Count; i++)
            {
                if ((object)Values[i] == (object)arg) { Values.RemoveAt(i); }
                else { i++; }
            }
            if (count != Values.Count) { EventCountChanged(Values.Count); }
            thred_control.Set();
        }

        public void CriticalSectionEnter()
        {
            thred_control.WaitOne();
        }

        public void CriticalSectionExit()
        {
            thred_control.WaitOne();
        }
    }
}
