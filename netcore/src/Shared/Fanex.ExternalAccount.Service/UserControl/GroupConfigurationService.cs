namespace Fanex.ExternalAccount.Service.UserControl
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.ExternalAccount.DataAccess.Criteria.GroupConfiguration;
    using Fanex.ExternalAccount.DataAccess.UserControl;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Utilities.Reports.WinLossDetail;
    using Fanex.Data.Repository;

    public interface IGroupConfigurationService
    {
        IEnumerable<GroupConfigurationModel> Get(GetGroupConfigurationCriteria criteria);

        Result Update(UpdateGroupConfigurationCriteria criteria);
    }

    public class GroupConfigurationService : IGroupConfigurationService
    {
        private readonly IGroupConfigurationDAO groupConfigurationDAO;

        public GroupConfigurationService() : this(null)
        {
        }

        public GroupConfigurationService(IGroupConfigurationDAO groupConfigurationDAO)
        {
            this.groupConfigurationDAO = groupConfigurationDAO ?? new GroupConfigurationDAO();
        }

        public IEnumerable<GroupConfigurationModel> Get(GetGroupConfigurationCriteria criteria) 
            => groupConfigurationDAO.Get(criteria);

        public Result Update(UpdateGroupConfigurationCriteria criteria)
            => groupConfigurationDAO.Update(criteria);
    }
}