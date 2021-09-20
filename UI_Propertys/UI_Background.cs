using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using xLib.UI_Propertys;

namespace xLib.UI_Propertys
{
    public class UI_Background : NotifyPropertyChanged
    {
        private Brush background;

        public Brush Background
        {
            get { return background; }
        }

        public UI_Background(object key)
        {
            background = (Brush)(new BrushConverter().ConvertFrom(key));
            OnPropertyChanged(nameof(Background));
        }

        public void Set(object key)
        {
            try { background = (Brush)(new BrushConverter().ConvertFrom(key)); OnPropertyChanged(nameof(Background)); }
            catch { }
        }
    }
}
