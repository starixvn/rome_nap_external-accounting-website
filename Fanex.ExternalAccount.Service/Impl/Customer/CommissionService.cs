// --------------------------------------------------------------------------
// <copyright file="CommissionService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// Creator: Simon
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using Fanex.ExternalAccount.DataAccess.Impl.Customer;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.Service.Impl.Base;
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Fanex.PTEngine.BusinessObjects;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CommissionService : CustomerService, ICommissionService
    {
        private readonly ICommissionDao _dao = new CommissionDao();

        public ICommissionSetting GetCommission(IRequestContext requestContext)
        {
            return this._dao.Age_CustomerSelUserComm(requestContext);
        }

        public DTO.Result UpdateCommissionMaster(ICommissionSetting commissionSetting, IRequestContext requestContext)
        {
            return this._dao.Age_CustomerUpdMasterComm(commissionSetting, requestContext);
        }

        public DTO.Result UpdateCommissionAgent(ICommissionSetting commissionSetting, IRequestContext requestContext)
        {
            return this._dao.Age_CustomerUpdAgentComm(commissionSetting, requestContext);
        }

        public DTO.Result UpdateCommissionMember(ICommissionSetting commissionSetting, IRequestContext requestContext)
        {
            return this._dao.Age_CustomerUpdUserComm(commissionSetting, requestContext);
        }
    }
}