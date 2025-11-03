namespace Fanex.ExternalAccount.DataAccess.UserControl
{
    using Criteria.GroupConfiguration;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Utilities.Reports.WinLossDetail;

    
    public interface IGroupConfigurationDAO
    {
        IEnumerable<GroupConfigurationModel> Get(GetGroupConfigurationCriteria criteria);
        Result Update(UpdateGroupConfigurationCriteria criteria);
    }

    public class GroupConfigurationDAO : IGroupConfigurationDAO
    {
        private readonly IDynamicRepository dynamicRepository;

        public GroupConfigurationDAO(IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public Result Update(UpdateGroupConfigurationCriteria criteria)
        {
            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public IEnumerable<GroupConfigurationModel> Get(GetGroupConfigurationCriteria criteria)
        {
            var configurations = dynamicRepository.Fetch<GroupConfigurationModel>(criteria);

            return configurations.Where(i => !Group.IsOfficeGroup(i.GroupName));
        }
    }
}