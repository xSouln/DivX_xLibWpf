using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using xLib.UI;

namespace xLib.Common
{
    public class xCommunicationControl : UINotifyPropertyChanged
    {
        private Timer timer;
        private bool is_update;
        private bool is_available;
        private Brush background_true = (Brush)new BrushConverter().ConvertFrom("#FF21662A");
        private Brush background_false = (Brush)new BrushConverter().ConvertFrom("#FF641818");
        private Brush background;

        public event xEvent<bool> EventStateChanged;

        public Brush Background
        {
            get { return background; }
            set { background = value; OnPropertyChanged(nameof(Background)); }
        }

        public bool IsAvailable
        {
            get { return is_available; }
            set
            {
                if (value && background != background_true) { Background = background_true; }
                else if(!value && background != background_false) { Background = background_false; }

                if (is_available != value)
                {
                    is_available = value;
                    EventStateChanged?.Invoke(is_available);
                    OnPropertyChanged(nameof(IsAvailable));
                }
            }
        }

        public void StartControl(int update_period)
        {
            if (update_period < 100) { update_period = 100; }
            timer?.Dispose();
            timer = new Timer(update_state, null, 0, update_period);
        }

        public xCommunicationControl() { } // StartControl(2000);
        public xCommunicationControl(int update_period) { StartControl(update_period); }

        public void Dispose() { timer?.Dispose(); }

        private void update_state(object arg)
        {
            if (!is_update && IsAvailable) { IsAvailable = false; }
            is_update = false;
        }

        public void Update() { is_update = true; IsAvailable = true; }
    }
}
