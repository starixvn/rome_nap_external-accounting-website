// --------------------------------------------------------------------------
// <copyright file="CurrentSession.cs" company="">
// Copyright (c) . All rights reserved.
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
    using System.Globalization;
    using System.Web;
    using System.Web.UI;
    using Fanex.CustomerSetting.Module.Core.Web;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// Current Session.
    /// </summary>
    public class CurrentSession : ICurrentSession
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
        public const string EASFromSystem = "EASFromSystem";

        private static string _captcha = string.Empty;        
        private static string _userAttempt = string.Empty;

        #endregion Constants

        public static ICurrentSession Instance
        {
            get
            {
                return new CurrentSession();
            }
        }

        public string UserName
        {
            get
            {
                return Utils.ToString(HttpContext.Current.Session[EASUsername]);
            }

            set
            {
                HttpContext.Current.Session[EASUsername] = value;
            }
        }

        public int CustId
        {
            get
            {
                return Utils.ToInt32(HttpContext.Current.Session[EASCustId]);
            }

            set
            {
                HttpContext.Current.Session[EASCustId] = value;
            }
        }

        public int UserId
        {
            get
            {
                return Utils.ToInt32(HttpContext.Current.Session[EASUserId]);
            }

            set
            {
                HttpContext.Current.Session[EASUserId] = value;
            }
        }

        public int OnUserId
        {
            get
            {
                return Utils.ToInt32(HttpContext.Current.Session[EASOnUserId]);
            }

            set
            {
                HttpContext.Current.Session[EASOnUserId] = value;
            }
        }

        public bool IsLogin
        {
            get
            {
                return HttpContext.Current.User.Identity.IsAuthenticated;
            }

            set
            {
                HttpContext.Current.Session["IsLogin"] = value;
            }
        }

        public string NetSessionId
        {
            get { return HttpContext.Current.Session.SessionID; }
            set { HttpContext.Current.Session["SessionID"] = value; }
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
            }
        }

        public string CurrencyGroup
        {
            get
            {
                return Utils.ToString(HttpContext.Current.Session[EASCurrencyGroup]);
            }

            set
            {
                HttpContext.Current.Session[EASCurrencyGroup] = value;
            }
        }

        public bool IsInternalAdmin
        {
            get
            {
                return Utils.ToBoolean(HttpContext.Current.Session[EASInternalAdmin]);
            }

            set
            {
                HttpContext.Current.Session[EASInternalAdmin] = value;
            }
        }

        public bool IsITAdmin
        {
            get
            {
                return Utils.ToBoolean(HttpContext.Current.Session[EASITAdmin]);
            }

            set
            {
                HttpContext.Current.Session[EASITAdmin] = value;
            }
        }

        public int GroupId
        {
            get
            {
                return Utils.ToInt32(HttpContext.Current.Session[EASGroupId]);
            }

            set
            {
                HttpContext.Current.Session[EASGroupId] = value;
            }
        }

        public int FromSystem 
        {
            get
            {
                return Utils.ToInt32(HttpContext.Current.Session[EASFromSystem]);
            }

            set
            {
                HttpContext.Current.Session[EASFromSystem] = value;
            }
        }

        public string Language
        {
            get
            {
                return CultureInfo.CurrentCulture.Name;
            }

            set
            {
                value = "en-US";
            }
        }

        public int LoginAttempt
        {
            get
            {
                return Utils.ToInt32(HttpContext.Current.Session["_loginAttempt"]);
            }

            set
            {
                HttpContext.Current.Session["_loginAttempt"] = value;
            }
        }

        public string UserAttempt
        {
            get
            {
                return Utils.ToString(HttpContext.Current.Session["_userAttempt"]);
            }

            set
            {
                HttpContext.Current.Session["_userAttempt"] = value;
            }
        }
    }
}