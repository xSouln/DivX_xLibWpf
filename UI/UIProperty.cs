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

namespace xLib.UI
{
    public class UIPropertyEvent
    {

    }

    public delegate void UIPropertyEventHandler<TProperty, TEvent>(TProperty property, TEvent evt) where TProperty : UIProperty where TEvent : UIPropertyEvent;

    public interface IValueProperty
    {
        UITemplateAdapter ValueTemplateAdapter { get; set; }
        void ValueUpdate();
        void ValueChanged();
        object GetValue();
        void SetValue(object value);
    }

    public abstract class UIProperty : UINotifyPropertyChanged, ITemplateAdapter
    {
        protected string name = "";

        protected object _value;

        public static Brush RED = (Brush)new BrushConverter().ConvertFrom("#FF641818");
        public static Brush GREEN = (Brush)new BrushConverter().ConvertFrom("#FF21662A");
        public static Brush YELLOW = (Brush)new BrushConverter().ConvertFrom("#FF724C21");
        public static Brush TRANSPARENT = null;

        public object Content;

        protected UITemplateAdapter template_adapter;

        public UIProperty()
        {
            template_adapter = new TemplateContentControl("Value");
        }

        public UIProperty(UITemplateAdapter adapter)
        {
            TemplateAdapter = adapter;
        }

        public virtual UITemplateAdapter TemplateAdapter
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

        protected virtual void ValueUpdate()
        {
            OnPropertyChanged(nameof(Value));
        }

        protected virtual void ValueChanged()
        {

        }

        public virtual object Value
        {
            get => _value;
            set
            {
                if (value != null && _value.GetType() == value.GetType())
                {
                    try
                    {
                        if (Comparer<object>.Default.Compare(_value, value) == 0)
                        {
                            return;
                        }
                    }
                    catch { }
                    goto end;
                }

                return;

            end:;
                _value = value;
                ValueUpdate();
                ValueChanged();
            }
        }

        public virtual void Select()
        {

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

            TemplateAdapter = new TemplateContentControl("Value");
        }

        public UIProperty(UITemplateAdapter adapter)
        {
            _value = default(TValue);

            if (adapter == null)
            {
                TemplateAdapter = new TemplateContentControl(null);
            }
            else
            {
                TemplateAdapter = adapter;
            }
        }

        protected override void ValueChanged()
        {
            EventValueChanged?.Invoke(this, new UIPropertyEvent());
        }

        public new virtual TValue Value
        {
            get => _value != null ? (TValue)_value : default;
            set
            {
                try { if (Comparer<object>.Default.Compare(_value, value) == 0) { return; } }
                catch { }

                _value = value;
                ValueUpdate();
                ValueChanged();
            }
        }
    }

    public interface IRequestTemplateAdapter
    {
        UITemplateAdapter RequestTemplateAdapter { get; set; }
    }

    public interface IRequestProperty : IRequestTemplateAdapter
    {
        void RequestUpdate();
        void RequestChanged();
        object GetRequest();
        void SetRequest(object request);
    }

    public class UIProperty<TValue, TRequest> : UIProperty, IRequestProperty
    {
        protected object _request;

        public UIPropertyEventHandler<UIProperty<TValue, TRequest>, UIPropertyEvent> EventSelection;
        public UIPropertyEventHandler<UIProperty<TValue, TRequest>, UIPropertyEvent> EventValueChanged;
        public UIPropertyEventHandler<UIProperty<TValue, TRequest>, UIPropertyEvent> EventRequestChanged;

        public virtual UITemplateAdapter RequestTemplateAdapter { get; set; }

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
            RequestTemplateAdapter = new TemplateContentControl(nameof(Request));
        }

        public UIProperty(UITemplateAdapter adapter, UITemplateAdapter requestAdapter) : this(adapter)
        {
            RequestTemplateAdapter = requestAdapter;
        }

        public virtual void RequestUpdate()
        {
            OnPropertyChanged(nameof(Request));
        }

        public virtual void RequestChanged()
        {
            EventRequestChanged?.Invoke(this, new UIPropertyEvent());
        }

        protected override void ValueChanged()
        {
            EventValueChanged?.Invoke(this, new UIPropertyEvent());
        }

        public new virtual TValue Value
        {
            get => _value != null ? (TValue)_value : default;
            set
            {
                try
                {
                    if (Comparer<object>.Default.Compare(_value, value) == 0)
                    {
                        return;
                    }
                }
                catch
                {

                }

                _value = value;
                ValueUpdate();
                ValueChanged();
            }
        }

        public object GetRequest()
        {
            return _request;
        }

        public void SetRequest(object request)
        {
            if (request != null && request.GetType() == typeof(TRequest))
            {
                Request = (TRequest)request;
            }
        }

        public virtual TRequest Request
        {
            get => _request != null ? (TRequest)_request : default;
            set
            {
                try
                {
                    if (value != null && _request != null && Comparer<object>.Default.Compare(_request, value) == 0)
                    {
                        return;
                    }
                }
                catch
                {

                }

                _request = value;
                RequestUpdate();
                RequestChanged();
            }
        }
    }
}
