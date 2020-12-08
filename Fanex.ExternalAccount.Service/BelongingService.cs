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
            var criteria = new CheckBelongingCriteria(userId, groupName, custId, username);

            return !criteria.IsValid() || belongingDataAccess.IsBelonging(criteria);
        }
    }
}
