namespace Fanex.ExternalAccount.Service.Interface.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;

    public interface IFunctionService
    {
        Result CreateFunction(
            string name, 
            string key,
            string description,
            int parentId,
            string parentName,
            int displayOrder,
            int custId,
            string ip,
            string assignedGroupIDs);

        Result UpdateFunction(
            int id,
            string name,
            string description,
            int parentId, 
            string parentName, 
            int displayOrder,
            int custId, 
            string ip, 
            string oldAssignedGroupIDs, 
            string assignedGroupIDs);

        IEnumerable<Function> GetAllFunctions();

        Function GetFunctionPolicy(int funcId);

        Result UpdateStatus(int id, bool isTest, int custId, string ip);
    }
}