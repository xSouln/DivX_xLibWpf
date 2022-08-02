using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.UI
{
    public class UIListAdapter : UINotifyPropertyChanged
    {
        public ObservableCollection<object> Propertys { get; set; }

        public List<object> Operations { get; set; }

        public List<UIListAdapter> Parameters { get; set; }

        public UIListAdapter SelectedParameter { get; set; }

        public virtual string Name { get; set; }

        public UIListAdapter()
        {

        }
    }

    public interface IUIListAdapter
    {
        UIListAdapter Adapter { get; set; }
    }
}
