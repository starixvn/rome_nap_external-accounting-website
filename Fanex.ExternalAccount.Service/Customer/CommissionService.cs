namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using Interface.Customer;
    using PTEngine.BusinessObjects;
    using Result = DTO.Result;

    public class CommissionService : CustomerService, ICommissionService
    {
        private readonly ICommissionDao _dao = new CommissionDao();

        public ICommissionSetting GetCommission(IRequestContext requestContext)
            => _dao.Age_CustomerSelUserComm(requestContext);

        public Result UpdateCommissionMaster(ICommissionSetting commissionSetting, IRequestContext requestContext)
            => _dao.Age_CustomerUpdMasterComm(commissionSetting, requestContext);

        public Result UpdateCommissionAgent(ICommissionSetting commissionSetting, IRequestContext requestContext)
            => _dao.Age_CustomerUpdAgentComm(commissionSetting, requestContext);

        public Result UpdateCommissionMember(ICommissionSetting commissionSetting, IRequestContext requestContext)
            => _dao.Age_CustomerUpdUserComm(commissionSetting, requestContext);
    }
}