using System.Collections.Generic;
using Fanex.ExternalAccount.DTO.Customer;
using Fanex.ExternalAccount.Service.Interface.Customer;
using ThirdPartyProducts.StatusSetting.Model;
using ThirdPartyProducts.StatusSetting.Services;

namespace Fanex.Tarus.Business.Implement.Customer
{
    public class ThirdPartyProductStatusService : IThirdPartyProductStatusService
    {
        private StatusSettingService statusSettingService = new StatusSettingService();

        private const int KenoProductId = 8;
        private const int RacingProductId = 2;

        public ThirdPartyProductStatusViewModel Select(int custId, string custName, int roleId, int productId)
        {
            var request = new GetProductStatusRequest
            {
                CustId = custId,
                ProductId = productId,
                RoleId = roleId
            };

            return new ThirdPartyProductStatusViewModel()
            {
                CustId = custId,
                RoleId = roleId,
                CustName = custName,
                HasCheckAll = productId == KenoProductId,
                GroupBySport = productId == RacingProductId,
                ProductStatusList = statusSettingService.GetProductStatus(request)
            };
        }

        public List<ProductStatusResult> Update(
            int custId,
            int subAccId,
            int productId,
            int userId,
            int systemId,
            string ip,
            List<ProductStatus> productStatus)
        {
            var request = new UpdateProductStatusRequest
            {
                CustId = custId,
                SubAccId = subAccId,
                UplineId = 0,
                ProductId = productId,
                UserId = userId,
                SystemId = systemId,
                Ip = ip,
                ProductStatus = productStatus
            };

            return statusSettingService.UpdateProductStatus(request);
        }
    }
}
