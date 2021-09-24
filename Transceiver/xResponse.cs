using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.UI_Propertys;

namespace xLib.Transceiver
{
    public interface IResponseControl
    {
        unsafe bool Identification(xContent content);
        unsafe void Receive(xContent content);
    }
    public interface IResponseInfo<TResponseInfo> where TResponseInfo : unmanaged, IResponseAction
    {
        TResponseInfo Info { get; set; }
    }

    public interface IContentSeter
    {
        object SetContent(xContent content);
    }

    public unsafe class xResponsePacket<TContent> : IContentSeter where TContent : unmanaged
    {
        public TContent *Obj;
        public xContent Content;

        public xResponsePacket() { }
        public xResponsePacket(xContent content) { Content = content; Obj = (TContent*)content.Obj; }
        public object SetContent(xContent content) { Content = content; Obj = (TContent*)content.Obj; return this; }
    }

    public class xResponse : NotifyPropertyChanged, IResponseControl
    {
        public const char START_CHARECTER = '#';
        public class ParseCallback { public xContent Content; }

        public unsafe delegate ParseCallback DParseRule<TResponse>(TResponse response, xContent content);
        public unsafe delegate bool DReceiver<TResponse, TContent>(TResponse response, TContent packet) where TContent : IContentSeter, new();

        public xResponse() { }
        public xResponse(xResponse response)
        {
            name = response.name;
            header = response.header;
            ParseRule = response.ParseRule;
            EventReceive = response.EventReceive;
        }

        protected string name = "";
        protected string header = "";

        public DParseRule<xResponse> ParseRule;
        public DReceiver<xResponse, xResponsePacket<byte>> EventReceive;
        public xEvent Tracer;

        public virtual string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(nameof(Name)); }
        }
        public virtual string Header
        {
            get { return header; }
            set { header = value; OnPropertyChanged(nameof(Header)); }
        }

        public virtual unsafe bool Identification(xContent content)
        {
            if (ParseRule != null)
            {
                ParseCallback callback = ParseRule(this, content);
                if (callback != null) { Receive(callback.Content); return true; }
            }
            return false;
        }

        public unsafe virtual void Receive(xContent content) { Tracer?.Invoke("Receive" + name); EventReceive?.Invoke(this, new xResponsePacket<byte>(content)); }
    }

    public class xResponse<TContent> : xResponse where TContent : IContentSeter, new()
    {
        public new DParseRule<xResponse<TContent>> ParseRule;
        public new DReceiver<xResponse<TContent>, TContent> EventReceive;

        public override unsafe bool Identification(xContent content)
        {
            if (ParseRule != null)
            {
                ParseCallback callback = ParseRule(this, content);
                if (callback != null) { return (bool)(EventReceive?.Invoke(this, (TContent)new TContent().SetContent(callback.Content))); }
            }
            return false;
        }

        public unsafe override void Receive(xContent content) { Tracer?.Invoke("Receive" + name); EventReceive?.Invoke(this, (TContent)new TContent().SetContent(content)); }
    }

    public class xResponse<TContent, TResponseInfo> : xResponse, IResponseAction where TContent : IContentSeter, new() where TResponseInfo : IResponseAction
    {
        public TResponseInfo Info;
        public ushort Action { get { return Info.Action; } set { } }

        public new DParseRule<xResponse<TContent, TResponseInfo>> ParseRule;
        public new DReceiver<xResponse<TContent, TResponseInfo>, TContent> EventReceive;

        public override unsafe bool Identification(xContent content)
        {
            if (ParseRule != null)
            {
                ParseCallback callback = ParseRule(this, content);
                if (callback != null) { return (bool)(EventReceive?.Invoke(this, (TContent)new TContent().SetContent(callback.Content))); }
            }
            return false;
        }

        public override unsafe void Receive(xContent content) { Tracer?.Invoke("Receive" + name); EventReceive?.Invoke(this, (TContent)new TContent().SetContent(content)); }
    }
}
