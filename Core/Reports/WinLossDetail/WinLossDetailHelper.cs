using System;
using System.Linq;
using Fanex.CustomerSetting.Module.Core.Web;

namespace Fanex.ExternalAccount.Core.Reports.WinLossDetail
{
    public class WinLossDetailHelper
    {
        public static bool IsShowProduct(ProductElementConfiguration product, ICurrentSession currentSession)
        {
            if (currentSession.IsInternalAdmin || product.ShowForGroups == "*")
            {
                return true;
            }

            if (product.Hide)
            {
                return false;
            }

            string currencyGroup = currentSession.CurrencyGroup;
            string[] groups = product.ShowForGroups.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var group in groups)
            {
                if (currencyGroup.Equals(group, StringComparison.OrdinalIgnoreCase) ||
                    currencyGroup.Equals("Finance", StringComparison.OrdinalIgnoreCase) ||
                    currencyGroup.Equals("Internal", StringComparison.OrdinalIgnoreCase) ||
                    currencyGroup.Equals("IT", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsShowProduct(string productId, ICurrentSession currentSession)
        {
            return IsShowProduct(ProductsConfiguration.Instance.ProductCollection.First(n => n.Id.Equals(productId, StringComparison.OrdinalIgnoreCase)), currentSession);
        }
    }
}
