namespace Fanex.ExternalAccount.Core.Reports.WinLossDetail
{
    using System;
    using System.Linq;
    using System.Web;
    using CustomerSetting.Module.Core.Web;
    using DTO.UserControl;

    public static class WinLossDetailHelper
    {
        private static string GetGroupName(ICurrentSession currentSession)
        {
            var requestUrl = HttpContext.Current.Request.UrlReferrer?.AbsolutePath.Split(new[] {"/"},
                StringSplitOptions.RemoveEmptyEntries);

            var groupName = requestUrl != null && (requestUrl.Length == 0 || !currentSession.IsInternalAdmin)
                ? currentSession.CurrencyGroup
                : requestUrl?[requestUrl.Length - 1];

            return groupName;
        }

        public static bool IsHiddenProduct(ProductElementConfiguration productConfiguration,
            ICurrentSession currentSession)
        {
            if (productConfiguration.Hide)
            {
                return true;
            }

            if (productConfiguration.ShowForAll())
            {
                return false;
            }

            var groupName = GetGroupName(currentSession);

            if (string.IsNullOrWhiteSpace(groupName))
            {
                return true;
            }

            foreach (var group in productConfiguration.ShowGroupList)
            {
                if (groupName.Equals(group, StringComparison.OrdinalIgnoreCase)
                    || Group.IsOfficeGroup(groupName)
                )
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsShowProduct(string productId, ICurrentSession currentSession)
            => !IsHiddenProduct(ProductsConfiguration.Instance.ProductCollection.First(n =>
                n.Id.Equals(productId, StringComparison.OrdinalIgnoreCase)), currentSession);
    }
}