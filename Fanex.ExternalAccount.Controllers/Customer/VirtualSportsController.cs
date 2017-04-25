// --------------------------------------------------------------------------
// <copyright file="VirtualSportsController.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.13">StyleCop</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Customer.VirtualSports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Fanex.PTEngine.BusinessObjects;
    using Fanex.PTVirtualSports.Services;
    using Fanex.PTVirtualSports.Utilities;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VirtualSportsController : BaseController
    {
        private bool _isChanged = false;
        private List<Fanex.ExternalAccount.DTO.Result> _result = new List<Fanex.ExternalAccount.DTO.Result>();
                
        [ExternalAccountAuthenticate(Function = eFunctions.VirtualSports, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult EditSingle(PTVirtualSportsModel model)
        {
            if (!this.IsBelong(Request["groupName"], model.TargetCustId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            return this.PartialViewBase(model);
        }

        /// <summary>
        /// Gets virtual sports data.
        /// </summary>
        /// <param name="model">Model object.</param>
        /// <returns>ActionResult object.</returns>
        [HttpGet]
        [ExternalAccountAuthenticate(Function = eFunctions.VirtualSports, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult DataSource(PTVirtualSportsModel model)
        {
            IRequestContext currentContext = new RequestContext()
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = model.TargetCustId,
                TargetRoleId = model.TargetRoleId,
                CustId = model.CustId,
                RoleId = model.RoleId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper()
            };

            var customerService = new CustomerService();
            var customer = customerService.GetCustomer(currentContext);
            Hashtable ht = new Hashtable();
            ht.Add("customer", customer);
            ht.Add("context", currentContext);
            var result = SerializeToJs(ht);

            return this.Content(result);
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="model">Model object.</param>
        /// <returns>ActionResult object.</returns>
        [HttpPost]
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        [ExternalAccountAuthenticate(Function = eFunctions.VirtualSports, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateCustomerSetting(PTVirtualSportsUpdateModel model)
        {
            if (!this.IsBelong(Request["groupName"], model.TargetCustId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            IRequestContext currentContext = new RequestContext()
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = model.TargetCustId,
                TargetRoleId = model.TargetRoleId,
                CustId = model.CustId,
                RoleId = model.RoleId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper()
            };

            var customerService = new CustomerService();
            ICustomer oldCustomer = customerService.GetCustomer(currentContext);
            ICustomer newCustomer = JsonConvert.DeserializeObject<Customer>(Request.Form[1]);
            Hashtable otherSetting = JsonConvert.DeserializeObject<Hashtable>(Request.Form[2]);
            
            switch (model.UpdateType)
            {
                case 0:
                    break;

                case 1: // status
                    oldCustomer.BettingLimit = newCustomer.BettingLimit;
                    oldCustomer.PtSetting = newCustomer.PtSetting;
                    break;

                case 2: // bet setting
                    oldCustomer.StatusSetting = newCustomer.StatusSetting;
                    oldCustomer.PtSetting = newCustomer.PtSetting;
                    break;

                case 3: // position taking
                    oldCustomer.StatusSetting = newCustomer.StatusSetting;
                    oldCustomer.BettingLimit = newCustomer.BettingLimit;
                    break;
            }

            if (newCustomer != null)
            {
                var results = customerService.SaveCustomer(oldCustomer, newCustomer, currentContext, otherSetting);
                this.UpdateStatusMessages(currentContext, results.ResultStatus);
                this.UpdateBettingLimitMessages(currentContext, results.ResultBetSetting);
                this.UpdatePTMessages(currentContext, results.ResultPT);

                if (!this._isChanged)
                {
                    return this.JsonBase(new { errorCode = 50, errorMessage = ViewLang.NoChangeToUpdate });
                }

                var settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                var responseContent = JsonConvert.SerializeObject(this._result, Formatting.None, settings);
                return this.Content(responseContent);
            }

            return this.ViewBase("Error");
        }

        internal static string SerializeToJs(Hashtable tb)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return SerializeToJs(tb, settings);
        }

        /// <summary>
        /// Serializes java script data.
        /// </summary>
        /// <param name="tb">Hash table object.</param>
        /// <param name="settings">JSON settings object.</param>
        /// <returns>String object.</returns>
        internal static string SerializeToJs(Hashtable tb, JsonSerializerSettings settings)
        {
            string result = string.Empty;

            foreach (var key in tb.Keys)
            {
                var value = tb[key];
                result += "var " + key + " = " + JsonConvert.SerializeObject(value, Formatting.None, settings) + ";\n";
            }

            return result;
        }

        /// <summary>
        /// Updates status message.
        /// </summary>
        /// <param name="currentContext">IRequestContext object.</param>
        /// <param name="results">Result object.</param>
        [NonAction]
        private void UpdateStatusMessages(IRequestContext currentContext, IList<Fanex.PTEngine.BusinessObjects.Result> results)
        {
            if (results.Count > 0)
            {
                this._isChanged = true;
                string msg = ViewLang.Status + ": ";

                foreach (var r in results)
                {
                    int errCode = Convert.ToInt32(r.ErrorCode);
                    if (errCode == 0)
                    {
                        errCode = -2;
                        r.ErrorMessage = msg + ViewLang.UpdatedSuccessfully;
                    }
                    else if (errCode == 101 || errCode == 102 || errCode == 103 || errCode == 104)
                    {
                        r.ErrorMessage = msg + r.ErrorMessage;
                    }

                    this._result.Add(new Fanex.ExternalAccount.DTO.Result(errCode, r.ErrorMessage));
                }
            }
        }

        /// <summary>
        /// Updates betting limit messages.
        /// </summary>
        /// <param name="currentContext">IRequestContext object.</param>
        /// <param name="results">Result object.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        [NonAction]
        private void UpdateBettingLimitMessages(IRequestContext currentContext, IList<Fanex.PTVirtualSports.BusinessObjects.Result> results)
        {
            if (results.Count > 0)
            {
                this._isChanged = true;
                foreach (var r in results)
                {
                    int errCode = Convert.ToInt32(r.ErrorCode);
                    string msg1 = ViewLang.MinBet + "/" + ViewLang.MaxBet + ": ";
                    string msg2 = ViewLang.MaxPerMatch + ": ";

                    switch (errCode)
                    {
                        case -101:
                            r.ErrorMessage = msg1 + ViewLang.UpdatedSuccessfully;
                            break;

                        case -102:
                            r.ErrorMessage = msg2 + ViewLang.UpdatedSuccessfully;
                            break;

                        default:
                            switch (currentContext.TargetRoleId)
                            {
                                case Constant.MASTER_ROLEID:
                                    if (errCode == 147)
                                    {
                                        r.ErrorMessage = msg1 + r.ErrorMessage;
                                    }
                                    else if (errCode == 115)
                                    {
                                        r.ErrorMessage = msg2 + r.ErrorMessage;
                                    }

                                    break;

                                case Constant.AGENT_ROLEID:
                                    if (errCode == 105)
                                    {
                                        r.ErrorMessage = msg1 + r.ErrorMessage;
                                    }
                                    else if (errCode == 115)
                                    {
                                        r.ErrorMessage = msg2 + r.ErrorMessage;
                                    }

                                    break;

                                case Constant.MEMBER_ROLEID:
                                    if (errCode == 106)
                                    {
                                        r.ErrorMessage = msg1 + r.ErrorMessage;
                                    }
                                    else if (errCode == 115)
                                    {
                                        r.ErrorMessage = msg2 + r.ErrorMessage;
                                    }

                                    break;
                            }

                            break;
                    }

                    string sportName = CacheRef.GetSportName(r.SportTypeId);
                    this._result.Add(new Fanex.ExternalAccount.DTO.Result(errCode, sportName + " - " + r.ErrorMessage));
                }
            }
        }

        /// <summary>
        /// Updates PT messages.
        /// </summary>
        /// <param name="currentContext">IRequestContext object.</param>
        /// <param name="results">Result object.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        [NonAction]
        private void UpdatePTMessages(IRequestContext currentContext, IList<Fanex.PTVirtualSports.BusinessObjects.Result> results)
        {
            if (results.Count > 0)
            {
                this._isChanged = true;
                foreach (var r in results)
                {
                    int errCode = Convert.ToInt32(r.ErrorCode);
                    string msg1 = ViewLang.PositionTaking + ": ";
                    string msg2 = ViewLang.MinPT + ": ";

                    switch (errCode)
                    {
                        case -101:
                            r.ErrorMessage = msg1 + ViewLang.UpdatedSuccessfully;
                            break;

                        case -102:
                            r.ErrorMessage = msg2 + ViewLang.UpdatedSuccessfully;
                            break;

                        case -103:
                            r.ErrorMessage = msg1 + ViewLang.NoChangeToUpdate;
                            break;

                        default:
                            switch (currentContext.TargetRoleId)
                            {
                                case Constant.MASTER_ROLEID:
                                    if (errCode == 147)
                                    {
                                        r.ErrorMessage = msg1 + r.ErrorMessage;
                                    }
                                    else if (errCode == 115)
                                    {
                                        r.ErrorMessage = msg2 + r.ErrorMessage;
                                    }

                                    break;

                                case Constant.AGENT_ROLEID:
                                    if (errCode == 105)
                                    {
                                        r.ErrorMessage = msg1 + r.ErrorMessage;
                                    }
                                    else if (errCode == 115)
                                    {
                                        r.ErrorMessage = msg2 + r.ErrorMessage;
                                    }

                                    break;

                                case Constant.MEMBER_ROLEID:
                                    switch (currentContext.RoleId)
                                    {
                                        case Constant.SUPER_ROLEID:
                                            if (errCode == 106)
                                            {
                                                r.ErrorMessage = msg1 + r.ErrorMessage;
                                            }
                                            else if (errCode == 115)
                                            {
                                                r.ErrorMessage = msg2 + r.ErrorMessage;
                                            }
                                            else if (errCode == 99)
                                            {
                                                r.ErrorMessage = msg1 + r.ErrorMessage;
                                            }

                                            break;

                                        case Constant.MASTER_ROLEID:
                                            if (errCode == 106)
                                            {
                                                r.ErrorMessage = msg1;
                                            }
                                            else if (errCode == 115)
                                            {
                                                r.ErrorMessage = msg2;
                                            }
                                            else if (errCode == 98)
                                            {
                                                r.ErrorMessage = msg1 + r.ErrorMessage;
                                            }

                                            break;

                                        case Constant.AGENT_ROLEID:
                                            if (errCode == 106)
                                            {
                                                r.ErrorMessage = msg1 + r.ErrorMessage;
                                            }
                                            else if (errCode == 115)
                                            {
                                                r.ErrorMessage = msg2 + r.ErrorMessage;
                                            }

                                            break;
                                    }

                                    break;
                            }

                            break;
                    }

                    string sportName = CacheRef.GetSportName(r.SportTypeId);
                    this._result.Add(new Fanex.ExternalAccount.DTO.Result(errCode, sportName + " - " + r.ErrorMessage));
                }
            }
        }
    }
}