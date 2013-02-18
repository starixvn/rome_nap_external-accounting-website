// --------------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// Base controller for controller to inherit
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.13">Style Cop</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Base
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Web.Mvc;
    using System.Web.SessionState;
    using Fanex.ExternalAccount.Controllers.Context;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Base controller for controller to inherit
    /// </summary>
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class BaseController : Controller
    {
        #region Constant

        /// <summary>
        /// Constant object
        /// </summary>
        protected const string VALID = "valid";

        /// <summary>
        /// Constant object
        /// </summary>
        protected const string ACTIVE = "active";

        /// <summary>
        /// Constant object
        /// </summary>
        protected const string INACTIVE = "inactive";

        /// <summary>
        /// Constant object
        /// </summary>
        protected const string TRUE = "true";

        /// <summary>
        /// Constant object
        /// </summary>
        protected const string FALSE = "false";

        /// <summary>
        /// Constant object
        /// </summary>
        protected const string OK = "OK";

        /// <summary>
        /// Constant object
        /// </summary>
        protected const string SiteName = "ExtAcc";

        #endregion Constant

        /// <summary>
        /// Context of current logging in session
        /// </summary>
        private readonly ICurrentContext currentContext;

        /// <summary>
        /// Session of current logging in session
        /// </summary>
        private readonly ICurrentSession currentSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class
        /// </summary>
        public BaseController()
        {
            this.currentContext = new CurrentContext();
            this.currentSession = new CurrentSession();
        }

        /// <summary>
        /// Gets current context of this session
        /// </summary>
        public ICurrentContext CurrentContext
        {
            get
            {
                return this.currentContext ?? new CurrentContext();
            }
        }

        /// <summary>
        /// Gets current session of this session
        /// </summary>
        public ICurrentSession CurrentSession
        {
            get
            {
                return this.currentSession ?? new CurrentSession();
            }
        }

        /// <summary>
        /// Returns View associated with model object
        /// </summary>
        /// <param name="objModel">Model object</param>
        /// <returns>ActionResult object </returns>
        [OutputCache(Duration = 604800, VaryByParam = "*")]
        public virtual ActionResult ViewBase(object objModel = null)
        {
            Response.AppendHeader("Content-Encoding", "gzip");
            Response.Filter = new System.IO.Compression.GZipStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);

            return this.View(objModel);
        }

        /// <summary>
        /// JSON base method to render JSON to output stream
        /// </summary>
        /// <param name="data">Data need to render JSON object</param>
        /// <param name="settings">Allow Get method by default</param>
        /// <returns>ActionResult object</returns>
        public virtual ActionResult JsonBase(object data, JsonSerializerSettings settings = null)
        {
            if (settings == null)
            {
                settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.Formatting = Formatting.Indented;
            }

            return this.JsonExt(data, settings);
        }

        /// <summary>
        /// Returns Partial View associated with model object
        /// </summary>
        /// <param name="objModel">Model object</param>
        /// <returns>PartialViewResult object</returns>
        public virtual ActionResult PartialViewBase(object objModel = null)
        {
            return this.PartialView(objModel);
        }

        /// <summary>
        /// Exports excel function
        /// </summary>
        /// <param name="excelStream">Excel stream</param>
        /// <param name="fileName">Name of excel file</param>
        protected void ExportToExcel(MemoryStream excelStream, string fileName)
        {
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            Response.Clear();
            Response.BinaryWrite(excelStream.GetBuffer());
            Response.End();
        }

        /// <summary>
        /// Gets level name of request
        /// </summary>
        /// <param name="viewlevel">Level id</param>
        /// <returns>Level name</returns>
        protected string LevelName(int viewlevel)
        {
            switch (viewlevel)
            {
                case 1:
                    return "Member";
                case 2:
                    return "Agent";
                case 3:
                    return "Master";
                default:
                    return "Super";
            }
        }
    }
}