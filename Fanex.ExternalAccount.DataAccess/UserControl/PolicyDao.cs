namespace Fanex.ExternalAccount.DataAccess.UserControl
{
    using System.Collections.Generic;
    using Data;
    using DTO.UserControl;
    using NAP.Framework.Data;

    public class PolicyDao : IPolicyDao
    {
        public IEnumerable<GroupPolicy> GetGroupPolicies(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<GroupPolicy>(criteria.GetSpParams());
            }
        }

        public IEnumerable<UserPolicy> GetUserPolicies(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<UserPolicy>(criteria.GetSpParams());
            }
        }

        public DTO.Result UpdatePolicies(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                var outputParams = db.GetOuputParams();

                return new DTO.Result(outputParams["errcode"], outputParams["errmsg"]);
            }
        }
    }
}
