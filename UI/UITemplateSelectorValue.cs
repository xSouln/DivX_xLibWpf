using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace xLib.UI
{
    public class UITemplateSelectorValue : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            UIProperty property = item as UIProperty;

            if (property != null)
            {
                /*
                if (property is not IRequestProperty)
                {
                    return property.TemplateAdapter?.Template;
                }
                */
                return property.TemplateAdapter?.Template;
            }

            //var Element = new FrameworkElementFactory(typeof(ContentControl));
            //Element.SetBinding(ContentControl.ContentProperty, new Binding { Path = new PropertyPath("Value") });

            return new DataTemplate
            {
                VisualTree = new FrameworkElementFactory(typeof(ContentControl))
            };
        }
    }
}
