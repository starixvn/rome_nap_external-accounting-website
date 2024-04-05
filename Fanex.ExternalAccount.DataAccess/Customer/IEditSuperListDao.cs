namespace Fanex.ExternalAccount.DataAccess.Customer
{
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.NAP.Framework.Data;

    public interface IEditSuperListDao
    {
        Result CheckMultipleSupers(CheckMultipleSupersCriteria criteria);

        string ExecuteUpdate(ICriteria criteria);
    }

    public class EditSuperListDao : IEditSuperListDao
    {
        public Result CheckMultipleSupers(CheckMultipleSupersCriteria criteria)
        {
            var result = new Result();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    while (reader != null && reader.Read())
                    {
                        result.ResultData += reader.ToInt32("CustID") + ";" + reader.ToInt32("UserName") + ",";
                    }
                }

                var output = db.GetOuputParams();
                result.ErrorCode = output["errcode"].ToString();
                result.ErrorMessage = output["errmsg"].ToString();
                result.ResultData = result.ResultData.ToString().TrimEnd(new[] { ',' });
            }

            return result;
        }

        public string ExecuteUpdate(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());

                return db.GetOuputParams()["errmsg"].ToString();
            }
        }
    }
}
