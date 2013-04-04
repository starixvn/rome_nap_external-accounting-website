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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
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
        /// Updates member win limit setting
        /// </summary>
        /// <param name="custId">Updated customer id</param>
        /// <param name="isDailyAutoReset">Is daily auto reset</param>
        /// <param name="maxWinLimit">Maximum win limit</param>
        /// <returns>ActionResult object</returns>
        protected Fanex.LiveCasinoSetting.Model.Result UpdateMemberWinLimit(int custId, bool isDailyAutoReset, decimal maxWinLimit)
        {
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
            var custInfo = this.GetCustomerInfo(custId, model.CustomerInfo.GroupName);

            var req = new InsertAccountRequest
            {
                AgentId = (int)custInfo["recommend"],
                MasterId = (int)custInfo["mrecommend"],
                SuperId = (int)custInfo["srecommend"],
                CustId = custId,
                RoleId = uplineRoleId - 1,
                IsEnableDownline = true,
                SignUpIp = this.CurrentContext.Ip4Address,
                SignUpForm = 3, // External accounting
                UserId = this.CurrentSession.UserId,
                CustUserName = model.CustomerInfo.UserName,
                UserName = this.CurrentSession.UserName,
                ExchangeId = model.CustomerInfo.ExChangeId,
                Language = "en"
            };

            if (oldGameTypes == null || oldGameTypes.Count == 0)
            {
                oldGameTypes = this.GameSetting.GetByCustomerId(uplineRoleId == 4 ? req.SuperId : uplineRoleId == 3 ? req.MasterId : req.AgentId);
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
        /// <returns>DataRow object</returns>
        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        protected DataRow GetCustomerInfo(int custId, string groupName)
        {
            var report = new ReportService();
            var custInfo = report.GetCustomerInfo(groupName, custId, string.Empty, true);
            var item = custInfo.Rows[0];

            return item;
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
                case 3:
                    return LogActions.UpdateStakeLevel_Master;
                case 2:
                    return LogActions.UpdateStakeLevel_Agent;
                case 1:
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
                case 1:
                    return LogActions.ManualInsert_Member;
                case 2:
                    return LogActions.ManualInsert_Agent;
                case 3:
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
                var userInfo = this.GetCustomerInfo(custId, model.CustomerInfo.GroupName);

                // Update game setting in case of not sync live casino
                if (isUpdateGameSetting && !custInfo.IsSyncLiveCasino)
                {
                    // Update PT first
                    if (uplineRoleId == 4)
                    {
                        // Update master PT
                        result = this.UpdateMasterPT(custId, uplineId, customer);
                    }
                    else if (uplineRoleId == 3)
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
                else if (!custInfo.IsSyncLiveCasino)
                {
                    // Sync live casino when updated PT
                    var req = new InsertAccountRequest
                    {
                        AgentId = (int)userInfo["recommend"],
                        MasterId = (int)userInfo["mrecommend"],
                        SuperId = (int)userInfo["srecommend"],
                        CustId = custId,
                        RoleId = uplineRoleId - 1,
                        IsEnableDownline = true,
                        SignUpIp = this.CurrentContext.Ip4Address,
                        SignUpForm = 3, // External accounting
                        UserId = this.CurrentSession.UserId,
                        CustUserName = model.CustomerInfo.UserName,
                        UserName = this.CurrentSession.UserName,
                        ExchangeId = model.CustomerInfo.ExChangeId,
                        Language = "en"
                    };

                    // Manually insert account first
                    result = this.GameSetting.InsertAccount(req);

                    // Insert account success then update game setting
                    if (string.IsNullOrWhiteSpace(result.ErrorCode) && gameTypes.Count > 0)
                    {
                        var oldGameTypes = this.GameSetting.GetByCustomerId(custId);

                        // Dont have game setting then get by upline
                        if (oldGameTypes == null || oldGameTypes.Count == 0)
                        {
                            oldGameTypes = this.GameSetting.GetByCustomerId(uplineId);
                        }

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
                            // Update sync status for acc
                            ((GameSettingService)this.GameSetting).UpdateSyncLiveCasinoInfo(req);

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

            if (!custInfo.IsSyncLiveCasino)
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

            if (!custInfo.IsSyncLiveCasino)
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

            if (!custInfo.IsSyncLiveCasino)
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
        /// <param name="customer">JSON object string</param>
        /// <returns>InsertAccountRequest object</returns>
        protected InsertAccountRequest GetInsertAccountRequest(int custId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var custInfo = this.GetCustomerInfo(custId, model.CustomerInfo.GroupName);

            var result = new InsertAccountRequest()
            {
                AgentId = (int)custInfo["recommend"],
                CustId = custId,
                CustUserName = custInfo["username"].ToString(),
                ExchangeId = model.CustomerInfo.ExChangeId,
                SignUpIp = this.CurrentContext.Ip4Address,
                IsDisabledLiveCasino = model.CustomerInfo.DisableLiveCasino,
                IsEnableDownline = false,
                Language = "en",
                MasterId = (int)custInfo["mrecommend"],
                RoleId = 1,
                SignUpForm = 3, // External acc system
                SuperId = (int)custInfo["srecommend"],
                SyncLiveCasino = model.CustomerInfo.IsSyncLiveCasino,
                UserId = CurrentSession.UserId,
                UserName = CurrentSession.UserName,
                IsAutoInsert = false // Manually insert account
            };

            return result;
        }
    }
}