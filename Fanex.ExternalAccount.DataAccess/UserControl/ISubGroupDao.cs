namespace Fanex.ExternalAccount.DataAccess.UserControl
{
    using Fanex.ExternalAccount.DataAccess.Criteria.SubGroup;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.ViewModel.SubGroup;

    public interface ISubGroupDao
    {
        SubGroupViewModel GetSubGroupCustomer(GetSubGroupCustomerCriteria criteria);

        Result UpdateSubGroupCustomer(UpdateCustomerSubGroupCriteria criteria);
    }
}
