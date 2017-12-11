namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System;
    using System.Collections;
    using System.Data;
    using Fanex.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.PTEngine.BusinessObjects;
    using Logging.Extensions.Database;

    public class CommissionDao : DAOBase, ICommissionDao
    {
        public ICommissionSetting Age_CustomerSelUserComm(IRequestContext requestContext)
        {
            IDataReader reader;
            ICommissionSetting commission = new CommissionSetting();
            using (ObjectDb db = new ObjectDb("Age_CustomerSelUserComm", 120))
            {
                db.AddInParameter("custid", DbType.Int32, requestContext.TargetCustId);
                db.AddInParameter("userid", DbType.Int32, requestContext.CustId);
                try
                {
                    reader = db.ExecuteReader();
                    reader.Read();
                    commission.ReadFromDataRecord(requestContext, reader);
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }

            return commission;
        }

        public DTO.Result Age_CustomerUpdMasterComm(ICommissionSetting commissionSetting, IRequestContext requestContext)
        {
            Hashtable parameters = new Hashtable();
            commissionSetting.MapToSpParameters(parameters);

            parameters.Add("custid", requestContext.TargetCustId);
            parameters.Add("userid", requestContext.CustId);
            parameters.Add("subaccid", requestContext.SubAccId);
            parameters.Add("language", requestContext.Language);
            parameters.Add("ip", requestContext.IP);

            var result = DbHelper.ExecuteDbResult("Age_CustomerUpdMasterComm", parameters);
            return new DTO.Result(result.Outputs["errorcode"], result.Outputs["errormsg"], result.ResultSet);
        }

        public DTO.Result Age_CustomerUpdAgentComm(ICommissionSetting commissionSetting, IRequestContext requestContext)
        {
            Hashtable parameters = new Hashtable();
            commissionSetting.MapToSpParameters(parameters);

            parameters.Add("custid", requestContext.TargetCustId);
            parameters.Add("userid", requestContext.CustId);
            parameters.Add("subaccid", requestContext.SubAccId);
            parameters.Add("language", requestContext.Language);
            parameters.Add("ip", requestContext.IP);

            var result = DbHelper.ExecuteDbResult("Age_CustomerUpdAgentComm", parameters);
            return new DTO.Result(result.Outputs["errorcode"], result.Outputs["errormsg"], result.ResultSet);
        }        

        public DTO.Result Age_CustomerUpdUserComm(ICommissionSetting commissionSetting, IRequestContext requestContext)
        {
            Hashtable parameters = new Hashtable();
            commissionSetting.MapToSpParameters(parameters);

            parameters.Add("custid", requestContext.TargetCustId);
            parameters.Add("userid", requestContext.CustId);
            parameters.Add("subaccid", requestContext.SubAccId);
            parameters.Add("language", requestContext.Language);
            parameters.Add("ip", requestContext.IP);

            var result = DbHelper.ExecuteDbResult("Age_CustomerUpdUserComm", parameters);
            return new DTO.Result(result.Outputs["errorcode"], result.Outputs["errormsg"], result.ResultSet);
        }
    }
}