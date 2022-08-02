using System.Windows;
using System.Windows.Media;
using xLib.UI;

namespace xLib.UI
{
    public class UIButton : UINotifyPropertyChanged
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

        public UIButton() { }

        public UIButton(string name_on, string name_off, Brush background_on, Brush background_off)
        {
            if (name_on != null) { NameOn = name_on; }
            if (name_off != null) { NameOff = name_off; }
            if (background_on != null) { BackgroundOn = background_on; };
            if (background_off != null) { BackgroundOff = background_off; }
        }

        public string Name
        {
            set { name = value; OnPropertyChanged(nameof(Name)); }
            get
            {
                if (state && name != NameOn) { name = NameOn; }
                else if (!state && name != NameOff) { name = NameOff; }
                return name;
            }
        }

        public Brush Background
        {
            set { background = value; OnPropertyChanged(nameof(Background)); }
            get
            {
                if (state && background != BackgroundOn) { background = BackgroundOn; }
                else if (!state && background != BackgroundOff) { background = BackgroundOff; }
                return background;
            }
        }

        public bool State
        {
            set
            {
                state = value;
                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Background));
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

        private void Click(object sender, RoutedEventArgs e)
        {
            State ^= state;
        }
    }
}
