// --------------------------------------------------------------------------
// <copyright file="LiveCasinoBase.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.3.20">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Base
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Fanex.LiveCasinoSetting.Business;
    using Fanex.LiveCasinoSetting.Model;
    using Fanex.LiveCasinoSetting.Model.Log;
    using Fanex.LiveCasinoSetting.Request;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Base controller for live casino setting
    /// </summary>
    public class LiveCasinoBase : BaseController
    {
        /// <summary>
        /// Role super
        /// </summary>
        public const int RoleSuper = 4;

        /// <summary>
        /// Role master
        /// </summary>
        public const int RoleMaster = 3;

        /// <summary>
        /// Role agent
        /// </summary>
        public const int RoleAgent = 2;

        /// <summary>
        /// Role member
        /// </summary>
        public const int RoleMember = 1;

        /// <summary>
        /// Lock object for synchronization
        /// </summary>
        private static object locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveCasinoBase"/> class
        /// </summary>
        public LiveCasinoBase()
        {
            // Inits base properties
            this.GameSetting = new GameSettingService();
            this.PtSetting = new SettingService();
            this.MemberWinLimit = new MemberWinLimitService();
            this.LogWriter = new LogWriterService();
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected ISettingService PtSetting
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected IGameSettingService GameSetting
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected IMemberWinLimitService MemberWinLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets property
        /// </summary>
        protected ILogWriterService LogWriter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets member win limit value
        /// </summary>
        /// <param name="custId">Customer id</param>
        /// <returns>MemberWinLimit object</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult GetMemberWinLimit(int custId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return this.JsonBase(this.MemberWinLimit.GetMemberWinLimit(custId), settings);
        }

        /// <summary>
        /// Checks whether an account is disabled or not
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <param name="groupName">Group name that user belongs to</param>
        /// <returns>Boolean value</returns>
        protected bool IsDisableAccount(string username, string groupName)
        {
            var customerInfo = new ReportService().GetCustomerInfo(groupName, 0, username, true);
            var result = new DataTable();
            var srecommend = 0;
            var mrecommend = 0;
            var recommend = 0;
            var custId = 0;
            var roleId = 0;

            if (customerInfo != null)
            {
                roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                custId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);

                if (roleId == 4)
                {
                    result = new SuperService().GetData(null, custId, null, null, null, groupName);
                }
                else if (roleId == 3)
                {
                    result = new MasterService().GetData(srecommend, custId, null, null, null);
                }
                else if (roleId == 2)
                {
                    result = new AgentService().GetData(mrecommend, custId, null, null, null);
                }
                else
                {
                    result = new MemberService().GetData(recommend, custId, null, null, null);
                }
            }

            return !(bool)result.Rows[0]["enabled"];
        }

        /// <summary>
        /// Updates member win limit setting
        /// </summary>
        /// <param name="custId">Updated customer id</param>
        /// <param name="isDailyAutoReset">Is daily auto reset</param>
        /// <param name="maxWinLimit">Maximum win limit</param>
        /// <param name="customer">JSON customer string</param>
        /// <returns>ActionResult object</returns>
        protected Fanex.LiveCasinoSetting.Model.Result UpdateMemberWinLimit(int custId, bool isDailyAutoReset, decimal maxWinLimit, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);

            if (this.IsDisableAccount(model.CustomerInfo.UserName, model.CustomerInfo.GroupName))
            {
                return null;
            }

            var memberWinLimit = new MemberWinLimit()
            {
                IsDailyAutoReset = isDailyAutoReset,
                MaxWinLimit = maxWinLimit
            };

            var context = new RequestContext()
            {
                CustId = custId,
                Language = "en",
                SignUpIp = this.CurrentContext.Ip4Address,
                UserId = this.CurrentSession.UserId
            };

            var result = this.MemberWinLimit.UpdateMemberWinLimit(memberWinLimit, context);

            return result;
        }

        /// <summary>
        /// Updates game setting info
        /// </summary>
        /// <param name="uplineRoleId">Role id of up-line</param>
        /// <param name="custId">Customer id</param>
        /// <param name="customer">Customer string object</param>
        /// <returns>JSON string object</returns>
        protected List<object> UpdateGameSetting(int uplineRoleId, int custId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var oldGameTypes = this.GameSetting.GetByCustomerId(custId);
            var gameTypes = model.GameSetting;

            if (this.IsDisableAccount(model.CustomerInfo.UserName, model.CustomerInfo.GroupName))
            {
                return new List<object> { new { BettypeId = 1101, ErrorCode = 1, ErrorMessage = ViewLang.DisabledAccount } };
            }

            var req = this.GetInsertAccountRequest(custId, uplineRoleId, model.CustomerInfo.GroupName);

            if (oldGameTypes == null || oldGameTypes.Count == 0)
            {
                oldGameTypes = this.GameSetting.GetByCustomerId(uplineRoleId == RoleSuper ? req.SuperId : uplineRoleId == RoleMaster ? req.MasterId : req.AgentId);
            }

            if (oldGameTypes != null && oldGameTypes.Count > 0 && gameTypes.Count > 0)
            {
                oldGameTypes.ForEach(o => o.BettingLimit.ForEach(p =>
                {
                    gameTypes.ForEach(q =>
                    {
                        if (q.BetTypeId == o.BetTypeId)
                        {
                            q.BettingLimit.ForEach(r =>
                            {
                                if ((r.Id == p.Id && (r.IsSelected != p.IsSelected)) || r.IsSelected)
                                {
                                    r.IsDirty = true;
                                }
                            });
                        }
                    });
                }));

                var resultList = this.GameSetting.UpdateGameSetting(uplineRoleId, gameTypes, req);
                var results = new List<object>();

                foreach (var item in resultList)
                {
                    results.Add(new { BettypeId = item.Key, ErrorCode = item.Value.ErrorCode, ErrorMessage = item.Value.ErrorMessage });
                }

                // Write log stake level
                this.LogWriter.WriteLog(new LogUpdateStakeLevel(oldGameTypes, gameTypes, resultList)
                {
                    Action = this.LogActionUpdateStakeLevel(uplineRoleId - 1),
                    ErrorCode = resultList[gameTypes.ElementAt(0).BetTypeId].ErrorCode,
                    ErrorMessage = resultList[gameTypes.ElementAt(0).BetTypeId].ErrorMessage,
                    RoleId = uplineRoleId - 1,
                    SubAccId = req.SubAccId,
                    CustId = custId,
                    SignUpIp = this.CurrentContext.Ip4Address,
                    Language = "en",
                    UserId = req.UserId
                });

                return results;
            }

            return new List<object> { new { BettypeId = 0, ErrorCode = 0, ErrorMessage = string.Empty } };
        }

        /// <summary>
        /// Gets customer info
        /// </summary>
        /// <param name="custId">Customer Id</param>
        /// <param name="groupName">Group Name that user belongs to</param>
        /// <returns>CUSTINFO object</returns>
        protected CustInfo GetCustomerInfo(int custId, string groupName)
        {
            var report = new ReportService();
            var custInfo = report.GetCustomerInfo(groupName, custId, string.Empty, true);

            if (custInfo == null || custInfo.Rows.Count == 0)
            {
                return new CustInfo();
            }

            var item = custInfo.Rows[0];
            var info = new CustInfo
            {
                AgentId = (int)item["recommend"],
                MasterId = (int)item["mrecommend"],
                SuperId = (int)item["srecommend"],
                AgentName = item["agentname"] is DBNull ? string.Empty : item["agentname"].ToString(),
                MasterName = item["mastername"] is DBNull ? string.Empty : item["mastername"].ToString(),
                SuperName = item["supername"] is DBNull ? string.Empty : item["supername"].ToString(),
                CustId = (int)item["custid"],
                Username = (string)item["username"],
                RoleId = Utils.ToInt32(item["roleid"])
            };

            return info;
        }

        /// <summary>
        /// Gets log action update stake level
        /// </summary>
        /// <param name="roleId">Role id of customer</param>
        /// <returns>LogActions object</returns>
        protected LogActions LogActionUpdateStakeLevel(int roleId)
        {
            switch (roleId)
            {
                case RoleMaster:
                    return LogActions.UpdateStakeLevel_Master;
                case RoleAgent:
                    return LogActions.UpdateStakeLevel_Agent;
                case RoleMember:
                    return LogActions.UpdateStakeLevel_Member;
            }

            return LogActions.UpdateStakeLevel_Super;
        }

        /// <summary>
        /// Gets log action manual insert account
        /// </summary>
        /// <param name="roleId">Role id of customer</param>
        /// <returns>LogActions object</returns>
        protected LogActions LogActionManual(int roleId)
        {
            switch (roleId)
            {
                case RoleMember:
                    return LogActions.ManualInsert_Member;
                case RoleAgent:
                    return LogActions.ManualInsert_Agent;
                case RoleMaster:
                    return LogActions.ManualInsert_Master;
            }

            return LogActions.ManualInsert_Super;
        }

        /// <summary>
        /// Synchronizes account manually
        /// </summary>
        /// <param name="uplineRoleId">Role id of up-line</param>
        /// <param name="uplineId">Id of up-line</param>
        /// <param name="custId">Customer id</param>
        /// <param name="customer">Customer JSON string</param>
        /// <param name="groupName">Group name that user belongs to</param>
        /// <param name="isUpdateGameSetting">Boolean value</param>
        /// <returns>Result object</returns>
        protected Fanex.LiveCasinoSetting.Model.Result SyncManually(int uplineRoleId, int uplineId, int custId, string customer, string groupName = "ACCINDO", bool isUpdateGameSetting = false)
        {
            lock (locker)
            {
                var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
                var gameTypes = model.GameSetting;
                var result = new Fanex.LiveCasinoSetting.Model.Result();
                var custInfo = this.PtSetting.GetCustomerInfo(custId);
                var uplineInfo = this.PtSetting.GetCustomerInfo(uplineId);
                var userInfo = this.GetCustomerInfo(custId, model.CustomerInfo.GroupName);

                if (this.IsDisableAccount(model.CustomerInfo.UserName, model.CustomerInfo.GroupName))
                {
                    return null;
                }

                // Update game setting in case of not sync live casino
                if (isUpdateGameSetting && uplineInfo.IsSyncLiveCasino && !custInfo.IsSyncLiveCasino)
                {
                    // Update PT first
                    if (uplineRoleId == RoleSuper)
                    {
                        // Update master PT
                        result = this.UpdateMasterPT(custId, uplineId, customer);
                    }
                    else if (uplineRoleId == RoleMaster)
                    {
                        // Update agent PT
                        result = this.UpdateAgentPT(custId, uplineId, customer);
                    }
                    else
                    {
                        // Update member PT
                        result = this.UpdateMemberPT(custId, uplineId, customer);
                    }
                }
                else if (uplineInfo.IsSyncLiveCasino && !custInfo.IsSyncLiveCasino)
                {
                    // Sync live casino when updated PT
                    var req = this.GetInsertAccountRequest(custId, uplineRoleId, model.CustomerInfo.GroupName);

                    // Manually insert account first
                    result = this.GameSetting.InsertAccount(req);

                    // Update sync status for acc
                    ((GameSettingService)this.GameSetting).UpdateSyncLiveCasinoInfo(req);

                    // Insert account success then update game setting
                    if (string.IsNullOrWhiteSpace(result.ErrorCode) && gameTypes.Count > 0)
                    {
                        var oldGameTypes = this.GameSetting.GetByCustomerId(custId);

                        // Dont have game setting then get by upline
                        if (oldGameTypes == null || oldGameTypes.Count == 0)
                        {
                            oldGameTypes = this.GameSetting.GetByCustomerId(uplineId);
                        }

                        if (oldGameTypes != null || oldGameTypes.Count > 0)
                        {
                            // Force settings changed
                            oldGameTypes.ForEach(o => o.BettingLimit.ForEach(p =>
                            {
                                gameTypes.ForEach(q =>
                                {
                                    if (q.BetTypeId == o.BetTypeId)
                                    {
                                        q.BettingLimit.ForEach(r =>
                                        {
                                            if ((r.Id == p.Id && (r.IsSelected != p.IsSelected)) || r.IsSelected)
                                            {
                                                r.IsDirty = true;
                                            }
                                        });
                                    }
                                });
                            }));

                            // Update game settings
                            var resultList = ((GameSettingService)this.GameSetting).UpdateStakeLevel(custId, uplineRoleId - 1, gameTypes);

                            // Update game settings successfully then sync live casino
                            if (string.IsNullOrWhiteSpace(resultList[gameTypes.ElementAt(0).BetTypeId].ErrorCode))
                            {
                                // Write log manual insert account
                                var custList = new List<KeyValuePair<int, string>>();
                                custList.Add(new KeyValuePair<int, string>(req.CustId, req.CustUserName));

                                this.LogWriter.WriteLog(new Fanex.LiveCasinoSetting.Model.Log.LogManualInsertAccount()
                                {
                                    Action = this.LogActionManual(uplineRoleId - 1),
                                    AgentId = req.AgentId,
                                    CustId = req.CustId,
                                    CustUserName = req.CustUserName,
                                    ExchangeId = req.ExchangeId,
                                    SignUpIp = req.SignUpIp,
                                    Language = req.Language,
                                    MasterId = req.MasterId,
                                    RoleId = req.RoleId,
                                    SignUpForm = req.SignUpForm,
                                    SubAccId = req.SubAccId,
                                    SubAccName = req.SubAccName,
                                    SuperId = req.SuperId,
                                    UserId = req.UserId,
                                    UserName = req.UserName,
                                    ErrorCode = result.ErrorCode,
                                    ErrorMessage = result.ErrorMessage,
                                    CustomerList = custList
                                });

                                // Write log stake level
                                this.LogWriter.WriteLog(new LogUpdateStakeLevel(oldGameTypes, gameTypes, resultList)
                                                {
                                                    Action = this.LogActionUpdateStakeLevel(uplineRoleId - 1),
                                                    ErrorCode = resultList[gameTypes.ElementAt(0).BetTypeId].ErrorCode,
                                                    ErrorMessage = resultList[gameTypes.ElementAt(0).BetTypeId].ErrorMessage,
                                                    RoleId = uplineRoleId - 1,
                                                    SubAccId = req.SubAccId,
                                                    CustId = custId,
                                                    SignUpIp = this.CurrentContext.Ip4Address,
                                                    Language = "en",
                                                    UserId = req.UserId
                                                });
                            }
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Update PT
        /// </summary>
        /// <param name="custId">Customer id</param>
        /// <param name="uplineId">Up-line id</param>
        /// <param name="customer">Customer JSON string</param>
        /// <returns>Result object</returns>
        protected Fanex.LiveCasinoSetting.Model.Result UpdateMasterPT(int custId, int uplineId, string customer)
        {
            var context = new RequestContext
            {
                CustId = custId,
                IsSyncLiveCasino = true,
                Language = "en",
                SignUpIp = this.CurrentContext.Ip4Address,
                UplineId = uplineId,
                UserId = this.CurrentSession.UserId
            };

            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var pt = this.PtSetting.GetMasterPTSeting(custId, uplineId);
            var custInfo = this.PtSetting.GetCustomerInfo(custId);
            var uplineInfo = this.PtSetting.GetCustomerInfo(uplineId);

            if (this.IsDisableAccount(model.CustomerInfo.UserName, model.CustomerInfo.GroupName))
            {
                return null;
            }

            if (uplineInfo.IsSyncLiveCasino && !custInfo.IsSyncLiveCasino)
            {
                pt.PropertiesChangedList.Add("MaxPT");
                pt.PropertiesChangedList.Add("MinPT");
                pt.PropertiesChangedList.Add("Comm");

                ((SuperPTSetting)pt.Upline).PropertiesChangedList.Add("PresetPT");
                ((SuperPTSetting)pt.Upline).PropertiesChangedList.Add("AutoPresetPT");
                ((SuperPTSetting)pt.Upline).PropertiesChangedList.Add("IsAutoPresetPT");
            }
            else
            {
                // Handles Master values changed
                ((INotifyPropertyChanged)pt).PropertyChanged
                                 += (o, arg) =>
                                 {
                                     if (!pt.PropertiesChangedList.Contains(arg.PropertyName))
                                     {
                                         pt.PropertiesChangedList.Add(arg.PropertyName);
                                     }
                                 };

                // Handles Super values changed
                ((INotifyPropertyChanged)pt.Upline).PropertyChanged
                                 += (o, arg) =>
                                 {
                                     if (!((SuperPTSetting)pt.Upline).PropertiesChangedList.Contains(arg.PropertyName))
                                     {
                                         ((SuperPTSetting)pt.Upline).PropertiesChangedList.Add(arg.PropertyName);
                                     }
                                 };
            }

            // Super values
            ((SuperPTSetting)pt.Upline).PresetPT = model.UplinePtSetting.PresetPT;
            ((SuperPTSetting)pt.Upline).AutoPresetPT = model.UplinePtSetting.AutoPresetPT;
            ((SuperPTSetting)pt.Upline).IsAutoPresetPT = model.UplinePtSetting.IsAutoPresetPT;
            ((SuperPTSetting)pt.Upline).IsUpdateAllMember = model.UplinePtSetting.IsUpdateAllMember;
            ((SuperPTSetting)pt.Upline).NewMemberPT = model.UplinePtSetting.NewMemberPT;

            // Master values
            pt.MaxPT = model.PtSetting.MaxPT;
            pt.MinPT = model.PtSetting.MinPT;
            pt.Comm = model.PtSetting.Comm;

            var result = this.PtSetting.UpdateMasterPTSetting(context, pt);

            return result;
        }

        /// <summary>
        /// Update PT
        /// </summary>
        /// <param name="custId">Customer id</param>
        /// <param name="uplineId">Up-line id</param>
        /// <param name="customer">Customer JSON string</param>
        /// <returns>Result object</returns>
        protected Fanex.LiveCasinoSetting.Model.Result UpdateAgentPT(int custId, int uplineId, string customer)
        {
            var context = new RequestContext
            {
                CustId = custId,
                IsSyncLiveCasino = true,
                Language = "en",
                SignUpIp = this.CurrentContext.Ip4Address,
                UplineId = uplineId,
                UserId = this.CurrentSession.UserId
            };

            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var pt = this.PtSetting.GetAgentPTSetting(custId, uplineId);
            var custInfo = this.PtSetting.GetCustomerInfo(custId);
            var uplineInfo = this.PtSetting.GetCustomerInfo(uplineId);

            if (this.IsDisableAccount(model.CustomerInfo.UserName, model.CustomerInfo.GroupName))
            {
                return null;
            }

            if (uplineInfo.IsSyncLiveCasino && !custInfo.IsSyncLiveCasino)
            {
                pt.PropertiesChangedList.Add("MaxPT");
                pt.PropertiesChangedList.Add("MinPT");
                pt.PropertiesChangedList.Add("Comm");

                ((PTSetting)pt.Upline).PropertiesChangedList.Add("PresetPT");
                ((PTSetting)pt.Upline).PropertiesChangedList.Add("AutoPresetPT");
                ((PTSetting)pt.Upline).PropertiesChangedList.Add("IsAutoPresetPT");
            }
            else
            {
                // Handles Agent values changed
                ((INotifyPropertyChanged)pt).PropertyChanged
                                 += (o, arg) =>
                                 {
                                     if (!pt.PropertiesChangedList.Contains(arg.PropertyName))
                                     {
                                         pt.PropertiesChangedList.Add(arg.PropertyName);
                                     }
                                 };

                // Handles Master values changed
                ((INotifyPropertyChanged)pt.Upline).PropertyChanged
                                 += (o, arg) =>
                                 {
                                     if (!((SuperPTSetting)pt.Upline).PropertiesChangedList.Contains(arg.PropertyName))
                                     {
                                         ((SuperPTSetting)pt.Upline).PropertiesChangedList.Add(arg.PropertyName);
                                     }
                                 };
            }

            // Master values
            ((PTSetting)pt.Upline).PresetPT = model.UplinePtSetting.PresetPT;
            ((PTSetting)pt.Upline).AutoPresetPT = model.UplinePtSetting.AutoPresetPT;
            ((PTSetting)pt.Upline).IsAutoPresetPT = model.UplinePtSetting.IsAutoPresetPT;
            ((PTSetting)pt.Upline).IsUpdateAllMember = model.UplinePtSetting.IsUpdateAllMember;
            ((PTSetting)pt.Upline).NewMemberPT = model.UplinePtSetting.NewMemberPT;

            // Agent values
            pt.MaxPT = model.PtSetting.MaxPT;
            pt.MinPT = model.PtSetting.MinPT;
            pt.Comm = model.PtSetting.Comm;

            var result = this.PtSetting.UpdateAgentPTSetting(context, pt);

            return result;
        }

        /// <summary>
        /// Update PT
        /// </summary>
        /// <param name="custId">Customer id</param>
        /// <param name="uplineId">Up-line id</param>
        /// <param name="customer">Customer JSON string</param>
        /// <returns>Result object</returns>
        protected Fanex.LiveCasinoSetting.Model.Result UpdateMemberPT(int custId, int uplineId, string customer)
        {
            var context = new RequestContext
            {
                CustId = custId,
                IsSyncLiveCasino = true,
                Language = "en",
                SignUpIp = this.CurrentContext.Ip4Address,
                UplineId = uplineId,
                UserId = this.CurrentSession.UserId
            };

            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var pt = this.PtSetting.GetMemberPTSetting(custId, uplineId);            
            var custInfo = this.PtSetting.GetCustomerInfo(custId);
            var uplineInfo = this.PtSetting.GetCustomerInfo(uplineId);

            if (this.IsDisableAccount(model.CustomerInfo.UserName, model.CustomerInfo.GroupName))
            {
                return null;
            }

            if (uplineInfo.IsSyncLiveCasino && !custInfo.IsSyncLiveCasino)
            {
                pt.PropertiesChangedList.Add("Comm");

                ((PTSetting)pt.Upline).PropertiesChangedList.Add("PresetPT");
            }
            else
            {
                // Handles Member values changed
                ((INotifyPropertyChanged)pt).PropertyChanged
                                 += (o, arg) =>
                                 {
                                     if (!pt.PropertiesChangedList.Contains(arg.PropertyName))
                                     {
                                         pt.PropertiesChangedList.Add(arg.PropertyName);
                                     }
                                 };

                // Handles Agent values changed
                ((INotifyPropertyChanged)pt.Upline).PropertyChanged
                                 += (o, arg) =>
                                 {
                                     if (!((PTSetting)pt.Upline).PropertiesChangedList.Contains(arg.PropertyName))
                                     {
                                         ((PTSetting)pt.Upline).PropertiesChangedList.Add(arg.PropertyName);
                                     }
                                 };
            }

            // Agent values
            ((PTSetting)pt.Upline).PresetPT = model.UplinePtSetting.PresetPT;

            // Member values
            pt.Comm = model.PtSetting.Comm;

            var result = this.PtSetting.UpdateMemberPTSetting(context, pt);

            return result;
        }

        /// <summary>
        /// Update PT
        /// </summary>
        /// <param name="custId">Customer id</param>
        /// <param name="uplineId">Up-line id</param>
        /// <param name="customer">Customer JSON string</param>
        /// <returns>Result object</returns>
        protected Fanex.LiveCasinoSetting.Model.Result SuperUpdateMemberPT(int custId, int uplineId, string customer)
        {
            var pt = this.PtSetting.SuperGetMemberPTSetting(custId, uplineId);
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);

            if (this.IsDisableAccount(model.CustomerInfo.UserName, model.CustomerInfo.GroupName))
            {
                return null;
            }

            ((INotifyPropertyChanged)pt.Upline.Upline.Upline).PropertyChanged
                        += (o, arg) =>
                        {
                            if (!((SuperPTSetting)pt.Upline.Upline.Upline).PropertiesChangedList.Contains(arg.PropertyName))
                            {
                                ((SuperPTSetting)pt.Upline.Upline.Upline).PropertiesChangedList.Add(arg.PropertyName);
                            }
                        };

            ((SuperPTSetting)pt.Upline.Upline.Upline).PresetPT = model.SuperPtSetting.PresetPT;
            ((SuperPTSetting)pt.Upline.Upline.Upline).AutoPresetPT = model.SuperPtSetting.AutoPresetPT;
            ((SuperPTSetting)pt.Upline.Upline.Upline).IsAutoPresetPT = model.SuperPtSetting.IsAutoPresetPT;

            var result = this.PtSetting.UpdateMemberPTSettingBySuper(
                new RequestContext()
                {
                    CustId = custId,
                    SignUpIp = this.CurrentContext.Ip4Address,
                    Language = "en",
                    UplineId = uplineId,
                    UserId = this.CurrentSession.UserId
                },
                pt);

            return result;
        }

        /// <summary>
        /// Update PT
        /// </summary>
        /// <param name="custId">Customer id</param>
        /// <param name="uplineId">Up-line id</param>
        /// <param name="customer">Customer JSON string</param>
        /// <returns>Result object</returns>
        protected Fanex.LiveCasinoSetting.Model.Result MasterUpdateMemberPT(int custId, int uplineId, string customer)
        {
            var pt = this.PtSetting.MasterGetMemberPTSetting(custId, uplineId);
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);

            if (this.IsDisableAccount(model.CustomerInfo.UserName, model.CustomerInfo.GroupName))
            {                
                return null;
            }

            ((INotifyPropertyChanged)pt.Upline.Upline).PropertyChanged
                        += (o, arg) =>
                        {
                            if (!((SuperPTSetting)pt.Upline.Upline).PropertiesChangedList.Contains(arg.PropertyName))
                            {
                                ((SuperPTSetting)pt.Upline.Upline).PropertiesChangedList.Add(arg.PropertyName);
                            }
                        };

            ((PTSetting)pt.Upline.Upline).PresetPT = model.MasterPtSetting.PresetPT;
            ((PTSetting)pt.Upline.Upline).AutoPresetPT = model.MasterPtSetting.AutoPresetPT;
            ((PTSetting)pt.Upline.Upline).IsAutoPresetPT = model.MasterPtSetting.IsAutoPresetPT;

            var result = this.PtSetting.UpdateMemberPTSettingByMaster(
                new RequestContext()
                {
                    CustId = custId,
                    SignUpIp = this.CurrentContext.Ip4Address,
                    Language = "en",
                    UplineId = uplineId,
                    UserId = this.CurrentSession.UserId
                },
                pt);

            return result;
        }

        /// <summary>
        /// Gets insert account request
        /// </summary>
        /// <param name="custId">Customer id</param>
        /// <param name="uplineRoleId">Up-line role id</param>
        /// <param name="groupName">Group name</param>
        /// <returns>InsertAccountRequest object</returns>
        protected InsertAccountRequest GetInsertAccountRequest(int custId, int uplineRoleId, string groupName)
        {
            var custInfo = this.GetCustomerInfo(custId, groupName);
            var uplineInfo = this.PtSetting.GetCustomerInfo(uplineRoleId == RoleSuper ? custInfo.SuperId : uplineRoleId == RoleMaster ? custInfo.MasterId : custInfo.AgentId);

            var result = new InsertAccountRequest()
            {
                AgentId = custInfo.AgentId,
                CustId = custId,
                CustUserName = custInfo.Username,
                ExchangeId = uplineInfo.ExChangeId,
                SignUpIp = this.CurrentContext.Ip4Address,
                IsDisabledLiveCasino = uplineInfo.DisableLiveCasino,
                IsEnableDownline = true,
                Language = "en",
                MasterId = custInfo.MasterId,
                RoleId = custInfo.RoleId,
                SignUpForm = BaseController.FromSystem, // External acc system
                SuperId = custInfo.SuperId,
                SyncLiveCasino = uplineInfo.IsSyncLiveCasino,
                UserId = this.CurrentSession.UserId,
                UserName = this.CurrentSession.UserName,
                IsAutoInsert = false // Manually insert account
            };

            return result;
        }
    }
}