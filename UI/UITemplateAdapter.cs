using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace xLib.UI
{
    public class UITemplateAdapter
    {
        protected FrameworkElement container;

        public virtual FrameworkElementFactory Element
        {
            get;
            set;
        }

        public virtual FrameworkElement Container
        {
            get => container;
            set => container = value;
        }

        public virtual DataTemplate Template
        {
            get;
            set;
        }

        public UITemplateAdapter()
        {

        }

        public UITemplateAdapter(Type element_type)
        {
            Element = new FrameworkElementFactory(element_type);

            Template = new DataTemplate
            {
                VisualTree = Element
            };
        }
    }

    public interface ITemplateAdapter
    {
        UITemplateAdapter TemplateAdapter { get; set; }
    }
}
