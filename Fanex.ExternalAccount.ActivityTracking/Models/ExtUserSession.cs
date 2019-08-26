namespace Fanex.ExternalAccount.ActivityTracking.Models
{
    using System.Web;
    using Fanex.ExternalAccount.Core.Context;

    public class ExtUserSession : Fanex.ActivityTracking.Models.IUserSession
    {
        public int AdminId => CurrentSession.Instance.UserId;

        public string AdminName => CurrentSession.Instance.UserName;

        public int CustId => CurrentSession.Instance.CustId;

        public string CustName => CurrentSession.Instance.UserName;

        public string SessionID => HttpContext.Current?.Session.SessionID;

        public int OnUserId => CurrentSession.Instance.OnUserId;

        public string Language => CurrentSession.Instance.Language;

        public int SubAccId => CurrentSession.Instance.CustId;

        public string SubUsername => CurrentSession.Instance.UserName;

        public string CurrencyName => CurrentSession.Instance.CurrencyGroup;
    }
}
