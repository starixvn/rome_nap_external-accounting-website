namespace Fanex.ExternalAccount.Core.Configurations.SecurityCode
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Configuration;

    public class SecurityCodeManager
    {
        private readonly SecurityCodeSection _securityCodeSection;

        public SecurityCodeManager(IConfiguration configuration)
        {
            _securityCodeSection = new SecurityCodeSection(configuration);
        }

        public string[] GetActions(bool isIgnored)
        {
            return (
                from ActionFilterElement element
                in _securityCodeSection.ActionFilterCollection
                where element.IsIgnored == isIgnored
                select element.Action).ToArray();
        }

        public string[] GetFilteredUsers()
        {
            return (
                from UserCodeElement element
                in _securityCodeSection.UserCodeCollection
                select element.UserName).ToArray();
        }

        public string GetSecurityCode(string userName)
        {
            return (
                from UserCodeElement element in _securityCodeSection.UserCodeCollection
                where element.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                select element)
                .FirstOrDefault()?.SecurityCode.Trim();
        }

        public string GetAliveTime => _securityCodeSection.ScAliveTime;

        public int GetMaxAllowAttempt => _securityCodeSection.ScMaxAllowAttempt;
    }
}