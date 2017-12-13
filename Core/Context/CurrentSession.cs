namespace Fanex.ExternalAccount.Core.Context
{
    using System.Globalization;
    using System.Web;
    using CustomerSetting.Module.Core.Web;
    using Utilities;

    public class CurrentSession : ICurrentSession
    {
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
        public const string EASIsLogin = "EASIsLogin";
        public const string EASLoginAttempt = "_loginAttempt";
        public const string EASUserAttempt = "_userAttempt";

        public static ICurrentSession Instance => new CurrentSession();

        public bool IsAuthenticated =>
            HttpContext.Current.User.Identity.IsAuthenticated
            && IsLogin;

        public bool HadNoSessionData => !IsLogin;

        public int LoginAttempt
        {
            get { return Utils.ToInt32(HttpContext.Current.Session[EASLoginAttempt]); }

            set { HttpContext.Current.Session[EASLoginAttempt] = value; }
        }

        public string UserAttempt
        {
            get { return Utils.ToString(HttpContext.Current.Session[EASUserAttempt]); }

            set { HttpContext.Current.Session[EASUserAttempt] = value; }
        }

        public string UserName
        {
            get { return Utils.ToString(HttpContext.Current.Session[EASUsername]); }

            set { HttpContext.Current.Session[EASUsername] = value; }
        }

        public int CustId
        {
            get { return Utils.ToInt32(HttpContext.Current.Session[EASCustId]); }

            set { HttpContext.Current.Session[EASCustId] = value; }
        }

        public int UserId
        {
            get { return Utils.ToInt32(HttpContext.Current.Session[EASUserId]); }

            set { HttpContext.Current.Session[EASUserId] = value; }
        }

        public int OnUserId
        {
            get { return Utils.ToInt32(HttpContext.Current.Session[EASOnUserId]); }

            set { HttpContext.Current.Session[EASOnUserId] = value; }
        }

        public bool IsLogin
        {
            get { return Utils.ToBoolean(HttpContext.Current.Session[EASIsLogin]); }

            set { HttpContext.Current.Session[EASIsLogin] = value; }
        }

        public string NetSessionId
        {
            get { return HttpContext.Current.Session.SessionID; }
            set { HttpContext.Current.Session["SessionID"] = value; }
        }

        public string Captcha
        {
            get { return HttpContext.Current.Session[EASCaptcha] as string; }

            set { HttpContext.Current.Session[EASCaptcha] = value; }
        }

        public string CurrencyGroup
        {
            get { return Utils.ToString(HttpContext.Current.Session[EASCurrencyGroup]); }

            set { HttpContext.Current.Session[EASCurrencyGroup] = value; }
        }

        public bool IsInternalAdmin
        {
            get { return Utils.ToBoolean(HttpContext.Current.Session[EASInternalAdmin]); }

            set { HttpContext.Current.Session[EASInternalAdmin] = value; }
        }

        public bool IsITAdmin
        {
            get { return Utils.ToBoolean(HttpContext.Current.Session[EASITAdmin]); }

            set { HttpContext.Current.Session[EASITAdmin] = value; }
        }

        public int GroupId
        {
            get { return Utils.ToInt32(HttpContext.Current.Session[EASGroupId]); }

            set { HttpContext.Current.Session[EASGroupId] = value; }
        }

        public int FromSystem
        {
            get { return Utils.ToInt32(HttpContext.Current.Session[EASFromSystem]); }

            set { HttpContext.Current.Session[EASFromSystem] = value; }
        }

        public string Language
        {
            get { return CultureInfo.CurrentCulture.Name; }

            set { }
        }
    }
}