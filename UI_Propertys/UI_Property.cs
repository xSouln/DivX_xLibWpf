using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace xLib.UI_Propertys
{
    public interface IUI_PropertyEvents
    {
        void SelectionChanged();
    }

    public interface IUI_PropertyValue<TValue>
    {
        TValue Value { get; set; }
    }
    public interface IUI_PropertyRequest<TRequest>
    {
        TRequest Request { get; set; }
    }

    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class UI_Property : NotifyPropertyChanged, IUI_PropertyValue<object>, IUI_PropertyRequest<object>
    {
        public delegate void xEventChanged<TProperty>(TProperty property);
        public delegate void xEventBackgroundChanged<TProperty>(TProperty property);
        public delegate Brush xBackgroundRule<TProperty>(TProperty property);

        protected string name = "";
        protected string note = "";
        protected bool state = false;
        protected bool is_writable = false;
        protected bool is_enable = false;
        protected int code;

        protected object _value;
        protected object _request;

        public static Brush RED = (Brush)new BrushConverter().ConvertFrom("#FF641818");
        public static Brush GREEN = (Brush)new BrushConverter().ConvertFrom("#FF21662A");
        public static Brush YELLOW = (Brush)new BrushConverter().ConvertFrom("#FF724C21");
        public static Brush TRANSPARENT = null;

        //public Brush BackgroundFalse = (Brush)new BrushConverter().ConvertFrom("#FF641818");
        //public Brush BackgroundTrue = (Brush)new BrushConverter().ConvertFrom("#FF21662A");
        protected Brush background_value;
        protected Brush background_request;

        public object Content;
        public xEventChanged<UI_Property> EventSelectionChanged;
        public xEventChanged<UI_Property> EventChangedValue;
        public xEventChanged<UI_Property> EventChangedRequest;
        public xBackgroundRule<UI_Property> BackgroundValueRule;
        public xBackgroundRule<UI_Property> BackgroundRequestRule;

        protected List<ManualResetEvent> wait_handler = new List<ManualResetEvent>();

        public virtual string Name
        {
            set { name = value; OnPropertyChanged(nameof(Name)); }
            get { return name; }
        }

        public virtual string Note
        {
            set { note = value; OnPropertyChanged(nameof(Note)); }
            get { return note; }
        }

        public bool IsEnable
        {
            set { is_enable = value; OnPropertyChanged(nameof(IsEnable)); }
            get { return is_enable; }
        }

        public bool IsReadOnly
        {
            get { return !is_writable; }
            set { }
        }

        public bool IsWritable
        {
            set { is_writable = value; IsEnable = is_enable | is_writable; OnPropertyChanged(nameof(IsWritable)); OnPropertyChanged(nameof(IsReadOnly)); }
            get { return is_writable; }
        }

        public Brush BackgroundValue
        {
            set { background_value = value; OnPropertyChanged(nameof(BackgroundValue)); }
            get { return background_value; }
        }

        public Brush BackgroundRequest
        {
            set { background_request = value; OnPropertyChanged(nameof(BackgroundRequest)); }
            get { return background_request; }
        }

        public virtual object Value
        {
            get { return _value; }
            set
            {
                if (Comparer<object>.Default.Compare(_value, value) != 0)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                    EventChangedValue?.Invoke(this);
                    BackgroundValue = BackgroundValueRule?.Invoke(this);

                    foreach (ManualResetEvent alement in wait_handler) { alement.Set(); }
                    wait_handler.Clear();
                }
            }
        }

        public virtual object Request
        {
            get { return _request; }
            set
            {
                if (Comparer<object>.Default.Compare(_request, value) != 0)
                {
                    _request = value;
                    OnPropertyChanged(nameof(Request));
                    EventChangedRequest?.Invoke(this);
                    BackgroundRequest = BackgroundRequestRule?.Invoke(this);
                }
            }
        }

        public virtual void BackgroundsUpdate()
        {
            BackgroundRequest = BackgroundRequestRule?.Invoke(this);
            BackgroundValue = BackgroundValueRule?.Invoke(this);
        }

        protected static void timer_callback(object arg)
        {
            ManualResetEvent handler = (ManualResetEvent)arg;
            handler?.Set();
        }

        protected static object wait_value_state(UI_Property property, object state, int time)
        {
            if (Comparer<object>.Default.Compare(property.Value, state) == 0) { return property.Value; }

            ManualResetEvent handler = new ManualResetEvent(false);
            Timer timer = new Timer(timer_callback, handler, time, 0);
            property.wait_handler.Add(handler);
            handler.WaitOne();
            timer?.Dispose();

            return property.Value;
        }

        public virtual async Task<object> WaitValue(object state, int time)
        {
            return await Task.Run(() => wait_value_state(this, state, time));
        }

        public virtual void SelectionChanged() { EventSelectionChanged?.Invoke(this); }
        public static void SelectionChanged(IUI_PropertyEvents events) { events.SelectionChanged(); }
    }

    public class UI_Property<TValue> : UI_Property, IUI_PropertyEvents, IUI_PropertyValue<TValue>, IUI_PropertyRequest<TValue>
    {
        public UI_Property()
        {
            _value = default(TValue);
            _request = default(TValue);
        }

        public new xEventChanged<UI_Property<TValue>> EventSelectionChanged;
        public new xEventChanged<UI_Property<TValue>> EventChangedValue;
        public new xEventChanged<UI_Property<TValue>> EventChangedRequest;
        public new xBackgroundRule<UI_Property<TValue>> BackgroundValueRule;
        public new xBackgroundRule<UI_Property<TValue>> BackgroundRequestRule;

        public new TValue Value
        {
            set
            {
                if (Comparer<TValue>.Default.Compare((TValue)_value, value) != 0)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                    EventChangedValue?.Invoke(this);
                    BackgroundValue = BackgroundValueRule?.Invoke(this);

                    foreach (ManualResetEvent alement in wait_handler) { alement.Set(); }
                    wait_handler.Clear();
                }
            }
            get { return (TValue)_value; }
        }

        public new TValue Request
        {
            set
            {
                if (Comparer<TValue>.Default.Compare((TValue)_request, value) != 0)
                {
                    _request = value;
                    OnPropertyChanged(nameof(Request));
                    EventChangedRequest?.Invoke(this);
                    BackgroundRequest = BackgroundRequestRule?.Invoke(this);
                }
            }
            get { return (TValue)_request; }
        }

        public override void BackgroundsUpdate()
        {
            BackgroundRequest = BackgroundRequestRule?.Invoke(this);
            BackgroundValue = BackgroundValueRule?.Invoke(this);
        }

        public async Task<TValue> WaitValue(TValue state, int time)
        {
            return (TValue)await Task.Run(() => wait_value_state(this, state, time));
        }

        public override void SelectionChanged() { EventSelectionChanged?.Invoke(this); }
    }

    public class UI_Property<TValue, TRequest> : UI_Property, IUI_PropertyEvents, IUI_PropertyValue<TValue>, IUI_PropertyRequest<TRequest> where TValue : IComparable where TRequest : IComparable
    {
        public UI_Property()
        {
            _value = default(TValue);
            _request = default(TRequest);
        }

        public new xEventChanged<UI_Property<TValue, TRequest>> EventSelectionChanged;
        public new xEventChanged<UI_Property<TValue, TRequest>> EventChangedValue;
        public new xEventChanged<UI_Property<TValue, TRequest>> EventChangedRequest;
        public new xBackgroundRule<UI_Property<TValue, TRequest>> BackgroundValueRule;
        public new xBackgroundRule<UI_Property<TValue, TRequest>> BackgroundRequestRule;

        public new TValue Value
        {
            set
            {
                if (Comparer<TValue>.Default.Compare((TValue)_value, value) != 0)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                    EventChangedValue?.Invoke(this);
                    BackgroundValue = BackgroundValueRule?.Invoke(this);

                    foreach (ManualResetEvent alement in wait_handler) { alement.Set(); }
                    wait_handler.Clear();
                }
            }
            get { return (TValue)_value; }
        }

        public new TRequest Request
        {
            set
            {
                if (Comparer<TRequest>.Default.Compare((TRequest)_request, value) != 0)
                {
                    _request = value;
                    OnPropertyChanged(nameof(Request));
                    EventChangedRequest?.Invoke(this);
                    BackgroundRequest = BackgroundRequestRule?.Invoke(this);
                }
            }
            get { return (TRequest)_request; }
        }

        public override void BackgroundsUpdate()
        {
            BackgroundRequest = BackgroundRequestRule?.Invoke(this);
            BackgroundValue = BackgroundValueRule?.Invoke(this);
        }

        public async Task<TValue> WaitValue(TValue state, int time)
        {
            return (TValue)await Task.Run(() => wait_value_state(this, state, time));
        }

        public override void SelectionChanged() { EventSelectionChanged?.Invoke(this); }
    }
}
