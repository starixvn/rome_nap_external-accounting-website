namespace Fanex.ExternalAccount.Core.Reports.WinLossDetail
{
    using System;
    using System.Linq;
    using System.Web;
    using CustomerSetting.Module.Core.Web;

    public static class WinLossDetailHelper
    {
        public static bool IsHiddenProduct(ProductElementConfiguration product, ICurrentSession currentSession)
        {
            if (product.Hide)
            {
                return true;
            }

            if (product.ShowForGroups.Equals("*", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var groups = product.ShowForGroups.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

            var requestUrl = HttpContext.Current.Request.UrlReferrer?.AbsolutePath.Split(new[] {"/"},
                StringSplitOptions.RemoveEmptyEntries);

            var groupName = requestUrl != null && (requestUrl.Length == 0 || !currentSession.IsInternalAdmin)
                ? currentSession.CurrencyGroup
                : requestUrl?[requestUrl.Length - 1];

            if (string.IsNullOrWhiteSpace(groupName))
            {
                return true;
            }

            foreach (var group in groups)
            {
                if (groupName.Equals(group, StringComparison.OrdinalIgnoreCase)
                    || groupName.Equals("Finance", StringComparison.OrdinalIgnoreCase)
                    || groupName.Equals("Internal", StringComparison.OrdinalIgnoreCase)
                    || groupName.Equals("IT", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsShowProduct(string productId, ICurrentSession currentSession)
        {
            return !IsHiddenProduct(
                ProductsConfiguration.Instance.ProductCollection.First(n =>
                    n.Id.Equals(productId, StringComparison.OrdinalIgnoreCase)), currentSession);
        }
    }
}