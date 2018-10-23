namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System.Collections;
    using System.Data;
    using Base;
    using Data;
    using Data.DNAide;
    using Interface.Customer;
    using PTEngine.BusinessObjects;
    using Result = DTO.Result;

    public class CommissionDao : DAOBase, ICommissionDao
    {
        public ICommissionSetting Age_CustomerSelUserComm(IRequestContext requestContext)
        {
            ICommissionSetting commission = new CommissionSetting();
            using (var db = new ObjectDb("Age_CustomerSelUserComm", 120))
            {
                db.AddInParameter("custid", DbType.Int32, requestContext.TargetCustId);
                db.AddInParameter("userid", DbType.Int32, requestContext.CustId);
                var reader = db.ExecuteReader();
                reader.Read();
                commission.ReadFromDataRecord(requestContext, reader);
            }

            return commission;
        }

        public Result Age_CustomerUpdMasterComm(ICommissionSetting commissionSetting, IRequestContext requestContext)
            => DoCustomerUpdComm(commissionSetting, requestContext, "Age_CustomerUpdMasterComm");

        public Result Age_CustomerUpdAgentComm(ICommissionSetting commissionSetting, IRequestContext requestContext)
            => DoCustomerUpdComm(commissionSetting, requestContext, "Age_CustomerUpdAgentComm");

        public Result Age_CustomerUpdUserComm(ICommissionSetting commissionSetting, IRequestContext requestContext)
            => DoCustomerUpdComm(commissionSetting, requestContext, "Age_CustomerUpdUserComm");

        public Result DoCustomerUpdComm(
            ICommissionSetting commissionSetting,
            IRequestContext requestContext,
            string spName)
        {
            var parameters = new Hashtable();
            commissionSetting.MapToSpParameters(parameters);

            parameters.Add("custid", requestContext.TargetCustId);
            parameters.Add("userid", requestContext.CustId);
            parameters.Add("subaccid", requestContext.SubAccId);
            parameters.Add("language", requestContext.Language);
            parameters.Add("ip", requestContext.IP);

            var result = DbHelper.ExecuteDbResult("Age_CustomerUpdMasterComm", parameters);
            return new Result(result.Outputs["errorcode"], result.Outputs["errormsg"], result.ResultSet);
        }
    }
}