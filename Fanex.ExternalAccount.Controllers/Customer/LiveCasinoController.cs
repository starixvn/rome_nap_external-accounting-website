// --------------------------------------------------------------------------
// <copyright file="LiveCasinoController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.3.19">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Fanex.LiveCasinoSetting.Model;
    using Fanex.LiveCasinoSetting.Request;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Controller for processing live casino setting
    /// </summary>
    public class LiveCasinoController : LiveCasinoBase
    {
        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditMaster()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditAgent()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditMember()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SuperEditMember()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult MasterEditMember()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditMultipleMasters()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditMultipleAgents()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditMultipleMembers()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SuperEditMultipleMembers()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult MasterEditMultipleMembers()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditMasterPost(int custId, int uplineId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var settingModel = this.PtSetting.GetMasterSetting(custId, uplineId);

            #region For testing render game setting

            if (settingModel.GameSetting.Count == 0)
            {
                settingModel.GameSetting.Add(new LiveCasinoSetting.Model.GameType
                {
                    BettingLimit = new System.Collections.Generic.List<LiveCasinoSetting.Model.StakeLevel>()
                    {
                        new Fanex.LiveCasinoSetting.Model.StakeLevel
                        {
                            Id = 1,
                            IsDirty = true,
                            IsSelected = true,
                            Text = "10-100"
                        },
                        new Fanex.LiveCasinoSetting.Model.StakeLevel
                        {
                            Id = 2,
                            IsDirty = true,
                            IsSelected = false,
                            Text = "100-1000"
                        }
                    },
                    BetTypeId = 1101,
                    Id = 1,
                    Name = "Baccarat"
                });

                settingModel.GameSetting.Add(new LiveCasinoSetting.Model.GameType
                {
                    BettingLimit = new System.Collections.Generic.List<LiveCasinoSetting.Model.StakeLevel>()
                    {
                        new Fanex.LiveCasinoSetting.Model.StakeLevel
                        {
                            Id = 3,
                            IsDirty = true,
                            IsSelected = true,
                            Text = "20-100"
                        },
                        new Fanex.LiveCasinoSetting.Model.StakeLevel
                        {
                            Id = 4,
                            IsDirty = true,
                            IsSelected = false,
                            Text = "500-1000"
                        }
                    },
                    BetTypeId = 1102,
                    Id = 1,
                    Name = "Roulette"
                });
            }

            #endregion For testing render game setting

            var liveCasinoModel = new LiveCasinoModel()
            {
                SuperPTSetting = new PTModel
                {
                    AutoPresetPT = settingModel.PTSetting.Upline.AutoPresetPT,
                    Comm = settingModel.PTSetting.Upline.Comm,
                    IsAutoPresetPT = settingModel.PTSetting.Upline.IsAutoPresetPT,
                    IsAutoPresetPTChanged = settingModel.PTSetting.Upline.IsAutoPresetPTChanged,
                    IsCommChanged = settingModel.PTSetting.Upline.IsCommChanged,
                    IsDirty = settingModel.PTSetting.Upline.IsDirty,
                    IsIsAutoPresetPTChanged = settingModel.PTSetting.Upline.IsIsAutoPresetPTChanged,
                    IsMaxPTChanged = settingModel.PTSetting.Upline.IsMaxPTChanged,
                    IsMinPTChanged = settingModel.PTSetting.Upline.IsMinPTChanged,
                    IsPresetPTChanged = settingModel.PTSetting.Upline.IsPresetPTChanged,
                    IsUpdateAllMember = settingModel.PTSetting.Upline.IsUpdateAllMember,
                    MaxAutoPresetPT = settingModel.PTSetting.Upline.MaxAutoPresetPT,
                    MaxPT = settingModel.PTSetting.Upline.MaxPT,
                    MinPT = settingModel.PTSetting.Upline.MinPT,
                    NewMemberPT = settingModel.PTSetting.Upline.NewMemberPT,
                    PresetPT = settingModel.PTSetting.Upline.PresetPT
                },
                CustomerInfo = new Fanex.ExternalAccount.ViewModel.Customer.CustomerInfo
                {
                    CustomerId = settingModel.CustomerInfo.CustomerId,
                    DisableLiveCasino = settingModel.CustomerInfo.DisableLiveCasino,
                    ExChangeId = settingModel.CustomerInfo.ExChangeId,
                    Fax = settingModel.CustomerInfo.Fax,
                    FirstName = settingModel.CustomerInfo.FirstName,
                    IsSyncLiveCasino = settingModel.CustomerInfo.IsSyncLiveCasino,
                    LastName = settingModel.CustomerInfo.LastName,
                    Limit = settingModel.CustomerInfo.Limit as MemberWinLimit,
                    Mobile = settingModel.CustomerInfo.Mobile,
                    NickName = settingModel.CustomerInfo.NickName,
                    Phone = settingModel.CustomerInfo.Phone,
                    RoleId = settingModel.CustomerInfo.RoleId,
                    Site = settingModel.CustomerInfo.Site,
                    UplineDisableLiveCasino = settingModel.CustomerInfo.UplineDisableLiveCasino,
                    UserName = settingModel.CustomerInfo.UserName
                },
                GameSetting = settingModel.GameSetting,
                MasterPTSetting = new PTModel
                {
                    AutoPresetPT = settingModel.PTSetting.AutoPresetPT,
                    Comm = settingModel.PTSetting.Comm,
                    IsAutoPresetPT = settingModel.PTSetting.IsAutoPresetPT,
                    IsAutoPresetPTChanged = settingModel.PTSetting.IsAutoPresetPTChanged,
                    IsCommChanged = settingModel.PTSetting.IsCommChanged,
                    IsDirty = settingModel.PTSetting.IsDirty,
                    IsIsAutoPresetPTChanged = settingModel.PTSetting.IsIsAutoPresetPTChanged,
                    IsMaxPTChanged = settingModel.PTSetting.IsMaxPTChanged,
                    IsMinPTChanged = settingModel.PTSetting.IsMinPTChanged,
                    IsPresetPTChanged = settingModel.PTSetting.IsPresetPTChanged,
                    IsUpdateAllMember = settingModel.PTSetting.IsUpdateAllMember,
                    IsValid = settingModel.PTSetting.IsValid,
                    MaxAutoPresetPT = settingModel.PTSetting.MaxAutoPresetPT,
                    MaxPT = settingModel.PTSetting.MaxPT,
                    MinPT = settingModel.PTSetting.MinPT,
                    NewMemberPT = settingModel.PTSetting.NewMemberPT,
                    PresetPT = settingModel.PTSetting.PresetPT
                },
                CustIds = new List<int>()
            };

            return this.JsonBase(liveCasinoModel, settings);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditAgentPost()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditMemberPost()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SuperEditMemberPost()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult MasterEditMemberPost()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditMultipleMastersPost()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditMultipleAgentsPost()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditMultipleMembersPost()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SuperEditMultipleMembersPost()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult MasterEditMultipleMembersPost()
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdateMasterPTPost(int custId, int uplineId, string customer)
        {
            var context = new RequestContext
            {
                CustId = custId,
                IsSyncLiveCasino = true,
                Language = "en-US",
                SignUpIp = this.CurrentContext.Ip4Address,
                UplineId = uplineId,
                UserId = this.CurrentSession.UserId
            };

            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var pt = this.PtSetting.GetMasterPTSeting(custId, uplineId);
            var isMultiple = model.IsMultiple;

            if (isMultiple || !model.CustomerInfo.IsSyncLiveCasino)
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
                //Handles Master values changed
                ((INotifyPropertyChanged)pt).PropertyChanged
                                 += (o, arg) =>
                                 {
                                     if (!pt.PropertiesChangedList.Contains(arg.PropertyName))
                                     {
                                         pt.PropertiesChangedList.Add(arg.PropertyName);
                                     }
                                 };

                //Handles Super values changed
                ((INotifyPropertyChanged)pt.Upline).PropertyChanged
                                 += (o, arg) =>
                                 {
                                     if (!((SuperPTSetting)pt.Upline).PropertiesChangedList.Contains(arg.PropertyName))
                                     {
                                         ((SuperPTSetting)pt.Upline).PropertiesChangedList.Add(arg.PropertyName);
                                     }
                                 };
            }

            //Super values
            ((SuperPTSetting)pt.Upline).PresetPT = model.SuperPTSetting.PresetPT;
            ((SuperPTSetting)pt.Upline).AutoPresetPT = model.SuperPTSetting.AutoPresetPT;
            ((SuperPTSetting)pt.Upline).IsAutoPresetPT = model.SuperPTSetting.IsAutoPresetPT;
            ((SuperPTSetting)pt.Upline).IsUpdateAllMember = model.SuperPTSetting.IsUpdateAllMember;
            ((SuperPTSetting)pt.Upline).NewMemberPT = model.SuperPTSetting.NewMemberPT;

            //Master values
            pt.MaxPT = model.MasterPTSetting.MaxPT;
            pt.MinPT = model.MasterPTSetting.MinPT;
            pt.Comm = model.MasterPTSetting.Comm;

            var result = this.PtSetting.UpdateMasterPTSetting(context, pt);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdateMasterWinLimit(int custId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);

            var memberWinLimit = new MemberWinLimit()
            {
                IsDailyAutoReset = model.CustomerInfo.Limit.IsDailyAutoReset,
                MaxWinLimit = model.CustomerInfo.Limit.MaxWinLimit
            };

            var context = new RequestContext()
            {
                CustId = custId,
                Language = "en-US",
                SignUpIp = this.CurrentContext.Ip4Address,
                UserId = this.CurrentSession.UserId
            };

            var result = this.MemberWinLimit.UpdateMemberWinLimit(memberWinLimit, context);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }
    }
}