using Fanex.ExternalAccount.DTO.Customer;
using System.Collections.Generic;
using ThirdPartyProducts.StatusSetting.Model;

namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    public interface IThirdPartyProductStatusService
    {
        ThirdPartyProductStatusViewModel Select(int custId, string custName, int roleId, int productId);

        List<ProductStatusResult> Update(
            int custId,
            int subAccId,
            int productId,
            int userId,
            int systemId,
            string ip,
            string username,
            List<ProductStatus> productStatus);
    }
}
