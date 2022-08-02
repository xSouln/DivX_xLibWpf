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
    public class TemplateButton : UITemplateAdapter
    {
        public TemplateButton() : this("Value")
        {

        }

        public TemplateButton(string TextPropertyPath) : base()
        {
            var grid = new FrameworkElementFactory(typeof(Grid));
            var free = new FrameworkElementFactory(typeof(FrameworkElement));
            Element = new FrameworkElementFactory(typeof(Button));

            grid.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 0));
            grid.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);

            free.SetValue(UIElement.VisibilityProperty, Visibility.Hidden);
            free.SetValue(FrameworkElement.WidthProperty, 200.0);
            /*
            free.SetBinding(FrameworkElement.HeightProperty, new Binding
            {
                RelativeSource = new RelativeSource
                {
                    Mode = RelativeSourceMode.TemplatedParent
                },
                Path = new PropertyPath("Height")
            });
            */
            Element.SetValue(Control.FontSizeProperty, 18.0);
            Element.SetValue(Control.ForegroundProperty, UIProperty.GetBrush("#FFDEC316"));
            Element.SetValue(Control.BackgroundProperty, UIProperty.GetBrush("#FF494849"));
            Element.SetValue(Control.BorderBrushProperty, UIProperty.GetBrush("#FF834545"));
            Element.SetValue(Control.BorderBrushProperty, UIProperty.GetBrush("#FF834545"));

            //Element.SetValue(Control.PaddingProperty, new Thickness(-2));
            Element.SetValue(FrameworkElement.MarginProperty, new Thickness(-1));

            //Element.SetValue(FrameworkElement.HeightProperty, double.NaN);
            //Element.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);

            Element.SetResourceReference(Control.TemplateProperty, "ButtonTemplate1");

            if (TextPropertyPath != null)
            {
                Element.SetBinding(ContentControl.ContentProperty, new Binding { Path = new PropertyPath(TextPropertyPath) });
            }

            grid.AppendChild(free);
            grid.AppendChild(Element);

            Template = new DataTemplate { VisualTree = grid };
        }
    }
}
