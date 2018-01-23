namespace Fanex.CustomerSetting.Module.BetSetting.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BetSettingTab
    {
        public BetSettingTab(int productId, string productName, bool isGetNameFromRemotingRef = false)
        {
            ProductId = productId;
            ProductName = isGetNameFromRemotingRef 
                ? RemotingRef.RefData.GetResourceDataByRefIdAndResourceId("Product", productId.ToString(), "en-US", productName)
                : productName;
        }

        public int ProductId { get; }

        public string ProductName { get; }
    }
}
