// --------------------------------------------------------------------------
// <copyright file="ICommissionService.cs" company="">
// Copyright (c) . All rights reserved.
// Creator: Simon
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using Fanex.PTEngine.BusinessObjects;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICommissionService
    {
        ICommissionSetting GetCommission(IRequestContext requestContext);

        DTO.Result UpdateCommissionMaster(ICommissionSetting commissionSetting, IRequestContext requestContext);

        DTO.Result UpdateCommissionAgent(ICommissionSetting commissionSetting, IRequestContext requestContext);

        DTO.Result UpdateCommissionMember(ICommissionSetting commissionSetting, IRequestContext requestContext);
    }
}