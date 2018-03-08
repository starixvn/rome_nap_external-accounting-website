namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System.Collections.Generic;
    using DataAccess.Impl.UserControl;
    using DataAccess.Interface.UserControl;
    using DTO;
    using DTO.UserControl;
    using Interface.UserControl;

    public class FunctionService : IFunctionService
    {
        private readonly IFunctionDao _funcDao;

        public FunctionService(IFunctionDao funcDao = null)
        {
            _funcDao = funcDao ?? new FunctionDao();
        }

        public Result CreateFunction(Function function, int custId, string ip, string assignedGroupIDs)
            => _funcDao.CreateFunction(function, custId, ip, assignedGroupIDs);

        public Result UpdateFunction(Function function, int custId, string ip, string oldAssignedGroupIDs, string assignedGroupIDs)
            => _funcDao.UpdateFunction(function, custId, ip, oldAssignedGroupIDs, assignedGroupIDs);

        public IEnumerable<Function> GetAllFunctions() => _funcDao.GetAllFunctions();

        public Function GetFunctionPolicy(int funcId) => _funcDao.GetFunctionPolicy(funcId);

        public Result UpdateStatus(int id, bool isTest, int custId, string ip)
            => _funcDao.UpdateStatus(id, isTest, custId, ip);
    }
}