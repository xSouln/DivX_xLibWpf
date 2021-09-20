using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace xLib.UI_Propertys
{
    public interface IUI_FPropertyEvents
    {
        void SelectionChanged();
    }

    public interface IUI_FPropertyValue<TValue>
    {
        TValue Value { get; set; }
    }
    public interface IUI_FPropertyRequest<TRequest>
    {
        TRequest Request { get; set; }
    }

    public interface IUI_FProperty
    {
        void Update();
    }

    public interface IUI_Text
    {
        string Set { set; }
        string Get { get; }
        EventHandler TextChanged { set; }
    }

    public abstract class UI_FormEvents
    {
        //void TextChanged(object sender, EventArgs e);
    }

    public abstract class UI_FProperty : IUI_PropertyValue<object>
    {
        public delegate void xSelectionChanged<TProperty>(TProperty property);
        public delegate TProperty xParseRull<out TProperty>(string arg);
        //public delegate string xSetterRull<TProperty>(TProperty arg);

        protected string _name = "";
        protected string _note = "";
        protected object _value;

        protected bool update_name = false;
        protected bool update_value = false;

        protected bool state = false;
        protected bool is_writable = false;

        public object Content;
        public ActionAccessUI<object> PointEntryUI;
        public xParseRull<string> ParseRull;

        public virtual string Name
        {
            set { _name = value; update_name = true; }
            get { return _name; }
        }

        public virtual object Value
        {
            set { _value = value; }
            get { return _value; }
        }

        protected virtual void TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class UI_FProperty<TValue> : UI_FProperty, IUI_PropertyValue<TValue> where TValue: IFormattable
    {
        public UI_FProperty() { _value = default(TValue); }
        public new xParseRull<TValue> ParseRull;

        public virtual new TValue Value
        {
            set
            {
                _value = value;
                if (PointEntryUI != null && Content != null) { PointEntryUI((object arg) => { ((IUI_Text)Content).Set = _value.ToString(); }, null); }
                else { update_value = true; }
            }
            get { return (TValue)_value; }
        }
    }
}
