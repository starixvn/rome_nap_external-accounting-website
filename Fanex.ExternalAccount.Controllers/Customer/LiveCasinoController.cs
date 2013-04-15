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
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.ViewModel.Customer;
    using Fanex.LiveCasinoSetting.Model;
    using Fanex.LiveCasinoSetting.Request;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using RemotingRef;

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
        public ActionResult EditMasterPost(int custId, int uplineId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var settingModel = this.PtSetting.GetMasterSetting(custId, uplineId);

            var gameSetting = settingModel.GameSetting;

            foreach (var item in gameSetting)
            {
                item.Name = RefData.GetBetTypeName(item.BetTypeId.ToString(), "en", string.Empty);
            }

            var liveCasinoModel = new LiveCasinoModel()
            {
                UplinePtSetting = new PTModel
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
                GameSetting = gameSetting,
                PtSetting = new PTModel
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
                }
            };

            return this.JsonBase(liveCasinoModel, settings);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditAgentPost(int custId, int uplineId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var settingModel = this.PtSetting.GetAgentSetting(custId, uplineId);

            var gameSetting = settingModel.GameSetting;

            foreach (var item in gameSetting)
            {
                item.Name = RefData.GetBetTypeName(item.BetTypeId.ToString(), "en", string.Empty);
            }

            var liveCasinoModel = new LiveCasinoModel()
            {
                UplinePtSetting = new PTModel
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
                GameSetting = gameSetting,
                PtSetting = new PTModel
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
                }
            };

            return this.JsonBase(liveCasinoModel, settings);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditMemberPost(int custId, int uplineId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var settingModel = this.PtSetting.GetMemberSetting(custId, uplineId);

            var gameSetting = settingModel.GameSetting;

            foreach (var item in gameSetting)
            {
                item.Name = RefData.GetBetTypeName(item.BetTypeId.ToString(), "en", string.Empty);
            }

            var liveCasinoModel = new LiveCasinoModel()
            {
                UplinePtSetting = new PTModel
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
                GameSetting = gameSetting,
                PtSetting = new PTModel
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
                }
            };

            return this.JsonBase(liveCasinoModel, settings);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult SuperEditMemberPost(int custId, int uplineId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var settingModel = this.PtSetting.SuperGetMemberSetting(custId, uplineId);

            var gameSetting = settingModel.GameSetting;

            foreach (var item in gameSetting)
            {
                item.Name = RefData.GetBetTypeName(item.BetTypeId.ToString(), "en", string.Empty);
            }

            var liveCasinoModel = new LiveCasinoModel()
            {
                SuperPtSetting = new PTModel
                {
                    AutoPresetPT = settingModel.PTSetting.Upline.Upline.Upline.AutoPresetPT,
                    Comm = settingModel.PTSetting.Upline.Upline.Upline.Comm,
                    IsAutoPresetPT = settingModel.PTSetting.Upline.Upline.Upline.IsAutoPresetPT,
                    IsAutoPresetPTChanged = settingModel.PTSetting.Upline.Upline.Upline.IsAutoPresetPTChanged,
                    IsCommChanged = settingModel.PTSetting.Upline.Upline.Upline.IsCommChanged,
                    IsDirty = settingModel.PTSetting.Upline.Upline.Upline.IsDirty,
                    IsIsAutoPresetPTChanged = settingModel.PTSetting.Upline.Upline.Upline.IsIsAutoPresetPTChanged,
                    IsMaxPTChanged = settingModel.PTSetting.Upline.Upline.Upline.IsMaxPTChanged,
                    IsMinPTChanged = settingModel.PTSetting.Upline.Upline.Upline.IsMinPTChanged,
                    IsPresetPTChanged = settingModel.PTSetting.Upline.Upline.Upline.IsPresetPTChanged,
                    IsUpdateAllMember = settingModel.PTSetting.Upline.Upline.Upline.IsUpdateAllMember,
                    MaxAutoPresetPT = settingModel.PTSetting.Upline.Upline.Upline.MaxAutoPresetPT,
                    MaxPT = settingModel.PTSetting.Upline.Upline.Upline.MaxPT,
                    MinPT = settingModel.PTSetting.Upline.Upline.Upline.MinPT,
                    NewMemberPT = settingModel.PTSetting.Upline.Upline.Upline.NewMemberPT,
                    PresetPT = settingModel.PTSetting.Upline.Upline.Upline.PresetPT
                },
                MasterPtSetting = new PTModel
                {
                    AutoPresetPT = settingModel.PTSetting.Upline.Upline.AutoPresetPT,
                    Comm = settingModel.PTSetting.Upline.Upline.Comm,
                    IsAutoPresetPT = settingModel.PTSetting.Upline.Upline.IsAutoPresetPT,
                    IsAutoPresetPTChanged = settingModel.PTSetting.Upline.Upline.IsAutoPresetPTChanged,
                    IsCommChanged = settingModel.PTSetting.Upline.Upline.IsCommChanged,
                    IsDirty = settingModel.PTSetting.Upline.Upline.IsDirty,
                    IsIsAutoPresetPTChanged = settingModel.PTSetting.Upline.Upline.IsIsAutoPresetPTChanged,
                    IsMaxPTChanged = settingModel.PTSetting.Upline.Upline.IsMaxPTChanged,
                    IsMinPTChanged = settingModel.PTSetting.Upline.Upline.IsMinPTChanged,
                    IsPresetPTChanged = settingModel.PTSetting.Upline.Upline.IsPresetPTChanged,
                    IsUpdateAllMember = settingModel.PTSetting.Upline.Upline.IsUpdateAllMember,
                    MaxAutoPresetPT = settingModel.PTSetting.Upline.Upline.MaxAutoPresetPT,
                    MaxPT = settingModel.PTSetting.Upline.Upline.MaxPT,
                    MinPT = settingModel.PTSetting.Upline.Upline.MinPT,
                    NewMemberPT = settingModel.PTSetting.Upline.Upline.NewMemberPT,
                    PresetPT = settingModel.PTSetting.Upline.Upline.PresetPT
                },
                UplinePtSetting = new PTModel
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
                GameSetting = gameSetting,
                PtSetting = new PTModel
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
                }
            };

            return this.JsonBase(liveCasinoModel, settings);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult MasterEditMemberPost(int custId, int uplineId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var settingModel = this.PtSetting.MasterGetMemberSetting(custId, uplineId);

            var gameSetting = settingModel.GameSetting;

            foreach (var item in gameSetting)
            {
                item.Name = RefData.GetBetTypeName(item.BetTypeId.ToString(), "en", string.Empty);
            }

            var liveCasinoModel = new LiveCasinoModel()
            {                
                MasterPtSetting = new PTModel
                {
                    AutoPresetPT = settingModel.PTSetting.Upline.Upline.AutoPresetPT,
                    Comm = settingModel.PTSetting.Upline.Upline.Comm,
                    IsAutoPresetPT = settingModel.PTSetting.Upline.Upline.IsAutoPresetPT,
                    IsAutoPresetPTChanged = settingModel.PTSetting.Upline.Upline.IsAutoPresetPTChanged,
                    IsCommChanged = settingModel.PTSetting.Upline.Upline.IsCommChanged,
                    IsDirty = settingModel.PTSetting.Upline.Upline.IsDirty,
                    IsIsAutoPresetPTChanged = settingModel.PTSetting.Upline.Upline.IsIsAutoPresetPTChanged,
                    IsMaxPTChanged = settingModel.PTSetting.Upline.Upline.IsMaxPTChanged,
                    IsMinPTChanged = settingModel.PTSetting.Upline.Upline.IsMinPTChanged,
                    IsPresetPTChanged = settingModel.PTSetting.Upline.Upline.IsPresetPTChanged,
                    IsUpdateAllMember = settingModel.PTSetting.Upline.Upline.IsUpdateAllMember,
                    MaxAutoPresetPT = settingModel.PTSetting.Upline.Upline.MaxAutoPresetPT,
                    MaxPT = settingModel.PTSetting.Upline.Upline.MaxPT,
                    MinPT = settingModel.PTSetting.Upline.Upline.MinPT,
                    NewMemberPT = settingModel.PTSetting.Upline.Upline.NewMemberPT,
                    PresetPT = settingModel.PTSetting.Upline.Upline.PresetPT
                },
                UplinePtSetting = new PTModel
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
                GameSetting = gameSetting,
                PtSetting = new PTModel
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
                }
            };

            return this.JsonBase(liveCasinoModel, settings);
        }       

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdateMasterPTPost(int custId, int uplineId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var result = this.UpdateMasterPT(custId, uplineId, customer);

            // Process manually sync
            this.SyncManually(LiveCasinoBase.RoleSuper, uplineId, custId, customer, model.CustomerInfo.GroupName);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdateAgentPTPost(int custId, int uplineId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var result = this.UpdateAgentPT(custId, uplineId, customer);

            // Process manually sync
            this.SyncManually(LiveCasinoBase.RoleMaster, uplineId, custId, customer, model.CustomerInfo.GroupName);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdateMemberPTPost(int custId, int uplineId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var result = this.UpdateMemberPT(custId, uplineId, customer);

            // Process manually sync
            this.SyncManually(LiveCasinoBase.RoleAgent, uplineId, custId, customer, model.CustomerInfo.GroupName);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult SuperUpdateMemberPTPost(int custId, int uplineId, string customer)
        {
            var result = this.SuperUpdateMemberPT(custId, uplineId, customer);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult MasterUpdateMemberPTPost(int custId, int uplineId, string customer)
        {
            var result = this.MasterUpdateMemberPT(custId, uplineId, customer);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdateMasterLiveCasinoPost(int custId, int uplineId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var result = this.UpdateMasterPT(custId, uplineId, customer);

            // Process manually sync
            this.SyncManually(LiveCasinoBase.RoleSuper, uplineId, custId, customer, model.CustomerInfo.GroupName);

            var resultMWL = this.UpdateMemberWinLimit(custId, model.CustomerInfo.Limit.IsDailyAutoReset, model.CustomerInfo.Limit.MaxWinLimit);
            var resultGS = this.UpdateGameSetting(LiveCasinoBase.RoleSuper, custId, customer);

            var results = new List<object>();

            results.Add(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
            results.Add(new { ErrorCode = resultMWL.ErrorCode, ErrorMessage = resultMWL.ErrorMessage });
            results.Add(resultGS);

            return this.JsonBase(results);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdateAgentLiveCasinoPost(int custId, int uplineId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var result = this.UpdateAgentPT(custId, uplineId, customer);

            // Process manually sync
            this.SyncManually(LiveCasinoBase.RoleMaster, uplineId, custId, customer, model.CustomerInfo.GroupName);

            var resultMWL = this.UpdateMemberWinLimit(custId, model.CustomerInfo.Limit.IsDailyAutoReset, model.CustomerInfo.Limit.MaxWinLimit);
            var resultGS = this.UpdateGameSetting(LiveCasinoBase.RoleMaster, custId, customer);

            var results = new List<object>();

            results.Add(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
            results.Add(new { ErrorCode = resultMWL.ErrorCode, ErrorMessage = resultMWL.ErrorMessage });
            results.Add(resultGS);

            return this.JsonBase(results);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdateMemberLiveCasinoPost(int custId, int uplineId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var result = this.UpdateMemberPT(custId, uplineId, customer);

            // Process manually sync
            this.SyncManually(LiveCasinoBase.RoleAgent, uplineId, custId, customer, model.CustomerInfo.GroupName);

            var resultMWL = this.UpdateMemberWinLimit(custId, model.CustomerInfo.Limit.IsDailyAutoReset, model.CustomerInfo.Limit.MaxWinLimit);
            var resultGS = this.UpdateGameSetting(LiveCasinoBase.RoleAgent, custId, customer);
            var resultStatus = this.PtSetting.UpdateStatusLiveCasino(new UpdateLiveCasinoStatusRequest(this.GetInsertAccountRequest(custId, RoleAgent, model.CustomerInfo.GroupName)));

            var results = new List<object>();

            results.Add(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
            results.Add(new { ErrorCode = resultMWL.ErrorCode, ErrorMessage = resultMWL.ErrorMessage });
            results.Add(resultGS);
            results.Add(new { ErrorCode = resultStatus.ErrorCode, ErrorMessage = resultStatus.ErrorMessage });

            return this.JsonBase(results);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult SuperUpdateMemberLiveCasinoPost(int custId, int uplineId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var result = this.SuperUpdateMemberPT(custId, uplineId, customer);
            var resultMWL = this.UpdateMemberWinLimit(custId, model.CustomerInfo.Limit.IsDailyAutoReset, model.CustomerInfo.Limit.MaxWinLimit);
            var resultGS = this.UpdateGameSetting(LiveCasinoBase.RoleSuper, custId, customer);

            var results = new List<object>();

            results.Add(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
            results.Add(new { ErrorCode = resultMWL.ErrorCode, ErrorMessage = resultMWL.ErrorMessage });
            results.Add(resultGS);

            return this.JsonBase(results);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult MasterUpdateMemberLiveCasinoPost(int custId, int uplineId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var result = this.MasterUpdateMemberPT(custId, uplineId, customer);
            var resultMWL = this.UpdateMemberWinLimit(custId, model.CustomerInfo.Limit.IsDailyAutoReset, model.CustomerInfo.Limit.MaxWinLimit);
            var resultGS = this.UpdateGameSetting(LiveCasinoBase.RoleMaster, custId, customer);

            var results = new List<object>();

            results.Add(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
            results.Add(new { ErrorCode = resultMWL.ErrorCode, ErrorMessage = resultMWL.ErrorMessage });
            results.Add(resultGS);

            return this.JsonBase(results);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdateMemberWinLimitPost(int uplineId, int custId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);
            var result = this.UpdateMemberWinLimit(custId, model.CustomerInfo.Limit.IsDailyAutoReset, model.CustomerInfo.Limit.MaxWinLimit);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult UpdateGameSettingPost(int uplineRoleId, int uplineId, int custId, string customer)
        {
            var model = JsonConvert.DeserializeObject<LiveCasinoModel>(customer);

            // Process manually sync
            this.SyncManually(uplineRoleId, uplineId, custId, customer, model.CustomerInfo.GroupName, true);

            var result = this.UpdateGameSetting(uplineRoleId, custId, customer);

            return this.JsonBase(result);
        }
    }
}