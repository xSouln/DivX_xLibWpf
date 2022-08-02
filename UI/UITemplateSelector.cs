using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace xLib.UI
{
    public class UITemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && item is ITemplateAdapter request)
            {
                return request.TemplateAdapter?.Template;
            }

            return new DataTemplate
            {
                VisualTree = new FrameworkElementFactory(typeof(ContentControl))
            };

        }
    }
}
