namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using Utilities;

    public class CurrentSession : ICurrentSession
    {
        public const string EASUsername = "EASUsername";
        public const string EASCustId = "EASCustId";
        public const string EASUserId = "EASUserId";
        public const string EASOnUserId = "EASOnUserId";
        public const string EASCaptcha = "EASCaptcha";
        public const string EASCurrencyGroup = "EASCurrencyGroup";
        public const string EASParentGroup = "EASParentGroup";
        public const string EASInvalidOTP = "EASInvalidOTP";
        public const string EASEnabledOTP = "EASEnabledOTP";
        public const string EASOTPFailedCounter = "EASOTPFailedCounter";
        public const string EASOTPEncryptCode = "EASOTPEncryptCode";
        public const string EASOTPSecretKey = "EASOTPSecretKey";
        public const string EASOTPModifiedDate = "EASOTPModifiedDate";
        public const string EASInternalAdmin = "EASInternalAdmin";
        public const string EASITAdmin = "EASITAdmin";
        public const string EASGroupId = "EASGroupId";
        public const string EASFromSystem = "EASFromSystem";
        public const string EASIsLogin = "EASIsLogin";
        public const string EASLoginAttempt = "_loginAttempt";
        public const string EASUserAttempt = "_userAttempt";
        public const string EASHashPwd = "EASHash";
        public const string EASLine = "EASLine";
        public const string EASSCLastActiveTime = "EASSCLastActiveTime";
        public const string EASSCFailedCounter = "EASSCFailedCounter";

        public static ICurrentSession Instance => new CurrentSession();

        public bool IsAuthenticated =>
            HttpContext.Current.User.Identity.IsAuthenticated
            && IsLogin;

        public bool HadNoSessionData => !IsLogin;

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public int LoginAttempt
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            get { return Utils.ToInt32(HttpContext.Current.Session[EASLoginAttempt]); }

            set { HttpContext.Current.Session[EASLoginAttempt] = value; }
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public string UserAttempt
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
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

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        public string ParentGroupName
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            get { return Utils.ToString(HttpContext.Current.Session[EASParentGroup]); }

            set { HttpContext.Current.Session[EASParentGroup] = value; }
        }

        public bool IsBelongToSpecialGroups
            => !IsInternalAdmin && !IsITAdmin && Config.SpecialGroups().Contains(ParentGroupName, StringComparer.CurrentCultureIgnoreCase);

        public bool InvalidOTP
        {
            get { return Utils.ToBoolean(HttpContext.Current.Session[EASInvalidOTP]); }

            set { HttpContext.Current.Session[EASInvalidOTP] = value; }
        }

        public string OTPEncryptCode
        {
            get { return Utils.ToString(HttpContext.Current.Session[EASOTPEncryptCode]); }

            set { HttpContext.Current.Session[EASOTPEncryptCode] = value; }
        }

        public int OTPFailedCounter
        {
            get { return Utils.ToInt32(HttpContext.Current.Session[EASOTPFailedCounter]); }

            set { HttpContext.Current.Session[EASOTPFailedCounter] = value; }
        }

        public bool EnabledOTP
        {
            get { return Utils.ToBoolean(HttpContext.Current.Session[EASEnabledOTP]); }

            set { HttpContext.Current.Session[EASEnabledOTP] = value; }
        }

        public string OTPSecretKey
        {
            get { return Utils.ToString(HttpContext.Current.Session[EASOTPSecretKey]); }

            set { HttpContext.Current.Session[EASOTPSecretKey] = value; }
        }

        public string OTPModifiedDate
        {
            get { return Utils.ToString(HttpContext.Current.Session[EASOTPModifiedDate]); }

            set { HttpContext.Current.Session[EASOTPModifiedDate] = value; }
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

        public string HashPassword
        {
            get { return HttpContext.Current.Session[EASHashPwd] as string; }
            set { HttpContext.Current.Session[EASHashPwd] = value; }
        }

        public string CurrentLine
        {
            get { return HttpContext.Current.Session[EASLine] as string; }
            set { HttpContext.Current.Session[EASLine] = value; }
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public double LastActiveTime
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            get => Utils.ToDouble(HttpContext.Current.Session[EASSCLastActiveTime]);
            set => HttpContext.Current.Session[EASSCLastActiveTime] = value;
        }

        public void UpdateLastActiveTime(object lastActiveTime  = null)
        {
            LastActiveTime = lastActiveTime == null
                ? Utils.ConvertDateTimeToMilliseconds(DateTime.Now)
                : Utils.ToDouble(lastActiveTime);
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public int SCFailedCounter
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            get => Utils.ToInt32(HttpContext.Current.Session[EASSCFailedCounter]);
            set => HttpContext.Current.Session[EASSCFailedCounter] = value;
        }

        public string LangKey { get; set; } = "E";
    }
}