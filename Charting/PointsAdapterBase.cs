using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using xLib;
using xLib.Templates;
using xLib.UI;

namespace xLib.Charting
{
    public class PointsAdapterBase<TPoint> : GraphAdapterBase where TPoint : unmanaged
    {
        public virtual List<TPoint> Points { get; set; }

        public PointsAdapterBase() : base()
        {
            Points = new List<TPoint>();
        }

        public virtual void Add(TPoint point)
        {
            Points.Add(point);
        }

        public virtual unsafe void Add(TPoint* point, int count)
        {
            while (count > 0)
            {
                Points.Add(*point);
                count--;
                point++;
            }
        }

        public override void Clear()
        {
            Points.Clear();
        }
    }
}
