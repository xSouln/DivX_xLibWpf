using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLib.UI;

namespace xLib.Templates
{
    public class TemplateContentControl : UITemplateAdapter
    {
        public TemplateContentControl() : this("Value")
        {

        }

        public TemplateContentControl(string TextPropertyPath)
        {
            Element = new FrameworkElementFactory(typeof(ContentControl));

            if (TextPropertyPath != null)
            {
                Element.SetBinding(ContentControl.ContentProperty, new Binding { Path = new PropertyPath(TextPropertyPath) });
            }

            Template = new DataTemplate { VisualTree = Element };
        }
    }
}
