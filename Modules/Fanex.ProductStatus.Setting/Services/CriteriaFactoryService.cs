namespace Fanex.ProductStatus.Setting.Services
{
    using System.Collections.Generic;
    using Criteria;
    using Fanex.Data.Repository;
    using Models;
    using Shared;

    public interface CriteriaFactoryService
    {
        ICriteria CreateSelectCriteria(int productId, int custId, int sportType, byte settingStatus);

        IBaseNonQueryCommand CreateUpdateCriteria(
            int productId,
            int custId,
            int actorId,
            string ip,
            byte settingStatus,
            IEnumerable<ProductStatusCommand> commands);
    }

    internal class CriteriaFactoryServiceImpl : CriteriaFactoryService
    {
        private const int MacauGamesStatusSettingType = 21;

        public ICriteria CreateSelectCriteria(int productId, int custId, int sportType, byte settingStatus)
        {
            switch (productId)
            {
                case ProductIdentity.RNGSlot:
                    return new SelectDCSProductStatusCriteria(productId, custId, settingStatus, MacauGamesStatusSettingType);

                default:
                    return new SelectDCSProductStatusCriteria(productId, custId, settingStatus);
            }
        }

        public IBaseNonQueryCommand CreateUpdateCriteria(
            int productId,
            int custId,
            int actorId,
            string ip,
            byte settingStatus,
            IEnumerable<ProductStatusCommand> commands)
        {
            switch (productId)
            {
                case ProductIdentity.RNGSlot:
                    return new UpdateDCSProductStatusCriteria(custId, actorId, ip, commands, settingStatus, MacauGamesStatusSettingType);

                default:
                    return new UpdateDCSProductStatusCriteria(custId, actorId, ip, commands, settingStatus);
            }
        }
    }
}