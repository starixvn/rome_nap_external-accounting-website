namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using System.Collections.Generic;
    using DTO.Customer;
    using ThirdPartyProducts.StatusSetting.Model;

    public interface IThirdPartyProductStatusService
    {
        ThirdPartyProductStatusViewModel Select(int custId, string custName, int roleId, int productId);

        List<ProductStatusResult> Update
        (int custId,
            int subAccId,
            int productId,
            int userId,
            int systemId,
            string ip,
            string username,
            List<ProductStatus> productStatus);
    }
}