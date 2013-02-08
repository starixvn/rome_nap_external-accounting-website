// --------------------------------------------------------------------------
// <copyright file="JsonBase.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.2.8">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Base
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Newtonsoft.Json;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
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

        /// <summary>Gets or sets the serialiser settings</summary>
        public JsonSerializerSettings Settings { get; set; }

        /// <summary>Gets or sets the encoding of the response</summary>
        public Encoding ContentEncoding { get; set; }

        /// <summary>Gets or sets the content type for the response</summary>
        public string ContentType { get; set; }

        /// <summary>Gets or sets the body of the response</summary>
        public object ResponseBody { get; set; }

        /// <summary>Gets the formatting types depending on whether we are in debug mode</summary>
        private Formatting Formatting
        {
            get
            {
                return Debugger.IsAttached ? Formatting.Indented : Formatting.None;
            }
        }

        /// <summary>
        /// Serialises the response and writes it out to the response object
        /// </summary>
        /// <param name="context">The execution context</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            // set content type
            if (!string.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }

            // set content encoding
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