namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Linq;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Impl.UserControl;

    public class ExternalAccountAuthorization
    {
        public bool IsBelong()
        {
            var currentContext = new CurrentContext();

            return IsBelong(currentContext.GroupName, currentContext.CustId, string.Empty, true);
        }

        public bool IsBelong(string groupname, int custId, string userName, bool isGetUplineInfo)
        {
            var currentSession = new CurrentSession();
            var loginService = new LoginService();
            var userInfo = new UserService().GetUserById(currentSession.UserId);

            if (currentSession.GroupId != userInfo.GroupID)
            {
                currentSession.IsInternalAdmin = loginService.CheckExternalGroup(currentSession.UserId, 11, "ExternalGroup");
                currentSession.GroupId = userInfo.GroupID;
                currentSession.CurrencyGroup = userInfo.GroupName;
                currentSession.IsITAdmin = currentSession.GroupId == 1;
            }

            if (!currentSession.IsInternalAdmin)
            {
                var groups = loginService.GroupList();
                var userGroup = groups.Where(o => o.GroupID == currentSession.GroupId);
                groupname = string.IsNullOrEmpty(groupname) ? userGroup.FirstOrDefault().GroupName : groupname;
                var result = new ReportService().GetCustomerInfo(groupname, custId, userName, isGetUplineInfo);

                return result != null && result.Rows.Count > 0;
            }

            return currentSession.IsInternalAdmin;
        }

    }
}
