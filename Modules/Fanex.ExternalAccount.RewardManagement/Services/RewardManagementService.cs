namespace Fanex.ExternalAccount.RewardManagement.Services
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.RewardManagement.Models;
    using Fanex.ExternalAccount.RewardManagement.Criteria;
    using Fanex.ExternalAccount.RewardManagement.DataAccesses;
    using Fanex.ExternalAccount.RewardManagement.Requests;
    
    public interface IRewardManagementService
    {
        RewardReport GetReport(RewardReportRequest request);

        IEnumerable<Program> GetPrograms();
    }

    public class RewardManagementService : IRewardManagementService
    {
        private readonly IRewardDataAccess _dataAccess;
        

        public RewardManagementService(IRewardDataAccess dataAccess = null)
        {
            _dataAccess = dataAccess ?? new RewardDataAccess();
        }

        public RewardReport GetReport(RewardReportRequest request) => _dataAccess.GetReport(new GetRewardRecordCriteria
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            UserName = request.Username ?? string.Empty,
            ProgramIds = request.ProgramIds,
            PaymentStatus = request.MemberPaymentStatus,
            GroupName = request.GroupName,
            PageSize = request.PageSize,
            PageIndex = request.Page,
        });

        public IEnumerable<Program> GetPrograms() => _dataAccess.GetPrograms(new GeProgramCriteria());
    }
}