// --------------------------------------------------------------------------
// <copyright file="ICommissionDao.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// Creator: Simon
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.PTEngine.BusinessObjects;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICommissionDao
    {
        ICommissionSetting Age_CustomerSelUserComm(IRequestContext requestContext);

        DTO.Result Age_CustomerUpdAgentComm(ICommissionSetting commissionSetting, IRequestContext requestContext);

        DTO.Result Age_CustomerUpdMasterComm(ICommissionSetting commissionSetting, IRequestContext requestContext);

        DTO.Result Age_CustomerUpdUserComm(ICommissionSetting commissionSetting, IRequestContext requestContext);
    }
}