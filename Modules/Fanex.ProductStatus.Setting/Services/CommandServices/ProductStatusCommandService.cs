namespace Fanex.ProductStatus.Setting.Services.CommandServices
{
    using System.Collections.Generic;
    using Models;

    public interface ProductStatusCommandService
    {
        Result UpdateProductStatus(
            int productId,
            int custId,
            byte custRole,
            int actorId,
            string actorName,
            string ip,
            byte settingStatus,
            IEnumerable<ProductStatusCommand> commands);
    }
}
