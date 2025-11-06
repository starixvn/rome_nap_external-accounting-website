namespace Fanex.ExternalAccount.Core.Base
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class JsonBase : IActionResult
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

        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;

            if (!response.HasStarted)
            {
                response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            }

            if (ContentEncoding != null)
            {
                response.ContentType = $"{response.ContentType}; charset={ContentEncoding.WebName}";
            }

            if (ResponseBody != null)
            {
                var json = JsonConvert.SerializeObject(ResponseBody, Formatting, Settings);
                var bytes = (ContentEncoding ?? Encoding.UTF8).GetBytes(json);
                await response.Body.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}
