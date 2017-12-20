namespace Fanex.ExternalAccount.Service.Interface.UserControl
{
    using System.Collections.Generic;
    using DTO;
    using DTO.UserControl;

    public interface IFunctionService
    {
        Result CreateFunction(Function funtion, int custId, string ip, string assignedGroupIDs);

        Result UpdateFunction(Function function, int custId, string ip, string oldAssignedGroupIDs, string assignedGroupIDs);

        IEnumerable<Function> GetAllFunctions();

        Function GetFunctionPolicy(int funcId);

        Result UpdateStatus(int id, bool isTest, int custId, string ip);
    }
}