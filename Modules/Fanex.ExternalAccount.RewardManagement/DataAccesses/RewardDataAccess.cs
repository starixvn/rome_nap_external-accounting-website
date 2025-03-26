namespace Fanex.ExternalAccount.RewardManagement.DataAccesses
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.RewardManagement.Criteria;
    using Fanex.ExternalAccount.RewardManagement.Models;
    using Fanex.Data.Repository;

    public interface IRewardDataAccess
    {
        IEnumerable<Program> GetPrograms(GeProgramCriteria criteria);

        RewardReport GetReport(GetRewardRecordCriteria criteria);
    }

    public class RewardDataAccess : IRewardDataAccess
    {
        private readonly IDynamicRepository repository;

        public RewardDataAccess(IDynamicRepository repository = null)
            => this.repository = repository ?? new DynamicRepository();

        public IEnumerable<Program> GetPrograms(GeProgramCriteria criteria)
            => repository.Fetch<Program>(criteria);

        public RewardReport GetReport(GetRewardRecordCriteria criteria)
        {
            var dbResult = repository.Fetch<RewardRecord>(criteria);
            return new RewardReport
            {
                Items = dbResult,
                TotalItems = criteria.TotalRecords
            };
        }
    }
}