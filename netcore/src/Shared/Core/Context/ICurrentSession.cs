namespace Fanex.ExternalAccount.Core.Context
{
    public interface ICurrentSession
    {
        string UserName { get; set; }

        int CustId { get; set; }

        int UserId { get; set; }

        int GroupId { get; set; }

        bool IsLogin { get; set; }

        int OnUserId { get; set; }

        string NetSessionId { get; set; }

        string Captcha { get; set; }

        string CurrencyGroup { get; set; }

        bool IsInternalAdmin { get; set; }

        bool IsITAdmin { get; set; }

        string Language { get; set; }

        int FromSystem { get; set; }

        bool IsBelongToSpecialGroups { get; }

        string LangKey { get; set; }

        string ParentGroupName { get; set; }

        string OTPModifiedDate { get; set; }

        string OTPEncryptCode { get; set; }

        bool EnabledOTP { get; set; }

        int LoginAttempt { get; set; }

        bool InvalidOTP { get; set; }

        bool IsAuthenticated { get; }

        string HashPassword { get; set; }

        string CurrentLine { get; set; }

        double LastActiveTime { get; set; }

        int SCFailedCounter { get; set; }
        
        string UserAttempt { get; set; }

        void UpdateLastActiveTime(object lastActiveTime = null);

        int OTPFailedCounter { get; set; }

        string OTPSecretKey { get; set; }
    }
}
