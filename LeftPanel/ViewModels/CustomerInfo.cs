// --------------------------------------------------------------------------
// <copyright file="CustomerModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.07.25">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// TODO: Model for left panel.
    /// </summary>
    public class CustomerModel
    {
        public CustomerModel()
        {
            this.BalanceInfo = new BalanceInfo();
            this.DownlineList = new DownlineList();
            this.Login = new List<ObjectInfo>();
            this.OnlineList = new OnlineList();
            this.PersonalInfo = new PersonalInfo();
            this.Setting = new Setting();
        }

        public PersonalInfo PersonalInfo { get; set; }

        public BalanceInfo BalanceInfo { get; set; }

        public DownlineList DownlineList { get; set; }

        public Setting Setting { get; set; }

        public List<ObjectInfo> Login { get; set; }

        public OnlineList OnlineList { get; set; }
    }
    
    public class PersonalInfo : DownlineList
    {
        public PersonalInfo() : base()
        {            
        }

        public ObjectInfo CurrentLevel { get; set; }

        public ObjectInfo Name { get; set; }

        public ObjectInfo Nickname { get; set; }

        public ObjectInfo Phone { get; set; }

        public ObjectInfo Mobile { get; set; }

        public ObjectInfo Fax { get; set; }
    }

    public class BalanceInfo
    {
        public BalanceInfo()
        {
            this.Balances = new List<ObjectInfo>();
            this.Statuses = new List<Status>();
        }

        public ObjectInfo CurrencyId { get; set; }

        public ObjectInfo Currency { get; set; }

        public List<ObjectInfo> Balances { get; set; }

        public List<Status> Statuses { get; set; }
    }

    public class Status
    {
        public int TotalActive { get; set; }

        public int TotalClosed { get; set; }

        public int TotalSuspended { get; set; }

        public string Text { get; set; }

        public string Tooltip { get; set; }
    }

    public class DownlineList : BasicInfo
    {
        public DownlineList()
        {
            this.Downlines = new List<ObjectInfo>();
            this.Pages = new List<ObjectInfo>();
            this.CurrentPage = 1;
        }

        public List<ObjectInfo> Downlines { get; set; }

        public List<ObjectInfo> Pages { get; set; }

        public int CurrentPage { get; set; }
    }

    public class Setting
    {
        public Setting()
        {
            this.EnableItems = new List<ObjectInfo>();
            this.EditItems = new List<ObjectInfo>();
            this.SyncItems = new List<ObjectInfo>();
            this.Details = new List<ObjectInfo>();
        }

        public List<ObjectInfo> EnableItems { get; set; }

        public List<ObjectInfo> EditItems { get; set; }

        public List<ObjectInfo> SyncItems { get; set; }

        public List<ObjectInfo> Details { get; set; }
    }
        
    public class Login : OnlineInfo
    {
        public ObjectInfo Site { get; set; }

        public ObjectInfo Password { get; set; }

        public ObjectInfo LoginOption { get; set; }

        public ObjectInfo SecurityCode { get; set; }

        public ObjectInfo CreatedDate { get; set; }

        public ObjectInfo LastTrans { get; set; }
    }

    public class OnlineList
    {
        public OnlineList()
        {
            this.Onlines = new List<OnlineInfo>();
        }

        public List<OnlineInfo> Onlines { get; set; }
    }

    public class OnlineInfo
    {
        public ObjectInfo IP { get; set; }

        public ObjectInfo LoginTime { get; set; }
    }

    public class BasicInfo
    {
        public ObjectInfo CustId { get; set; }

        public ObjectInfo UplineId { get; set; }

        public ObjectInfo RoleId { get; set; }

        public ObjectInfo UplineRoleId { get; set; }
    }

    public class ObjectInfo
    {
        /// <summary>
        /// Gets or sets key of a field
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets value for the title of a field
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets title for a field
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets tootltip for title if it's content is too long
        /// </summary>
        public string Tooltip { get; set; }
    }
}