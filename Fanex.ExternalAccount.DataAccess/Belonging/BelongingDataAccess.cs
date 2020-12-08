namespace Fanex.ExternalAccount.DataAccess.Belonging
{
    using System;
    using Fanex.Data;
    using Fanex.NAP.Framework.Data;

    public interface IBelongingDataAccess
    {
        bool IsBelonging(ICriteria criteria);
    }

    public class BelongingDataAccess : IBelongingDataAccess
    {
        public bool IsBelonging(ICriteria criteria)
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
