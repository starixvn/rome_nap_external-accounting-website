namespace Fanex.ExternalAccount.DataAccess.UserControl
{
    using Criteria.GroupConfiguration;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO;

    public interface IGroupConfigurationDAO
    {
        Result Update(UpdateGroupConfigurationCriteria criteria);
    }

    public class GroupConfigurationDAO : IGroupConfigurationDAO
    {
        private readonly IDynamicRepository dynamicRepository;

        public GroupConfigurationDAO(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Result Update(UpdateGroupConfigurationCriteria criteria)
        {
            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }
    }
}
