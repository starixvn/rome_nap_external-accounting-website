using System;
using System.Collections.Generic;
using System.Linq;
using CasinoLibrary.Model.UserException;
using Fanex.ExternalAccount.DTO.Customer;
using Fanex.ExternalAccount.Service.Interface.Customer;
using Fanex.Module.Logs;
using Fanex.Module.Logs.Services;
using ThirdPartyProducts.StatusSetting.Enums;
using ThirdPartyProducts.StatusSetting.Model;
using ThirdPartyProducts.StatusSetting.Services;

namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    public class ThirdPartyProductStatusService : IThirdPartyProductStatusService
    {
        public ThirdPartyProductStatusViewModel Select(int custId, string custName, int roleId, int productId)
        {
            return new ThirdPartyProductStatusViewModel()
            {
                CustId = custId,
                RoleId = roleId,
                CustName = custName,
                HasCheckAll = productId == (int)Product.Keno,
                GroupBySport = productId == (int)Product.Racing,
                ProductStatusList = StatusSettingService.GetProductStatus(new GetProductStatusRequest { CustId = custId, ProductId = productId, RoleId = roleId })
            };
        }

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
                var customer = new CasinoLibrary.SBAgent.Customer();
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
                    casinoEx.WriteCasinoException(Formater.LogType.Warning);
                }
                catch (Exception ex)
                {
                    ex.Write(Formater.LogType.Warning);
                }
            }

            return result;
        }
    }
}
