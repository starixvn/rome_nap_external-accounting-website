namespace Fanex.ExternalAccount.Service.ProductStatusSetting
{
    using System.Collections.Generic;
    using DTO.ProductStatusSetting;
    using ProductStatus.Setting.Models;
    using ProductStatus.Setting.Services;

    public interface ProductStatusSettingService
    {
        IEnumerable<Result> MultipleUpdate(
            int productId,
            int[] custIds,
            string[] custNames,
            int actorId,
            string ip,
            ProductStatusCommand[] commands);
    }

    public class ProductStatusSettingServiceImpl : ProductStatusSettingService
    {
        private readonly ProductStatusService productStatusService;

        public ProductStatusSettingServiceImpl(ProductStatusService productStatusService = null)
        {
            this.productStatusService = productStatusService ?? new ProductStatusServiceImpl();
        }

        public IEnumerable<Result> MultipleUpdate(
            int productId,
            int[] custIds,
            string[] custNames,
            int actorId,
            string ip,
            ProductStatusCommand[] commands)
        {
            Result result;
            int customerId;

            for (int i = 0, max = custIds.Length; i < max; i++)
            {
                customerId = custIds[i];
                result = productStatusService.UpdateProductStatus(productId, customerId, actorId, ip, commands);

                yield return new MultipleResult(result, customerId, custNames[i]);
            }
        }
    }
}
