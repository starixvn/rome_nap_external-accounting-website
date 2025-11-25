namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
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

        private static IHttpContextAccessor? _httpContextAccessor;

        public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private static HttpContext? HttpContext
        {
            get
            {
                if (_httpContextAccessor?.HttpContext != null)
                {
                    return _httpContextAccessor.HttpContext;
                }
                
                // Fallback: try to get from service locator if available
                try
                {
                    var serviceProvider = new ServiceCollection().BuildServiceProvider();
                    var accessor = serviceProvider.GetService<IHttpContextAccessor>();
                    return accessor?.HttpContext;
                }
                catch
                {
                    return null;
                }
            }
        }

        private static ISession? Session => HttpContext?.Session;

        public static ICurrentSession Instance => new CurrentSession();

        public bool IsAuthenticated =>
            HttpContext?.User?.Identity?.IsAuthenticated == true
            && IsLogin;

        public bool HadNoSessionData => !IsLogin;

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public int LoginAttempt
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            get { return Utils.ToInt32(GetSessionValue(EASLoginAttempt)); }

            set { SetSessionValue(EASLoginAttempt, value); }
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public string UserAttempt
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            get { return Utils.ToString(GetSessionValue(EASUserAttempt)); }

            set { SetSessionValue(EASUserAttempt, value); }
        }

        public string UserName
        {
            get { return Utils.ToString(GetSessionValue(EASUsername)); }

            set { SetSessionValue(EASUsername, value); }
        }

        public int CustId
        {
            get { return Utils.ToInt32(GetSessionValue(EASCustId)); }

            set { SetSessionValue(EASCustId, value); }
        }

        public int UserId
        {
            get { return Utils.ToInt32(GetSessionValue(EASUserId)); }

            set { SetSessionValue(EASUserId, value); }
        }

        public int OnUserId
        {
            get { return Utils.ToInt32(GetSessionValue(EASOnUserId)); }

            set { SetSessionValue(EASOnUserId, value); }
        }

        public bool IsLogin
        {
            get { return Utils.ToBoolean(GetSessionValue(EASIsLogin)); }

            set { SetSessionValue(EASIsLogin, value); }
        }

        public string NetSessionId
        {
            get { return HttpContext?.Session?.Id ?? string.Empty; }
            set { SetSessionValue("SessionID", value); }
        }

        public string Captcha
        {
            get { return GetSessionValue(EASCaptcha) as string ?? string.Empty; }

            set { SetSessionValue(EASCaptcha, value); }
        }

        public string CurrencyGroup
        {
            get { return Utils.ToString(GetSessionValue(EASCurrencyGroup)); }

            set { SetSessionValue(EASCurrencyGroup, value); }
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        public string ParentGroupName
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            get { return Utils.ToString(GetSessionValue(EASParentGroup)); }

            set { SetSessionValue(EASParentGroup, value); }
        }

        public bool IsBelongToSpecialGroups
            => !IsInternalAdmin && !IsITAdmin && Config.SpecialGroups().Any(g => g.Equals(ParentGroupName, StringComparison.CurrentCultureIgnoreCase));

        public bool InvalidOTP
        {
            get { return Utils.ToBoolean(GetSessionValue(EASInvalidOTP)); }

            set { SetSessionValue(EASInvalidOTP, value); }
        }

        public string OTPEncryptCode
        {
            get { return Utils.ToString(GetSessionValue(EASOTPEncryptCode)); }

            set { SetSessionValue(EASOTPEncryptCode, value); }
        }

        public int OTPFailedCounter
        {
            get { return Utils.ToInt32(GetSessionValue(EASOTPFailedCounter)); }

            set { SetSessionValue(EASOTPFailedCounter, value); }
        }

        public bool EnabledOTP
        {
            get { return Utils.ToBoolean(GetSessionValue(EASEnabledOTP)); }

            set { SetSessionValue(EASEnabledOTP, value); }
        }

        public string OTPSecretKey
        {
            get { return Utils.ToString(GetSessionValue(EASOTPSecretKey)); }

            set { SetSessionValue(EASOTPSecretKey, value); }
        }

        public string OTPModifiedDate
        {
            get { return Utils.ToString(GetSessionValue(EASOTPModifiedDate)); }

            set { SetSessionValue(EASOTPModifiedDate, value); }
        }

        public bool IsInternalAdmin
        {
            get { return Utils.ToBoolean(GetSessionValue(EASInternalAdmin)); }

            set { SetSessionValue(EASInternalAdmin, value); }
        }

        public bool IsITAdmin
        {
            get { return Utils.ToBoolean(GetSessionValue(EASITAdmin)); }

            set { SetSessionValue(EASITAdmin, value); }
        }

        public int GroupId
        {
            get { return Utils.ToInt32(GetSessionValue(EASGroupId)); }

            set { SetSessionValue(EASGroupId, value); }
        }

        public int FromSystem
        {
            get { return Utils.ToInt32(GetSessionValue(EASFromSystem)); }

            set { SetSessionValue(EASFromSystem, value); }
        }

        public string Language
        {
            get { return CultureInfo.CurrentCulture.Name; }

            set { }
        }

        public string HashPassword
        {
            get { return GetSessionValue(EASHashPwd) as string ?? string.Empty; }
            set { SetSessionValue(EASHashPwd, value); }
        }

        public string CurrentLine
        {
            get { return GetSessionValue(EASLine) as string ?? string.Empty; }
            set { SetSessionValue(EASLine, value); }
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public double LastActiveTime
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            get => Utils.ToDouble(GetSessionValue(EASSCLastActiveTime));
            set => SetSessionValue(EASSCLastActiveTime, value);
        }

        public void UpdateLastActiveTime(object lastActiveTime = null)
        {
            LastActiveTime = lastActiveTime == null
                ? Utils.ConvertDateTimeToMilliseconds(DateTime.Now)
                : Utils.ToDouble(lastActiveTime);
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public int SCFailedCounter
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            get => Utils.ToInt32(GetSessionValue(EASSCFailedCounter));
            set => SetSessionValue(EASSCFailedCounter, value);
        }

        public string LangKey { get; set; } = "E";       

        private static object? GetSessionValue(string key)
        {
            if (Session == null)
                return null;

            try
            {
                return Session.GetString(key);
            }
            catch
            {
                return null;
            }
        }

        private static void SetSessionValue(string key, object? value)
        {
            if (Session == null)
                return;

            try
            {
                if (value == null)
                {
                    Session.Remove(key);
                }
                else
                {
                    // Convert to string for storage (ASP.NET Core Session only stores strings)
                    Session.SetString(key, value.ToString() ?? string.Empty);
                }
            }
            catch
            {
                // Ignore session errors
            }
        }
    }
}
