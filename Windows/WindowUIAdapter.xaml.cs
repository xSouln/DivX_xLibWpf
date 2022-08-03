using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using xLib.UI;

namespace xLib.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowUIAdapter.xaml
    /// </summary>
    public partial class WindowUIAdapter : Window
    {
        public WindowUIAdapter()
        {
            InitializeComponent();
        }

        public WindowUIAdapter(UIViewModel view_model) : this()
        {
            //Grid.SetColumn(view_model.UIModel, 0);
            //Grid.SetRow(view_model.UIModel, 0);

            //GridControl.Children.Add(view_model.UIModel);

            //view_model.Update();
        }
    }
}
