using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLib.UI_Propertys;

namespace xLib.Templates
{
    public class TemplateContentControl : UITemplateAdapter
    {
        public TemplateContentControl()
        {
            Element = new FrameworkElementFactory(typeof(ContentControl));

            Element.SetBinding(ContentControl.ContentProperty, new Binding { Path = new PropertyPath("Value") });

            this.Template = new DataTemplate { VisualTree = Element };
        }

        public TemplateContentControl(string TextPropertyPath) : base()
        {
            Element.SetBinding(TextBlock.TextProperty, new Binding { Path = new PropertyPath(TextPropertyPath) });
        }
    }
}
