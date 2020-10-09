namespace Fanex.ExternalAccount.Core.Configurations.SecurityCode
{
    using System;
    using System.Configuration;
    using System.Linq;

    public static class SecurityCodeManager
    {
        private static SecurityCodeSection securityCodeSection =
            ConfigurationManager.GetSection("SecurityCodeSection") as SecurityCodeSection;

        public static string[] GetActions(bool isIgnored)
        {
            return (
                from ActionFilterElement element
                in securityCodeSection.ActionFilterCollection
                where element.IsIgnored == isIgnored
                select element.Action).ToArray();
        }

        public static string[] GetFilteredUsers()
        {
            return (
                from UserCodeElement element
                in securityCodeSection.UserCodeCollection
                select element.UserName).ToArray();
        }

        public static string GetSecurityCode(string userName)
        {
            return (
                from UserCodeElement element in securityCodeSection.UserCodeCollection
                where element.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                select element)
                .FirstOrDefault()?.SecurityCode.Trim();
        }

        public static string GetAliveTime => securityCodeSection.ScAliveTime;

        public static int GetMaxAllowAttempt => securityCodeSection.ScMaxAllowAttempt;
    }
}