namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.PTEngine.BusinessObjects;

    public interface ICommissionDao
    {
        ICommissionSetting Age_CustomerSelUserComm(IRequestContext requestContext);

        DTO.Result Age_CustomerUpdAgentComm(ICommissionSetting commissionSetting, IRequestContext requestContext);

        DTO.Result Age_CustomerUpdMasterComm(ICommissionSetting commissionSetting, IRequestContext requestContext);

        DTO.Result Age_CustomerUpdUserComm(ICommissionSetting commissionSetting, IRequestContext requestContext);
    }
}