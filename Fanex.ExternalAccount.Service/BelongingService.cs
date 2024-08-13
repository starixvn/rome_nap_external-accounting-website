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
            bool isNotUserBelonging = !criteria.IsValid() || !belongingDataAccess.IsBelonging(criteria);

            if (isNotUserBelonging)
            {
                return false;
            }
            else
            {
                if (custId == 0 && username.IsNullOrTrimEmpty())
                {
                    return true;
                }
                else
                {
                    var custCriteria = new CheckCustomerBelongingCriteria(groupName, custId, username);
                    return belongingDataAccess.CheckCustomerBelongingCriteria(custCriteria);
                }
            }
        }
    }
}
