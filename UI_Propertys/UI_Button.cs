using System.Windows.Media;
using xLib.UI_Propertys;

namespace xLib.UI_Propertys
{
    public class UI_Button : NotifyPropertyChanged
    {
        public string NameOn = "true";
        public string NameOff = "false";
        public string NameIgnore = "";

        public Brush BackgroundOn = (Brush)new BrushConverter().ConvertFrom("#FF21662A");
        public Brush BackgroundOff = (Brush)new BrushConverter().ConvertFrom("#FF641818");
        //private Brush background_ignore = (Brush)(new BrushConverter().ConvertFrom("#FF641818"));

        private string name = "";
        private bool state = false;
        private bool is_enable = false;
        private bool is_ignore = false;
        private Brush background;

        public UI_Button() { }

        public UI_Button(string name_on, string name_off, Brush background_on, Brush background_off)
        {
            if (name_on != null) { NameOn = name_on; }
            if (name_off != null) { NameOff = name_off; }
            if (background_on != null) { BackgroundOn = background_on; };
            if (background_off != null) { BackgroundOff = background_off; }
        }

        public string Name
        {
            set { name = value; OnPropertyChanged(nameof(Name)); }
            get { return name; }
        }

        public bool State
        {
            set
            {
                state = value;
                OnPropertyChanged(nameof(State));

                if (is_ignore) { Background = null; return; }

                if (state) { Name = NameOn; Background = BackgroundOn; }
                else { Name = NameOff; Background = BackgroundOff; }
            }
            get { return state; }
        }

        public bool IsEnable
        {
            set { is_enable = value; OnPropertyChanged(nameof(IsEnable)); }
            get { return is_enable; }
        }

        public bool IsIgnore
        {
            set { is_ignore = value; if (is_ignore) { Background = null; } OnPropertyChanged(nameof(IsEnable)); }
            get { return is_ignore; }
        }

        public Brush Background
        {
            set { background = value; OnPropertyChanged(nameof(Background)); }
            get { return background; }
        }
    }
}
