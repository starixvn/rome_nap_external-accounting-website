namespace Fanex.ExternalAccount.RewardManagement.Services
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.RewardManagement.Models;
    //using Fanex.NAP.Framework.Data;
    using Fanex.ExternalAccount.RewardManagement.Criteria;
    using Fanex.ExternalAccount.RewardManagement.DataAccesses;
    using Fanex.ExternalAccount.RewardManagement.Requests;
    using Fanex.Data.Repository;
    
    public interface IRewardManagementService
    {
        RewardReport GetReport(RewardReportRequest request);

        IEnumerable<Program> GetPrograms();
    }

    public class RewardManagementService : IRewardManagementService
    {
        private readonly IRewardDataAccess _dataAccess;
        

        public RewardManagementService(IDynamicRepository repository = null, IRewardDataAccess dataAccess = null)
        {
            _dataAccess = dataAccess ?? new RewardDataAccess(repository);
        }

        public RewardReport GetReport(RewardReportRequest request) => _dataAccess.GetReport(new GetRewardRecordCriteria(request));

        public IEnumerable<Program> GetPrograms() => _dataAccess.GetPrograms(new GeProgramCriteria());
    }
}