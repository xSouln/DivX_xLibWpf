using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.UI_Propertys;

namespace xLib.UI_Propertys
{
    public class UI_LoaderHandler : NotifyPropertyChanged
    {
        public const string BACKGROUND_GREEN = "#FF21662A";
        public const string BACKGROUND_RED = "#FF641818";

        private bool enabling;
        private bool is_enable;
        private bool comliting;
        private bool comlite;
        private bool resolution;
        private bool data_is_selected;

        public UI_Button Button = new UI_Button("Start load", "Stop load", UI_Property.GREEN, UI_Property.RED);

        public bool Enabling
        {
            get { return enabling; }
            set { enabling = value; OnPropertyChanged(nameof(Enabling)); OnPropertyChanged(nameof(Resolution)); }
        }

        public bool IsEnable
        {
            get { return is_enable; }
            set { is_enable = value; OnPropertyChanged(nameof(IsEnable)); OnPropertyChanged(nameof(Resolution)); }
        }

        public bool Comliting
        {
            get { return comliting; }
            set { comliting = value; OnPropertyChanged(nameof(Comliting)); }
        }

        public bool Comlite
        {
            get { return comlite; }
            set { comlite = value; OnPropertyChanged(nameof(Comlite)); }
        }

        public bool DataIsSelected
        {
            get { return data_is_selected; }
            set { data_is_selected = value; Button.IsEnable = data_is_selected; OnPropertyChanged(nameof(DataIsSelected)); }
        }

        public bool Resolution
        {
            get { resolution = !enabling && !is_enable; Button.State = resolution; return resolution; }
            set { resolution = value; OnPropertyChanged(nameof(Resolution)); }
        }
    }
}
