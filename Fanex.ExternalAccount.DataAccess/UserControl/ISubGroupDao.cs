namespace Fanex.ExternalAccount.DataAccess.UserControl
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DataAccess.Criteria.SubGroup;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.SubGroup;
    using Fanex.ExternalAccount.ViewModel.SubGroup;

    public interface ISubGroupDao
    {
        SubGroupViewModel GetSubGroupCustomer(GetSubGroupCustomerCriteria criteria);

        Result UpdateSubGroupCustomer(UpdateCustomerSubGroupCriteria criteria);

        IEnumerable<SubGroupAdmin> GetSubGroupAdmin(GetSubGroupAdminCriteria criteria);

        Result UpdateSubGroupAdminPassword(UpdateSubGroupAdminPwdCriteria criteria);

        void ForceKickout(ForceKickoutAdminSubGroupCriteria criteria);
    }
}
