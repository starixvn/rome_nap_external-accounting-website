namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CasinoLibrary.Model.UserException;
    using DTO.Customer;
    using Interface.Customer;
    using System.Diagnostics;
    using Fanex.Logging.Extensions.Web;
    using ThirdPartyProducts.StatusSetting.Enums;
    using ThirdPartyProducts.StatusSetting.Model;
    using ThirdPartyProducts.StatusSetting.Services;
    using Customer = CasinoLibrary.SBAgent.Customer;

    public class ThirdPartyProductStatusService : IThirdPartyProductStatusService
    {
        public ThirdPartyProductStatusViewModel Select(int custId, string custName, int roleId, int productId)
            => new ThirdPartyProductStatusViewModel
            {
                CustId = custId,
                RoleId = roleId,
                CustName = custName,
                HasCheckAll = productId == (int)Product.Keno,
                GroupBySport = productId == (int)Product.Racing,
                ProductStatusList =
                    StatusSettingService.GetProductStatus(new GetProductStatusRequest
                    {
                        CustId = custId,
                        ProductId = productId,
                        RoleId = roleId
                    })
            };

        public List<ProductStatusResult> Update(
            int custId,
            int subAccId,
            int productId,
            int userId,
            int systemId,
            string ip,
            string username,
            List<ProductStatus> productStatus)
        {
            var result = StatusSettingService.UpdateProductStatus(new UpdateProductStatusRequest
            {
                CustId = custId,
                SubAccId = subAccId,
                UplineId = 0,
                ProductId = productId,
                UserId = userId,
                SystemId = systemId,
                Ip = ip,
                ProductStatus = productStatus
            });

            if (result.Any(r => r.ErrorCode.Equals("0")) && productId == (int)Product.Casino)
            {
                var customer = new Customer();
                try
                {
                    customer.UpdateCustEnableCasino(
                        custId,
                        !productStatus[0].IsDisabled,
                        !productStatus[0].IsDisabledDownline,
                        ip,
                        userId,
                        username,
                        3);
                }
                catch (ErrorException casinoEx)
                {
                    ExceptionHandler.Handle(new Exception(casinoEx.Message, casinoEx), TraceEventType.Warning);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex, TraceEventType.Warning);
                }
            }

            return result;
        }
    }
}