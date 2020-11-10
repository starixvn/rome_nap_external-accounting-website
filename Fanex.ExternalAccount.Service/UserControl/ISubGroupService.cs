namespace Fanex.ExternalAccount.Service.UserControl
{
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.ViewModel.SubGroup;

    public interface ISubGroupService
    {
        SubGroupViewModel GetListCustomerSubGroup(string groupName, string subGroupName, string username, int pageSize, int pageIndex);
        Result UpdateCustomerSubGroup(string subGroupName, string custIds, bool isAssigned, int userId, string ip6Address);
    }
}
