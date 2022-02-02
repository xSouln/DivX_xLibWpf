using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace xLib.UI_Propertys
{
    public class UITemplateAdapter
    {
        protected FrameworkElement container;

        public virtual FrameworkElementFactory Element { get; set; }

        public virtual FrameworkElement Container
        {
            get => container;
            set => container = value;
        }

        public virtual DataTemplate Template { get; set; }
    }
}
