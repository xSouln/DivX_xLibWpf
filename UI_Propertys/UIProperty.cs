using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using xLib.Templates;

namespace xLib.UI_Propertys
{
    public interface IUI_PropertyEvents
    {
        void Select();
    }

    public interface IUI_PropertyValue<TValue>
    {
        TValue Value { get; set; }
    }

    public interface IUI_PropertyRequest<TRequest>
    {
        TRequest Request { get; set; }
    }

    public interface IUI_Code
    {
        int Code { get; set; }
    }

    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class UIPropertyEvent
    {

    }

    public delegate void UIPropertyEventHandler<TProperty, TEvent>(TProperty property, TEvent evt) where TProperty : UIProperty where TEvent : UIPropertyEvent;

    public abstract class UIProperty : NotifyPropertyChanged
    {
        protected string name = "";
        protected object code;

        protected object _value;

        public static Brush RED = (Brush)new BrushConverter().ConvertFrom("#FF641818");
        public static Brush GREEN = (Brush)new BrushConverter().ConvertFrom("#FF21662A");
        public static Brush YELLOW = (Brush)new BrushConverter().ConvertFrom("#FF724C21");
        public static Brush TRANSPARENT = null;

        public object Content;

        protected UITemplateAdapter template_adapter;
        /*
        public object RequestTemplate
        {
            get => null;
            set => OnPropertyChanged(nameof(RequestTemplate));
        }
        */
        public UIProperty()
        {
            template_adapter = new TemplateContentControl();
        }

        public UIProperty(UITemplateAdapter adapter)
        {
            TemplateAdapter = adapter;
        }

        public UITemplateAdapter TemplateAdapter
        {
            get => template_adapter;
            set
            {
                template_adapter = value;
                OnPropertyChanged(nameof(TemplateAdapter));
            }
        }

        public static Brush GetBrush(string request)
        {
            Brush brush = null;
            try { brush = (Brush)new BrushConverter().ConvertFrom(request); }
            catch { }
            return brush;
        }

        public virtual string Name
        {
            set { name = value; OnPropertyChanged(nameof(Name)); }
            get => name;
        }

        public virtual object Code
        {
            set { code = value; OnPropertyChanged(nameof(Code)); }
            get => code;
        }

        protected virtual void UpdateValue()
        {

        }

        public virtual object GetValue() => _value;

        public virtual void Select()
        {

        }

        public virtual void SetValue(object request)
        {
            if (_value == null)
            {
                _value = request;
                UpdateValue();
                return;
            }

            if (request != null && _value.GetType() == request.GetType())
            {
                try
                {
                    if (Comparer<object>.Default.Compare(_value, request) == 0) { return; }
                }
                catch { }
                _value = request;
                UpdateValue();
            }
        }

        protected static async Task<object> wait_value_state_async(UIProperty property, object state, int time)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (Comparer<object>.Default.Compare(property._value, state) != 0 && stopwatch.ElapsedMilliseconds < time)
            {
                await Task.Delay(1);
                //Thread.Sleep(1);
            }
            stopwatch.Stop();
            return property._value;
        }

        protected static TState wait_value_state<TState>(UIProperty property, TState state, int time)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (Comparer<object>.Default.Compare(property._value, state) != 0 && stopwatch.ElapsedMilliseconds < time)
            {
                Thread.Sleep(1);
            }
            stopwatch.Stop();
            return (TState)property._value;
        }

        public virtual TState WaitValue<TState>(TState state, int time)
        {
            return wait_value_state(this, state, time);
        }

        public virtual async Task<TState> WaitValueAsync<TState>(TState state, int time)
        {
            return (TState)await Task.Run(() => wait_value_state_async(this, state, time));
        }
    }

    public class UIProperty<TValue> : UIProperty
    {
        public UIPropertyEventHandler<UIProperty<TValue>, UIPropertyEvent> EventSelection;
        public UIPropertyEventHandler<UIProperty<TValue>, UIPropertyEvent> EventValueChanged;

        public UIProperty() : base()
        {
            _value = default(TValue);
        }

        public UIProperty(UITemplateAdapter adapter) : base()
        {
            _value = default(TValue);
            TemplateAdapter = adapter;
        }

        protected override void UpdateValue()
        {
            OnPropertyChanged(nameof(Value));
            EventValueChanged?.Invoke(this, new UIPropertyEvent());
        }

        public virtual TValue Value
        {
            get => _value != null ? (TValue)_value : default;
            set
            {
                try { if (Comparer<object>.Default.Compare(_value, value) == 0) { return; } }
                catch { }

                _value = value;
                UpdateValue();
            }
        }
    }

    public class UIProperty<TValue, TRequest> : UIProperty
    {
        protected object _request;

        public UIPropertyEventHandler<UIProperty<TValue, TRequest>, UIPropertyEvent> EventSelection;
        public UIPropertyEventHandler<UIProperty<TValue, TRequest>, UIPropertyEvent> EventValueChanged;
        public UIPropertyEventHandler<UIProperty<TValue, TRequest>, UIPropertyEvent> EventRequestChanged;

        public UIProperty() : base()
        {
            _value = default(TValue);
            _request = default(TRequest);
        }

        public UIProperty(UITemplateAdapter adapter) : base()
        {
            _value = default(TValue);
            _request = default(TRequest);
            TemplateAdapter = adapter;
        }

        protected virtual void UpdateRequest()
        {
            OnPropertyChanged(nameof(Request));
            EventRequestChanged?.Invoke(this, new UIPropertyEvent());
        }

        protected override void UpdateValue()
        {
            OnPropertyChanged(nameof(Value));
            EventValueChanged?.Invoke(this, new UIPropertyEvent());
        }

        public virtual TValue Value
        {
            get => _value != null ? (TValue)_value : default;
            set
            {
                try { if (Comparer<object>.Default.Compare(_value, value) == 0) { return; } }
                catch { }

                _value = value;
                UpdateValue();
            }
        }

        public virtual TRequest Request
        {
            get => _request != null ? (TRequest)_request : default;
            set
            {
                try { if (Comparer<object>.Default.Compare(_request, value) == 0) { return; } }
                catch { }

                _request = value;
                UpdateRequest();
            }
        }

        public override void Select()
        {

        }
    }
}
