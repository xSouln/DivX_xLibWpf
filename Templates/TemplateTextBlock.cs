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
    public class TemplateTextBlock : UITemplateAdapter
    {
        protected void Init(string TextPropertyPath)
        {
            var grid = new FrameworkElementFactory(typeof(Grid));
            var free = new FrameworkElementFactory(typeof(FrameworkElement));
            var Element = new FrameworkElementFactory(typeof(TextBlock));

            grid.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 0));
            free.SetValue(UIElement.VisibilityProperty, Visibility.Hidden);
            free.SetValue(FrameworkElement.WidthProperty, 200.0);

            Element.SetValue(Control.BackgroundProperty, null);
            Element.SetValue(Control.BorderBrushProperty, null);

            Element.SetValue(Control.PaddingProperty, new Thickness(0, 0, 0, 0));
            Element.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 0));

            Element.SetValue(FrameworkElement.HeightProperty, double.NaN);
            Element.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);

            if (TextPropertyPath != null)
            {
                Element.SetBinding(TextBlock.TextProperty, new Binding { Path = new PropertyPath(TextPropertyPath) });
            }

            //Element.SetBinding(TextBlock.BackgroundProperty, new Binding { Path = new PropertyPath("Background") });

            grid.AppendChild(free);
            grid.AppendChild(Element);

            this.Template = new DataTemplate { VisualTree = grid };
        }

        public TemplateTextBlock() : this("Value")
        {

        }

        public TemplateTextBlock(string TextPropertyPath) : base()
        {
            var grid = new FrameworkElementFactory(typeof(Grid));
            var free = new FrameworkElementFactory(typeof(FrameworkElement));
            Element = new FrameworkElementFactory(typeof(TextBlock));

            grid.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 0));
            free.SetValue(UIElement.VisibilityProperty, Visibility.Hidden);
            free.SetValue(FrameworkElement.WidthProperty, 200.0);

            Element.SetValue(Control.BackgroundProperty, null);
            Element.SetValue(Control.BorderBrushProperty, null);

            Element.SetValue(Control.PaddingProperty, new Thickness(0, 0, 0, 0));
            Element.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 0));

            Element.SetValue(FrameworkElement.HeightProperty, double.NaN);
            Element.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);

            if (TextPropertyPath != null)
            {
                Element.SetBinding(TextBlock.TextProperty, new Binding { Path = new PropertyPath(TextPropertyPath) });
            }

            grid.AppendChild(free);
            grid.AppendChild(Element);

            this.Template = new DataTemplate { VisualTree = grid };
        }
    }
}
