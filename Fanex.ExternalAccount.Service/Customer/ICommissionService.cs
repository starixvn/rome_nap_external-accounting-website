namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using Fanex.PTEngine.BusinessObjects;

    public interface ICommissionService
    {
        ICommissionSetting GetCommission(IRequestContext requestContext);

        DTO.Result UpdateCommissionMaster(ICommissionSetting commissionSetting, IRequestContext requestContext);

        DTO.Result UpdateCommissionAgent(ICommissionSetting commissionSetting, IRequestContext requestContext);

        DTO.Result UpdateCommissionMember(ICommissionSetting commissionSetting, IRequestContext requestContext);
    }
}