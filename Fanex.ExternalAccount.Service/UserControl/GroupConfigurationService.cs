namespace Fanex.ExternalAccount.Service.UserControl
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DataAccess.Criteria.GroupConfiguration;
    using Fanex.ExternalAccount.DataAccess.UserControl;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.NAP.Framework.Data;

    public interface IGroupConfigurationService
    {
        IEnumerable<GroupConfigurationModel> Get(GetGroupConfigurationCriteria criteria);

        Result Update(UpdateGroupConfigurationCriteria criteria);        
    }

    public class GroupConfigurationService : IGroupConfigurationService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IGroupConfigurationDAO groupConfigurationDAO;

        public GroupConfigurationService() : this(null, null)
        {

        }

        public GroupConfigurationService(IDynamicRepository dynamicRepository, IGroupConfigurationDAO groupConfigurationDAO)
        {
            this.groupConfigurationDAO = groupConfigurationDAO ?? new GroupConfigurationDAO();
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<GroupConfigurationModel> Get(GetGroupConfigurationCriteria criteria)
            => dynamicRepository.Fetch<GroupConfigurationModel>(criteria);       

        public Result Update(UpdateGroupConfigurationCriteria criteria)
            => groupConfigurationDAO.Update(criteria);
    }
}
