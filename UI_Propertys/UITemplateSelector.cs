using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace xLib.UI_Propertys
{
    public class UITemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item is UIProperty property ? property.TemplateAdapter?.Template : null;
        }
    }
}
