using System;
using System.Linq;
using System.Web;
using Fanex.CustomerSetting.Module.Core.Web;

namespace Fanex.ExternalAccount.Core.Reports.WinLossDetail
{
    public class WinLossDetailHelper
    {
        public static bool IsHiddenProduct(ProductElementConfiguration product, ICurrentSession currentSession)
        {
            if (product.Hide)
                return true;

            if (product.ShowForGroups.Equals("*", StringComparison.OrdinalIgnoreCase))
                return false;

            var groups = product.ShowForGroups.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var requestURL = HttpContext.Current.Request != null ? HttpContext.Current.Request.UrlReferrer.AbsolutePath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
            var groupName = requestURL.Length == 0 || !currentSession.IsInternalAdmin ? currentSession.CurrencyGroup : requestURL[requestURL.Length - 1];

            foreach (var group in groups)
                if (groupName.Equals(group, StringComparison.OrdinalIgnoreCase) ||
                    groupName.Equals("Finance", StringComparison.OrdinalIgnoreCase) ||
                    groupName.Equals("Internal", StringComparison.OrdinalIgnoreCase) ||
                    groupName.Equals("IT", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

            return true;
        }

        public static bool IsShowProduct(string productId, ICurrentSession currentSession)
        {
            return !IsHiddenProduct(ProductsConfiguration.Instance.ProductCollection.First(n => n.Id.Equals(productId, StringComparison.OrdinalIgnoreCase)), currentSession);
        }
    }
}
