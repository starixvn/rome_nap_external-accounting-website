namespace Fanex.ProductStatus.Setting.Services.CommandServices
{
    using System.Collections.Generic;
    using DataAccess;
    using Models;

    internal class DefaultProductStatusCommandService : ProductStatusCommandService
    {
        private readonly ProductStatusDataAccess statusDataAccess;
        private readonly CriteriaFactoryService criteriaFactory;

        public DefaultProductStatusCommandService(
            ProductStatusDataAccess statusDataAccess = null,
            CriteriaFactoryService criteriaFactory = null)
        {
            this.statusDataAccess = statusDataAccess ?? new ProductStatusDataAccessImpl();
            this.criteriaFactory = criteriaFactory ?? new CriteriaFactoryServiceImpl();
        }

        public Result UpdateProductStatus(
            int productId,
            int custId,
            short custRole,
            int actorId,
            string actorName,
            string ip,
            byte settingStatus,
            IEnumerable<ProductStatusCommand> commands)
        {
            var criteria = criteriaFactory.CreateUpdateCriteria(productId, custId, actorId, ip, settingStatus, commands);
            var result = statusDataAccess.ExecuteNonQuery(criteria);
            result.ParseDCSProductMessage();

            return result;
        }
    }
}