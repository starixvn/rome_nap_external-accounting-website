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

namespace Fanex.ExternalAccount.Core.Base
{
    using System.IO;
    using System.Web.Mvc;
    using System.Web.SessionState;
    using Fanex.ExternalAccount.Core.Context;
    using Newtonsoft.Json;

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
        public const int Super = 4;

        /// <summary>
        /// Constant object
        /// </summary>
        public const int Master = 3;

        /// <summary>
        /// Constant object
        /// </summary>
        public const int Agent = 2;

        /// <summary>
        /// Constant object
        /// </summary>
        public const int Member = 1;

        /// <summary>
        /// Constant object
        /// </summary>
        protected const string SiteName = "ExtAcc";

        /// <summary>
        /// Constant object
        /// </summary>
        protected const int FromSystem = 4;

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
        protected BaseController()
        {
            this.currentContext = new CurrentContext();
            this.currentSession = new CurrentSession();
        }

        /// <summary>
        /// Gets current context of this session
        /// </summary>
        protected virtual ICurrentContext CurrentContext
        {
            get
            {
                return this.currentContext ?? new CurrentContext();
            }
        }

        /// <summary>
        /// Gets current session of this session
        /// </summary>
        protected virtual ICurrentSession CurrentSession
        {
            get
            {
                return this.currentSession ?? new CurrentSession();
            }
        }

        /// <summary>
        /// Returns View associated with model object
        /// </summary>
        /// <param name="model">Model object</param>
        /// <returns>ActionResult object </returns>
        [OutputCache(Duration = 604800, VaryByParam = "*")]
        protected virtual ActionResult ViewBase(string viewName = null, object model = null)
        {
            Response.AppendHeader("Content-Encoding", "gzip");
            Response.Filter = new System.IO.Compression.GZipStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);

            return string.IsNullOrWhiteSpace(viewName) ? this.View(model) : this.View(viewName, model);
        }

        /// <summary>
        /// JSON base method to render JSON to output stream
        /// </summary>
        /// <param name="data">Data need to render JSON object</param>
        /// <param name="settings">Allow Get method by default</param>
        /// <returns>ActionResult object</returns>
        protected virtual ActionResult JsonBase(object data, JsonSerializerSettings settings = null)
        {
            if (settings == null)
            {
                settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
            }

            return this.JsonExt(data, settings);
        }

        /// <summary>
        /// JSON base method to render JSON to output stream
        /// </summary>
        /// <param name="data">Data need to render JSON object</param>
        /// <param name="settings">Allow Get method by default</param>
        /// <returns>ActionResult object</returns>
        protected virtual ActionResult JsonBase2(object data, JsonRequestBehavior settings = JsonRequestBehavior.AllowGet)
        {
            return this.Json(data, settings);
        }

        /// <summary>
        /// Returns Partial View associated with model object
        /// </summary>
        /// <param name="objModel">Model object</param>
        /// <returns>PartialViewResult object</returns>
        protected virtual ActionResult PartialViewBase(object objModel = null)
        {
            return this.PartialView(objModel);
        }

        /// <summary>
        /// Exports excel function
        /// </summary>
        /// <param name="excelStream">Excel stream</param>
        /// <param name="fileName">Name of excel file</param>
        protected virtual void ExportToExcel(MemoryStream excelStream, string fileName)
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
        protected virtual string LevelName(int viewlevel)
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