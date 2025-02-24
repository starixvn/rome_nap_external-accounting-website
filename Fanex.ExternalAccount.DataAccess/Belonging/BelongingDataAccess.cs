namespace Fanex.ExternalAccount.DataAccess.Belonging
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.Belonging;

    public interface IBelongingDataAccess
    {
        bool IsBelonging(ICriteria criteria);
        bool CheckCustomerBelongingCriteria(ICriteria criteria);
    }

    public class BelongingDataAccess : IBelongingDataAccess
    {
        private IDynamicRepository dynamicRepository;

        public BelongingDataAccess(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public bool IsBelonging(CheckBelongingCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                var ouputParams = db.GetOuputParams();

                return Convert.ToBoolean(ouputParams["isbelonging"]);
            }
        }

        public bool CheckCustomerBelongingCriteria(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                var ouputParams = db.GetOuputParams();

                return Convert.ToBoolean(ouputParams["isbelonging"]);
            }
        }
    }
}
