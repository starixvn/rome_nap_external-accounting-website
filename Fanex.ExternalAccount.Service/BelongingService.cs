using Fanex.Common.Extensions;

namespace Fanex.ExternalAccount.Service
{
    using Fanex.ExternalAccount.DataAccess.Belonging;
    using Fanex.ExternalAccount.DataAccess.Criteria.Belonging;

    public interface IBelongingService
    {
        bool IsBelonging(int userId, string groupName, int custId, string username);
    }

    public class BelongingService : IBelongingService
    {
        private readonly IBelongingDataAccess belongingDataAccess;

        public BelongingService(IBelongingDataAccess belongingDataAccess = null)
        {
            this.belongingDataAccess = belongingDataAccess ?? new BelongingDataAccess();
        }

        public bool IsBelonging(int userId, string groupName, int custId, string username)
        {
            var criteria = new CheckBelongingCriteria(userId, groupName);
            bool isUserBelonging = !criteria.IsValid() || belongingDataAccess.IsBelonging(criteria);

            if (isUserBelonging)
            {
                var custCriteria = new CheckCustomerBelongingCriteria(groupName, custId, username);
                if (!custCriteria.IsValid())
                {
                    return true;
                }
                else
                {
                    return belongingDataAccess.CheckCustomerBelongingCriteria(custCriteria);
                }
            }

            return isUserBelonging;
        }
    }
}
