// --------------------------------------------------------------------------
// <copyright file="JsonBase.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.2.8">Created</change>
// </history>

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
        
    /// <summary>
    /// JSON class for render JSON to output stream.
    /// </summary>
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

        /// <summary>Gets or sets the serializer settings.</summary>
        /// <value>System.Object object.</value>
        public JsonSerializerSettings Settings { get; set; }

        /// <summary>Gets or sets the encoding of the response.</summary>
        /// <value>System.Object object.</value>
        public Encoding ContentEncoding { get; set; }

        /// <summary>Gets or sets the content type for the response.</summary>
        /// <value>System.Object object.</value>
        public string ContentType { get; set; }

        /// <summary>Gets or sets the body of the response.</summary>
        /// <value>System.Object object.</value>
        public object ResponseBody { get; set; }

        /// <summary>Gets the formatting types depending on whether we are in debug mode.</summary>
        /// <value>System.Object object.</value>
        private Formatting Formatting
        {
            get
            {
                return Debugger.IsAttached ? Formatting.Indented : Formatting.None;
            }
        }

        /// <summary>
        /// Serializes the response and writes it out to the response object.
        /// </summary>
        /// <param name="context">The execution context.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;

            // set content type
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

            // set content encoding
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