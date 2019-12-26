namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using Criteria.Customer;
    using Data.DNAide;
    using DTO;
    using Interface.Customer;

    public class SuperDao : CustomerDao, ISuperDao
    {
        public Result UpdateSuperInfo(UpdateSuperInfoCriteria criteria)
        {
            var result = DbHelper.ExecuteDbResult(criteria.GetSettingKey(), criteria.GetSpParams());

            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(), result.ResultSet);
        }
    }
}