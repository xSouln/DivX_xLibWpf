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

    public interface IContentSeter
    {
        object SetContent(xContent content);
    }

    public class xResponseResult : IContentSeter
    {
        public xContent Content;

        public virtual object SetContent(xContent content)
        {
            Content = content;
            return this;
        }
    }

    public abstract class xResponse : NotifyPropertyChanged
    {
        public const char START_CHARECTER = '#';

        public delegate object DParseRule<TResponse>(TResponse response, xContent content);
        public delegate bool DReceiver<TResponse, TResult>(TResponse response, TResult packet);

        protected string name = "";
        protected string header = "";
        protected xResponseResult result;

        public object Context;
        public bool IsAccepted;
        public DParseRule<xResponse> ParseRule;
        public DReceiver<xResponse, xResponseResult> EventReceive;
        //protected object event_receive;
        public xEvent<string> Tracer;

        public xResponse(List<xResponse> responses) { responses?.Add(this); }

        public xResponse(xResponse response)
        {
            name = response.name;
            header = response.header;
            EventReceive = response.EventReceive;
            ParseRule = response.ParseRule;
            Tracer = response.Tracer;
        }

        public xResponse(List<xResponse> responses, xResponse response) : this(response)
        {
            responses?.Add(this);
        }

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

        public virtual xResponseResult Result
        {
            get { return result; }
            set { result = value; }
        }

        public virtual bool Identification(xContent content)
        {
            if (ParseRule != null)
            {
                object parse_content = ParseRule(this, content);
                if (parse_content != null)
                {
                    result = (xResponseResult)new xResponseResult().SetContent((xContent)parse_content);
                    IsAccepted = (bool)(EventReceive?.Invoke(this, result));
                    return IsAccepted;
                }
            }
            return false;
        }

        public virtual void Receive(xContent content)
        {
            Tracer?.Invoke("Receive" + name);
            IsAccepted = true;
            result = (xResponseResult)new xResponseResult().SetContent(content);
            EventReceive?.Invoke(this, result);
        }

        public virtual void Receive(object context, xContent content)
        {
            Context = context;
            Receive(content);
        }
    }

    public class xResponse<TResult> : xResponse, IResponseControl where TResult : xResponseResult, new()
    {
        public new DParseRule<xResponse<TResult>> ParseRule;
        public new DReceiver<xResponse<TResult>, TResult> EventReceive;

        public xResponse(List<xResponse> responses) : base(responses) { }

        public xResponse(xResponse<TResult> response) : base(response) { ParseRule = response.ParseRule; EventReceive = response.EventReceive; }

        public xResponse(List<xResponse> responses, xResponse<TResult> response) : base(responses, response) { ParseRule = response.ParseRule; EventReceive = response.EventReceive; }

        public new virtual TResult Result
        {
            get { return (TResult)result; }
            set { result = value; }
        }

        public override bool Identification(xContent content)
        {
            if (ParseRule != null)
            {
                object parse_content = ParseRule(this, content);
                if (parse_content != null)
                {
                    result = (TResult)new TResult().SetContent((xContent)parse_content);
                    if (EventReceive != null)
                    {
                        IsAccepted = EventReceive(this, (TResult)result);
                        return IsAccepted;
                    }
                    return true;
                }
            }
            return false;
        }

        public override void Receive(xContent content)
        {
            Tracer?.Invoke("Receive" + name);
            result = (TResult)new TResult().SetContent(content);
            EventReceive?.Invoke(this, (TResult)result);
        }
    }

    public class xResponse<TResult, TAction> : xResponse<TResult>, IResponseAction<TAction> where TResult : xResponseResult, new()
    {
        public new DReceiver<xResponse<TResult, TAction>, TResult> EventReceive;

        public xResponse(List<xResponse> responses, TAction action) : base(responses) { name = "" + action; Action = action; }

        public xResponse(List<xResponse> responses, xResponse<TResult, TAction> response) : base(responses, response) { Action = response.Action; EventReceive = response.EventReceive; }

        public TAction Action { get; set; }

        public override bool Identification(xContent content)
        {
            if (ParseRule != null && EventReceive != null)
            {
                object parse_content = ParseRule(this, content);
                if (parse_content != null)
                {
                    result = (TResult)new TResult().SetContent((xContent)parse_content);
                    IsAccepted = EventReceive(this, (TResult)result);
                    return IsAccepted;
                }
            }
            return false;
        }

        public override void Receive(xContent content)
        {
            Tracer?.Invoke("Receive" + name);
            result = (TResult)new TResult().SetContent(content);
            EventReceive?.Invoke(this, (TResult)result);
        }
    }
}
