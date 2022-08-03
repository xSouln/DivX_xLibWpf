using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace xLib.UI
{
    public abstract class UIViewModel : UINotifyPropertyChanged
    {
        public UIElement UIModel { get; set; }
        public string Name { get; set; }

        public virtual void Update()
        {

        }

        public virtual object GetParent()
        {
            return null;
        }

        public virtual void Dispose()
        {

        }
    }
}
