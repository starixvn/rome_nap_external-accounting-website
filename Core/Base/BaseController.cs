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
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.SessionState;
    using Fanex.CustomerSetting.Module.Core.Web;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.PTEngine.BusinessObjects;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Base controller for controller to inherit.
    /// </summary>
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class BaseController : Controller
    {
        #region Constant

        /// <summary>
        /// Constant object.
        /// </summary>
        public const int Super = 4;

        /// <summary>
        /// Constant object.
        /// </summary>
        public const int Master = 3;

        /// <summary>
        /// Constant object.
        /// </summary>
        public const int Agent = 2;

        /// <summary>
        /// Constant object.
        /// </summary>
        public const int Member = 1;

        /// <summary>
        /// Constant object.
        /// </summary>
        protected const string SiteName = "ExtAcc";

        /// <summary>
        /// Constant object.
        /// </summary>
        protected const int FromSystem = 4;

        #endregion Constant

        /// <summary>
        /// Context of current logging in session.
        /// </summary>
        private readonly ICurrentContext _currentContext;

        /// <summary>
        /// Session of current logging in session.
        /// </summary>
        private readonly ICurrentSession _currentSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        protected BaseController()
        {
            this._currentContext = new Fanex.ExternalAccount.Core.Context.CurrentContext();
            this._currentSession = new Fanex.ExternalAccount.Core.Context.CurrentSession();
        }

        /// <summary>
        /// Gets current context of this session.
        /// </summary>
        /// <value>ICurrentContext object.</value>
        protected virtual ICurrentContext CurrentContext
        {
            get
            {
                return this._currentContext ?? new Fanex.ExternalAccount.Core.Context.CurrentContext();
            }
        }

        /// <summary>
        /// Gets current session of this session.
        /// </summary>
        /// <value>ICurrentSession object.</value>
        protected virtual ICurrentSession CurrentSession
        {
            get
            {
                return this._currentSession ?? new Fanex.ExternalAccount.Core.Context.CurrentSession();
            }
        }

        /// <summary>
        /// Returns View associated with model object.
        /// </summary>
        /// <param name="viewName">URL to view.</param>
        /// <param name="model">Model object.</param>
        /// <returns>ActionResult object.</returns>
        [OutputCache(Duration = 604800, VaryByParam = "*")]
        protected virtual ActionResult ViewBase(string viewName = null, object model = null)
        {
            Response.AppendHeader("Content-Encoding", "gzip");
            Response.Filter = new System.IO.Compression.GZipStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);

            return string.IsNullOrWhiteSpace(viewName) ? this.View(model) : this.View(viewName, model);
        }

        /// <summary>
        /// JSON base method to render JSON to output stream.
        /// </summary>
        /// <param name="data">Data need to render JSON object.</param>
        /// <param name="settings">Allow Get method by default.</param>
        /// <returns>ActionResult object.</returns>
        protected virtual ActionResult JsonBase(object data, JsonSerializerSettings settings = null)
        {
            if (settings == null)
            {
                settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            return this.JsonExt(data, settings);
        }

        /// <summary>
        /// JSON base method to render JSON to output stream.
        /// </summary>
        /// <param name="data">Data need to render JSON object.</param>
        /// <param name="settings">Allow Get method by default.</param>
        /// <returns>ActionResult object.</returns>
        protected virtual ActionResult JsonBase2(object data, JsonRequestBehavior settings = JsonRequestBehavior.AllowGet)
        {
            return this.Json(data, settings);
        }

        /// <summary>
        /// Returns Partial View associated with model object.
        /// </summary>
        /// <param name="objModel">Model object.</param>
        /// <param name="viewName">URL to view.</param>
        /// <returns>PartialViewResult object.</returns>
        protected virtual ActionResult PartialViewBase(object objModel = null, string viewName = null)
        {
            return string.IsNullOrWhiteSpace(viewName) ? this.PartialView(objModel) : this.PartialView(viewName, objModel);
        }

        /// <summary>
        /// Exports excel function.
        /// </summary>
        /// <param name="excelStream">Excel stream.</param>
        /// <param name="fileName">Name of excel file.</param>
        protected virtual void ExportToExcel(MemoryStream excelStream, string fileName)
        {
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            Response.Clear();
            Response.BinaryWrite(excelStream.GetBuffer());
            Response.End();
        }

        /// <summary>
        /// Updates Casino statues.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="result">Result object.</param>
        /// <param name="customer">Customer object.</param>
        protected void UpdateCasinoStatus(int custId, Result result, Customer customer)
        {
            if (string.IsNullOrWhiteSpace(result.ErrorCode) || Utils.ToInt32(result.ErrorCode) == 0)
            {
                // Update Casino status for closed and suspended
                var cust = new CasinoLibrary.SBAgent.Customer();
                try
                {
                    cust.UpdateCustClosed(custId, customer.StatusSetting.Closed, CurrentContext.Ip4Address, CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                    cust.UpdateCustSuspend(custId, customer.StatusSetting.Suspended, CurrentContext.Ip4Address, CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Updates casino statuses.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="closed">Closed status.</param>
        /// <param name="suspended">Suspended status.</param>
        protected void UpdateCasinoStatus2(int custId, bool closed, bool suspended)
        {
            var cust = new CasinoLibrary.SBAgent.Customer();
            try
            {
                cust.UpdateCustClosed(custId, closed, CurrentContext.Ip4Address, CurrentSession.UserId, CurrentSession.UserName, FromSystem);
                cust.UpdateCustSuspend(custId, suspended, CurrentContext.Ip4Address, CurrentSession.UserId, CurrentSession.UserName, FromSystem);
            }
            catch (Exception)
            {
            }
        }

        protected bool IsBelong(string groupname, int custId, string userName, bool isGetUplineInfo)
        {
            var groups = new Fanex.ExternalAccount.Service.Impl.Login.LoginService().GroupList();
            var result = new ReportService().GetCustomerInfo(groupname, custId, userName, isGetUplineInfo);
            var hasGroup = CurrentSession.IsInternalAdmin ? groups.Any(o => o.GroupName.ToLower() == groupname.ToLower()) :
                groups.Where(o => o.GroupID == CurrentSession.GroupId).First().GroupName.ToLower() == groupname.ToLower();

            return (result != null && result.Rows.Count > 0 && hasGroup) || (custId == 0 && hasGroup);
        }

        /// <summary>
        /// Gets level name of request.
        /// </summary>
        /// <param name="viewlevel">Level id of user.</param>
        /// <returns>Level name of user.</returns>
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