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

        public Result CreateFunction(
            string name,
            string key,
            string description,
            int parentId,
            string parentName,
            int displayOrder,
            int custId,
            string ip,
            string assignedGroupIDs)
        {
            return _funcDao.CreateFunction(name, key, description, parentId, parentName, displayOrder, custId, ip,
                    assignedGroupIDs);
        }

        public Result UpdateFunction(
            int id,
            string name,
            string description,
            int parentId,
            string parentName,
            int displayOrder,
            int custId,
            string ip,
            string oldAssignedGroupIDs,
            string assignedGroupIDs)
            => _funcDao.UpdateFunction(id, name, description, parentId, parentName, displayOrder, custId, ip,
                    oldAssignedGroupIDs, assignedGroupIDs);

        public IEnumerable<Function> GetAllFunctions() => _funcDao.GetAllFunctions();

        public Function GetFunctionPolicy(int funcId) => _funcDao.GetFunctionPolicy(funcId);

        public Result UpdateStatus(int id, bool isTest, int custId, string ip)
            => _funcDao.UpdateStatus(id, isTest, custId, ip);
    }
}