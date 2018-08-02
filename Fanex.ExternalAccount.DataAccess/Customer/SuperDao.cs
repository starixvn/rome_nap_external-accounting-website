namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System.Collections;
    using System.Data;
    using Criteria.Customer;
    using Data.DNAide;
    using DTO;
    using Interface.Customer;

    public class SuperDao : CustomerDao, ISuperDao
    {
        public DataTable GetData(
            int? userId,
            string groupName,
            int? custId = null,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null
        )
        {
            var param = new Hashtable
            {
                {"groupname", groupName},
                {"custid", custId},
                {"enabled", enabled},
                {"closed", closed},
                {"suspended", suspended},
                {"userid", userId}
            };

            return DbHelper.ExecuteDataTable("EAS_CustomerSelByCom", param);
        }

        public Result UpdateSuperInfo(UpdateSuperInfoCriteria criteria)
        {
            var result = DbHelper.ExecuteDbResult(criteria.GetSettingKey(), criteria.GetSpParams());

            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(), result.ResultSet);
        }
    }
}