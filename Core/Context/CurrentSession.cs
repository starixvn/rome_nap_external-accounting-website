// --------------------------------------------------------------------------
// <copyright file="CurrentSession.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// Session info for current logging in
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.14">Created</change>
// </history>
namespace Fanex.ExternalAccount.Core.Context
{
    using System.Web.UI;
    using Fanex.CustomerSetting.Module.Core.Web;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// Current Session.
    /// </summary>
    public class CurrentSession : Page, ICurrentSession
    {
        #region Constants
        
        public const string EASUsername = "EASUsername";
        public const string EASCustId = "EASCustId";
        public const string EASUserId = "EASUserId";
        public const string EASOnUserId = "EASOnUserId";
        public const string EASCaptcha = "EASCaptcha";
        public const string EASCurrencyGroup = "EASCurrencyGroup";
        public const string EASInternalAdmin = "EASInternalAdmin";
        public const string EASITAdmin = "EASITAdmin";
        public const string EASGroupId = "EASGroupId";
        
        private static string _captcha = string.Empty;
        private static ICurrentSession _currentSession = null;

        #endregion Constants

        public static ICurrentSession Instance
        {
            get
            {
                return _currentSession ?? new CurrentSession();
            }
        }

        public string UserName
        {
            get
            {
                return Utils.ToString(this.Session[EASUsername]);
            }

            set
            {
                this.Session[EASUsername] = value;
            }
        }

        public int CustId
        {
            get
            {
                return Utils.ToInt32(this.Session[EASCustId]);
            }

            set
            {
                this.Session[EASCustId] = value;
            }
        }

        public int UserId
        {
            get
            {
                return Utils.ToInt32(this.Session[EASUserId]);
            }

            set
            {
                this.Session[EASUserId] = value;
            }
        }

        public int OnUserId
        {
            get
            {
                return Utils.ToInt32(this.Session[EASOnUserId]);
            }

            set
            {
                this.Session[EASOnUserId] = value;
            }
        }

        public bool IsLogin
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.UserName);
            }

            set
            {
                this.Session["IsLogin"] = value;
            }
        }

        public string NetSessionId
        {
            get { return this.Session.SessionID; }
            set { this.Session["SessionID"] = value; }
        }

        public string Captcha
        {
            get
            {
                return _captcha;
            }

            set
            {
                _captcha = value;
                this.Session[EASCaptcha] = value;
            }
        }

        public string CurrencyGroup
        {
            get
            {
                return Utils.ToString(this.Session[EASCurrencyGroup]);
            }

            set
            {
                this.Session[EASCurrencyGroup] = value;
            }
        }

        public bool IsInternalAdmin
        {
            get
            {
                return Utils.ToBoolean(this.Session[EASInternalAdmin]);
            }

            set
            {
                this.Session[EASInternalAdmin] = value;
            }
        }

        public bool IsITAdmin
        {
            get
            {
                return Utils.ToBoolean(this.Session[EASITAdmin]);
            }

            set
            {
                this.Session[EASITAdmin] = value;
            }
        }

        public int GroupId
        {
            get
            {
                return Utils.ToInt32(this.Session[EASGroupId]);
            }

            set
            {
                this.Session[EASGroupId] = value;
            }
        }
    }
}