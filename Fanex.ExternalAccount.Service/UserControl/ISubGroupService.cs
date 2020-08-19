namespace Fanex.ExternalAccount.Service.UserControl
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.ViewModel.SubGroup;

    public interface ISubGroupService
    {
        SubGroupViewModel GetListCustomerSubGroup(IEnumerable<Group> groups, int pageSize, int pageIndex);
        Result UpdateCustomerSubGroup(string subGroupName, string custIds, bool isAssigned, int userId, string ip6Address);
    }
}
