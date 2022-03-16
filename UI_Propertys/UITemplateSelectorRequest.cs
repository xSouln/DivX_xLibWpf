using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace xLib.UI_Propertys
{
    public class UITemplateSelectorRequest : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            UIProperty property = item as UIProperty;

            if (property != null)
            {
                if (property is IRequestProperty request)
                {
                    return request.RequestTemplateAdapter?.Template;
                }
            }

            return new DataTemplate
            {
                VisualTree = new FrameworkElementFactory(typeof(ContentControl))
            };
        }
    }
}
