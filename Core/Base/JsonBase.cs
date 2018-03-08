namespace Fanex.ExternalAccount.Core.Base
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Web.Mvc;
    using Newtonsoft.Json;

    public class JsonBase : ActionResult
    {
        public JsonBase()
        {
        }

        public JsonBase(object responseBody)
        {
            ResponseBody = responseBody;
        }

        public JsonBase(object responseBody, JsonSerializerSettings settings)
            : this(responseBody)
        {
            Settings = settings;
        }

        public JsonSerializerSettings Settings { get; set; }

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public object ResponseBody { get; set; }

        private static Formatting Formatting => Debugger.IsAttached ? Formatting.Indented : Formatting.None;

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;

            if (!response.IsRequestBeingRedirected)
            {
                response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            }

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (ResponseBody != null)
            {
                response.Write(JsonConvert.SerializeObject(ResponseBody, Formatting, Settings));
            }
        }
    }
}