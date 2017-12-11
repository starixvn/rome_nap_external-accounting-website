namespace Fanex.ExternalAccount.Core.Base
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Newtonsoft.Json;
        
    public class JsonBase : ActionResult
    {
        public JsonBase()
        {
        }

        public JsonBase(object responseBody)
        {
            this.ResponseBody = responseBody;
        }

        public JsonBase(object responseBody, JsonSerializerSettings settings)
            : this(responseBody)
        {
            this.Settings = settings;
        }

        public JsonSerializerSettings Settings { get; set; }

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public object ResponseBody { get; set; }

        private Formatting Formatting
        {
            get
            {
                return Debugger.IsAttached ? Formatting.Indented : Formatting.None;
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;

            if (!response.IsRequestBeingRedirected)
            {
                if (!string.IsNullOrEmpty(this.ContentType))
                {
                    response.ContentType = this.ContentType;
                }
                else
                {
                    response.ContentType = "application/json";
                }
            }

            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            if (this.ResponseBody != null)
            {
                response.Write(JsonConvert.SerializeObject(this.ResponseBody, this.Formatting, this.Settings));
            }
        }
    }
}