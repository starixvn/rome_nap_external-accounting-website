// --------------------------------------------------------------------------
// <copyright file="CommissionDao.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// Creator: Simon
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System.Collections;
    using System.Data;
    using Fanex.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.DTO;
    using Fanex.PTEngine.BusinessObjects;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CommissionDao : DAOBase, ICommissionDao
    {
        /// <summary>
        /// Get customer commission.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <returns>ICommissionSetting object.</returns>
        public ICommissionSetting Age_CustomerSelUserComm(IRequestContext requestContext)
        {
            IDataReader reader;
            ICommissionSetting commission = new CommissionSetting();
            using (ObjectDb db = new ObjectDb("Age_CustomerSelUserComm", 120))
            {
                db.AddInParameter("custid", DbType.Int32, requestContext.TargetCustId);
                db.AddInParameter("userid", DbType.Int32, requestContext.CustId);
                reader = db.ExecuteReader();
                reader.Read();
                commission.ReadFromDataRecord(requestContext, reader);
            }

            return commission;
        }

        /// <summary>
        /// Update master commission.
        /// </summary>
        /// <param name="commissionSetting">Commission setting.</param>
        /// <param name="requestContext">Request context object.</param>
        /// <returns>Result object.</returns>
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

        /// <summary>
        /// Update agent commission.
        /// </summary>
        /// <param name="commissionSetting">ICommissionSetting object</param>
        /// <param name="requestContext">IRequestContext object.</param>
        /// <returns>Result object.</returns>
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

        /// <summary>
        /// Update member commission.
        /// </summary>
        /// <param name="commissionSetting">Commission setting.</param>
        /// <param name="requestContext">Request context object.</param>
        /// <returns>Result object.</returns>
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