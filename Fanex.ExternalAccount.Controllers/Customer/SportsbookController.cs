// --------------------------------------------------------------------------
// <copyright file="SportsbookController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.13">StyleCop</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    using System.Collections;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Fanex.PTEngine.BusinessObjects;
    using Fanex.PTEngine.Services;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SportsbookController : BaseController
    {
        private ICommissionService _commissionService = new CommissionService();

        #region Bet Setting

        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditBetSetting()
        {
            return this.PartialViewBase();
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditBetSettingMasterGet(int custId, int parentId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            Fanex.PTEngine.Services.IBettingLimitService bettingLimitService = new Fanex.PTEngine.Services.BettingLimitService();

            IRequestContext currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(),
                CustId = parentId,
                TargetCustId = custId,
                RoleId = (int)CustomerRole.Super,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E",
            };

            IBettingLimit<BettingLimitItem> bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);

            return this.JsonBase(new { betting = bettingLimit, context = currentContext }, settings);
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditBetSettingAgentGet(int custId, int parentId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            Fanex.PTEngine.Services.IBettingLimitService bettingLimitService = new Fanex.PTEngine.Services.BettingLimitService();

            IRequestContext currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(),
                CustId = parentId,
                TargetCustId = custId,
                RoleId = (int)CustomerRole.Master,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E",
            };

            IBettingLimit<BettingLimitItem> bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);

            return this.JsonBase(new { betting = bettingLimit, context = currentContext }, settings);
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditBetSettingMemberGet(int custId, int parentId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            Fanex.PTEngine.Services.IBettingLimitService bettingLimitService = new Fanex.PTEngine.Services.BettingLimitService();

            IRequestContext currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(),
                CustId = parentId,
                TargetCustId = custId,
                RoleId = (int)CustomerRole.Agent,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E",
            };

            IBettingLimit<BettingLimitItem> bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);

            return this.JsonBase(new { betting = bettingLimit, context = currentContext }, settings);
        }

        /// <summary>
        /// Updates settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="txtCustomerObject">JSON string.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Update)]
        public ActionResult EditBetSettingPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            var bettingLimit = JsonConvert.DeserializeObject<BettingLimit>(txtCustomerObject, settings);

            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                TargetCustId = custId,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            IBettingLimitService bettingLimitService = new BettingLimitService();

            Fanex.PTEngine.BusinessObjects.Result result = bettingLimitService.UpdateBetting(bettingLimit, requestContext);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        #endregion Bet Setting

        #region Edit Commission

        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditCommission()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditCommissionDetail()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditCommissionMember()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditCommissionMemberDetail()
        {
            return this.PartialViewBase();
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditCommissionGet(int custId, int parentId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            IRequestContext currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(),
                CustId = parentId,
                TargetCustId = custId,
                RoleId = (int)CustomerRole.Super,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E",
            };

            ICommissionSetting commissionSetting = this._commissionService.GetCommission(currentContext);

            return this.JsonBase(new { commission = commissionSetting, context = currentContext }, settings);
        }

        /// <summary>
        /// Updates settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="currentLevel">Current level.</param>
        /// <param name="txtCustomerObject">JSON string.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Update)]
        public ActionResult EditCommissionPost(int custId, int parentId, int currentLevel, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            ICommissionSetting commissionSetting = JsonConvert.DeserializeObject<CommissionSetting>(txtCustomerObject, settings);

            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                TargetCustId = custId,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            DTO.Result result = null;
            if (currentLevel == (int)CustomerRole.Master)
            {
                result = this._commissionService.UpdateCommissionMaster(commissionSetting, requestContext);
            }
            else if (currentLevel == (int)CustomerRole.Agent)
            {
                result = this._commissionService.UpdateCommissionAgent(commissionSetting, requestContext);
            }
            else if (currentLevel == (int)CustomerRole.Member)
            {
                result = this._commissionService.UpdateCommissionMember(commissionSetting, requestContext);
            }

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        #endregion Edit Commission

        #region PT Setting

        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditPT(/*int custId, int parentId*/)
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult SuperMasterEditSB()
        {
            return this.PartialViewBase();
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditPTSuperGet(int custId, int parentId)
        {
            var setting = new Fanex.PTEngine.Services.CustomerService();
            var context = new RequestContext()
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                TargetRoleId = (int)CustomerRole.Super,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper()
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            customer = setting.GetCustomerForCompanyEditSuper(context);

            return this.JsonBase(
                    new { context = context, customer = customer }, 
                    new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore,
                    });
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditPTMasterGet(int custId, int parentId)
        {
            var setting = new Fanex.PTEngine.Services.CustomerService();
            var context = new RequestContext()
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper()
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            customer = setting.GetCustomerForSuperEditMaster(context);

            return this.JsonBase(
                new { context = context, customer = customer }, 
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        /// <summary>
        /// Updates settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="txtCustomerObject">JSON string.</param>
        /// <param name="isUpdateAllMember">Is update all member.</param>
        /// <param name="newPositionTaking">New position taking.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Update)]
        public ActionResult SuperUpdateMasterPTPost(int custId, int parentId, string txtCustomerObject, bool isUpdateAllMember, decimal newPositionTaking)
        {
            var mult = Request["isMult"];
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;

            if (!string.IsNullOrWhiteSpace(para))
            {
                for (int i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);

            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            };

            var custSrv = new Fanex.PTEngine.Services.CustomerService();
            var result = new Fanex.PTEngine.BusinessObjects.Result();

            if (!string.IsNullOrWhiteSpace(mult) && mult.Equals("true"))
            {
                result = custSrv.SuperUpdateMultipleMasters(customer, requestContext, isUpdateAllMember, newPositionTaking, param);
            }
            else
            {
                result = custSrv.SuperUpdateMaster(customer, requestContext, isUpdateAllMember, newPositionTaking);
            }

            this.UpdateCasinoStatus(custId, result, customer);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditPTAgentGet(int custId, int parentId)
        {
            var setting = new Fanex.PTEngine.Services.CustomerService();
            var context = new RequestContext()
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper()
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            customer = setting.GetCustomerForMasterEditAgent(context);

            return this.JsonBase(
                new { context = context, customer = customer }, 
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        /// <summary>
        /// Updates settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="txtCustomerObject">JSON string.</param>
        /// <param name="isUpdateAllMember">Is update all member.</param>
        /// <param name="newPositionTaking">New position taking.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Update)]
        public ActionResult MasterUpdateAgentPTPost(int custId, int parentId, string txtCustomerObject, bool isUpdateAllMember, decimal newPositionTaking)
        {
            var mult = Request["isMult"];
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;

            if (!string.IsNullOrWhiteSpace(para))
            {
                for (int i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);

            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            };

            var custSrv = new Fanex.PTEngine.Services.CustomerService();

            var result = new Fanex.PTEngine.BusinessObjects.Result();

            if (!string.IsNullOrWhiteSpace(mult) && mult.Equals("true"))
            {
                result = custSrv.MasterUpdateMultipleAgents(customer, requestContext, isUpdateAllMember, newPositionTaking, param);
            }
            else
            {
                result = custSrv.MasterUpdateAgent(customer, requestContext, isUpdateAllMember, newPositionTaking);
            }

            this.UpdateCasinoStatus(custId, result, customer);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditPTMemberGet(int custId, int parentId)
        {
            var setting = new Fanex.PTEngine.Services.CustomerService();
            var context = new RequestContext()
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Agent,
                TargetRoleId = (int)CustomerRole.Member,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper()
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            customer = setting.GetCustomerForAgentUpdateMember(context);

            return this.JsonBase(
                new { context = context, customer = customer }, 
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        /// <summary>
        /// Updates settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="txtCustomerObject">JSON string.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Update)]
        public ActionResult AgentUpdateMemberPTPost(int custId, int parentId, string txtCustomerObject)
        {
            var mult = Request["isMult"];
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;

            if (!string.IsNullOrWhiteSpace(para))
            {
                for (int i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);

            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                RoleId = (int)CustomerRole.Agent,
                TargetRoleId = (int)CustomerRole.Member,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            };

            var custSrv = new Fanex.PTEngine.Services.CustomerService();

            var result = new Fanex.PTEngine.BusinessObjects.Result();

            if (!string.IsNullOrWhiteSpace(mult) && mult.Equals("true"))
            {
                result = custSrv.AgentUpdateMultipleMembers(customer, requestContext, param);
            }
            else
            {
                result = custSrv.AgentUpdateMember(customer, requestContext);
            }

            this.UpdateCasinoStatus(custId, result, customer);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="agentId">Agent's id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult SuperEditPTMemberGet(int custId, int parentId, int agentId)
        {
            var setting = new Fanex.PTEngine.Services.CustomerService();
            var context = new RequestContext()
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                AgentId = agentId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Member,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper()
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            customer = setting.GetCustomerForSuperEditMember(context);

            return this.JsonBase(
                new { context = context, customer = customer }, 
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        /// <summary>
        /// Updates settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="txtCustomerObject">JSON string.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Update)]
        public ActionResult SuperEditPTMemberPost(int custId, int parentId, string txtCustomerObject)
        {
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);

            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Member,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            };

            if (!string.IsNullOrWhiteSpace(para))
            {
                for (int i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var custSrv = new Fanex.PTEngine.Services.CustomerService();
            var result = new Fanex.PTEngine.BusinessObjects.Result();

            if (param.Count > 0)
            {
                result = custSrv.SuperUpdateMultipleMembers(customer, requestContext, param);
            }
            else
            {
                result = custSrv.SuperUpdateMember(customer, requestContext);
            }

            this.UpdateCasinoStatus(custId, result, customer);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="agentId">Agent's id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult MasterEditPTMemberGet(int custId, int parentId, int agentId)
        {
            var setting = new Fanex.PTEngine.Services.CustomerService();
            var context = new RequestContext()
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                AgentId = agentId,
                CustId = parentId,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Member,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper()
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            customer = setting.GetCustomerForMasterEditMember(context);

            return this.JsonBase(
                new { context = context, customer = customer }, 
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        /// <summary>
        /// Updates settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="txtCustomerObject">JSON string.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Update)]
        public ActionResult MasterEditPTMemberPost(int custId, int parentId, string txtCustomerObject)
        {
            var para = Request["para"];
            var param = new Hashtable();
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            var customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);

            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Member,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address
            };

            if (!string.IsNullOrWhiteSpace(para))
            {
                for (int i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var custSrv = new Fanex.PTEngine.Services.CustomerService();
            var result = new Fanex.PTEngine.BusinessObjects.Result();

            if (param.Count > 0)
            {
                result = custSrv.MasterUpdateMultipleMembers(customer, requestContext, param);
            }
            else
            {
                result = custSrv.MasterUpdateMember(customer, requestContext);
            }

            this.UpdateCasinoStatus(custId, result, customer);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        #endregion PT Setting

        #region Multiple PT Setting

        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditMultiplePT()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult SuperMasterEditMultiplePTMember()
        {
            return this.PartialViewBase();
        }

        #endregion Multiple PT Setting

        #region Edit Min PT

        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult EditMinPT()
        {
            return this.PartialViewBase();
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult SuperEditMinPTMasterGet(int custId, int parentId)
        {
            var context = new RequestContext()
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditMinPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper()
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            var customerService = new Fanex.PTEngine.Services.CustomerService();
            customer = customerService.GetCustomerForSuperEditMinPTMaster(context);

            customer.BettingLimit = null;
            customer.StatusSetting = null;
            customer.CommissionSetting = null;
            customer.CommissionSetting2 = null;

            return this.JsonBase(
                new { context = context, customer = customer }, 
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        /// <summary>
        /// Gets settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Read)]
        public ActionResult MasterEditMinPTAgentGet(int custId, int parentId)
        {
            var context = new RequestContext()
            {
                AdminId = CurrentSession.UserId,
                TargetCustId = custId,
                CustId = parentId,
                FormId = FormType.EditMinPT,
                IP = CurrentContext.Ip4Address,
                Language = "En",
                LangKey = "E",
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                SiteName = SiteName,
                SubAccId = 0,
                UserName = CurrentSession.UserName.ToUpper()
            };

            ICustomer customer = new Customer();
            customer.PtSetting = new PTGrid();

            var customerService = new Fanex.PTEngine.Services.CustomerService();
            customer = customerService.GetCustomerForMasterEditMinPTAgent(context);

            customer.BettingLimit = null;
            customer.StatusSetting = null;
            customer.CommissionSetting = null;
            customer.CommissionSetting2 = null;

            return this.JsonBase(
                new { context = context, customer = customer }, 
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        /// <summary>
        /// Updates settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="txtCustomerObject">JSON string.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Update)]
        public ActionResult SuperEditMinPTMasterPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            Customer customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                RoleId = (int)CustomerRole.Super,
                TargetRoleId = (int)CustomerRole.Master,
                FormId = FormType.EditMinPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            var result = new Fanex.PTEngine.Services.CustomerService().SuperUpdateMinPTMaster(customer, requestContext);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        /// <summary>
        /// Updates settings data.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="parentId">Parent id.</param>
        /// <param name="txtCustomerObject">JSON string.</param>
        /// <returns>ActionResult object.</returns>
        [Authorization(Function = eFunctions.Sportsbook, Policy = EPolicy.Update)]
        public ActionResult MasterEditMinPTAgentPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;

            Customer customer = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);
            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                RoleId = (int)CustomerRole.Master,
                TargetRoleId = (int)CustomerRole.Agent,
                FormId = FormType.EditMinPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            var result = new Fanex.PTEngine.Services.CustomerService().MasterUpdateMinPTAgent(customer, requestContext);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        #endregion Edit Min PT
    }
}