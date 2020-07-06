using System;
using System.Configuration;
using System.Linq;

namespace Fanex.ExternalAccount.Core.Configurations.SecurityCode
{
    public static class SecurityCodeManager
    {
        private static SecurityCodeSection _securityCodeSection =
            ConfigurationManager.GetSection("SecurityCodeSection") as SecurityCodeSection;

        public static string[] GetFilteredActions()
        {
            return (
                from ActionFilterElement element
                in _securityCodeSection.ActionFilterCollection
                select element).Select(e => e.Action).ToArray();
        }

        public static string GetSecurityCode(string userName)
        {
            return (
                from UserCodeElement element in _securityCodeSection.UserCodeCollection
                where element.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                select element)
                .FirstOrDefault()?.SecurityCode.Trim();
        }

        public static string GetIdleTime => ConfigurationManager.AppSettings["SCFilter:IdleTime"];
    }
}