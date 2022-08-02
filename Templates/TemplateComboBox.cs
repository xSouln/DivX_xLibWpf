using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using xLib.UI;

namespace xLib.Templates
{
    public class TemplateComboBox : UITemplateAdapter
    {
        public TemplateComboBox()
        {
            var grid = new FrameworkElementFactory(typeof(Grid));
            var free = new FrameworkElementFactory(typeof(FrameworkElement));
            Element = new FrameworkElementFactory(typeof(ComboBox));

            grid.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 0));
            free.SetValue(UIElement.VisibilityProperty, Visibility.Hidden);
            free.SetValue(FrameworkElement.WidthProperty, 200.0);

            Element.SetValue(Control.ForegroundProperty, UIProperty.GetBrush("#FF000000"));
            Element.SetValue(Control.BackgroundProperty, null);
            Element.SetValue(Control.BorderBrushProperty, null);

            Element.SetValue(Control.PaddingProperty, new Thickness(0, 0, 0, 0));
            Element.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 0));

            Element.SetValue(FrameworkElement.HeightProperty, double.NaN);

            grid.AppendChild(free);
            grid.AppendChild(Element);

            this.Template = new DataTemplate { VisualTree = grid };
        }
    }
}
