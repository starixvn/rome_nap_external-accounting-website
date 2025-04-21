namespace Fanex.ProductStatus.Setting.Services.CommandServices
{
    using System.Collections.Generic;
    using Models;

    public interface ProductStatusCommandService
    {
        Result UpdateProductStatus(
            int productId,
            int custId,
            short custRole,
            int actorId,
            string actorName,
            string ip,
            byte settingStatus,
            IEnumerable<ProductStatusCommand> commands);
    }
}
