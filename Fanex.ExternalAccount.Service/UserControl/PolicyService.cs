namespace Fanex.ExternalAccount.Service.UserControl
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Shared;
    using DataAccess.Criteria.UserControl;
    using DataAccess.UserControl;
    using DTO;
    using DTO.UserControl;
    using Impl.UserControl;
    using Utilities;
    
    public interface IPolicyService
    {
        IEnumerable<HierarchicalFunction> GetHierarchicalFunctions();

        IEnumerable<GroupPolicy> GetGroupPolicies(int functionId);

        IEnumerable<UserPolicy> GetUserPolicies(int functionId);

        DTO.Result UpdateMultipleGroupPolicies(int functionId, string policy, int userId, string ip);

        DTO.Result UpdateMultipleUserPolicies(int functionId, string policy, int userId, string ip);
    }
    
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyDao policyDAO;
        private readonly IFunctionService functionService;

        public PolicyService(
            IPolicyDao permissionDAO = null,
            IFunctionService functionService = null)
        {
            this.policyDAO = permissionDAO ?? new PolicyDao();
            this.functionService = functionService ?? new FunctionService();
        }

        public IEnumerable<HierarchicalFunction> GetHierarchicalFunctions()
        {
            var functions = functionService
                    .GetAllFunctions()
                    .Where(f => f.FuncKey != eFunctions.ManageMenu
                        && f.FuncKey != eFunctions.ManageFunction
                        && f.FuncKey != eFunctions.ManageGroup
                        && f.FuncKey != eFunctions.ManageUser)
                    .Select(f => new Function
                    {
                        FuncID = f.FuncID,
                        FuncName = f.FuncName,
                        ParentID = f.ParentID,
                        FuncKey = f.FuncKey
                    })
                    .ToList();

            var hierarchicalFunctions = functions
                .Where(f => f.ParentID == 0
                    && f.FuncKey != eFunctions.Administration
                    && f.FuncKey != eFunctions.SystemConfiguration
                    && f.FuncKey != eFunctions.ExternalGroup)
                .Select(f => new HierarchicalFunction(f.FuncID, f.FuncName))
                .ToList();

            foreach (var item in hierarchicalFunctions)
            {
                FillSubFunctions(functions, item);
            }

            return hierarchicalFunctions;
        }

        private static void FillSubFunctions(IEnumerable<Function> functions, HierarchicalFunction parent)
        {
            var subFunctions = functions.Where(s => s.ParentID == parent.Id);
            HierarchicalFunction hierarchicalFunction;

            foreach (var item in subFunctions)
            {
                hierarchicalFunction = new HierarchicalFunction(item.FuncID, item.FuncName);
                FillSubFunctions(functions, hierarchicalFunction);

                parent.SubFunctions.Add(hierarchicalFunction);
            }
        }

        public IEnumerable<GroupPolicy> GetGroupPolicies(int functionId)
            => policyDAO.GetGroupPolicies(new GetGroupPolicyCriteria(functionId));

        public IEnumerable<UserPolicy> GetUserPolicies(int functionId)
            => policyDAO.GetUserPolicies(new GetUserPolicyCriteria(functionId));

        public Result UpdateMultipleGroupPolicies(int functionId, string policy, int userId, string ip)
        {
            var result = policyDAO.UpdateMultipleGroupPolicy(new UpdateMultipleGroupPolicyCriteria(
                functionId,
                policy,
                GetOldGroupPolicies(functionId),
                userId,
                ip));
            if (result.IsSuccess)
            {
                result.ErrorMessage = ViewLang.UpdateSuccessfully;
            }

            return result;
        }
        
        private string GetOldGroupPolicies(int functionId)
        {
            var oldGroupPolicies = GetGroupPolicies(functionId);

            return oldGroupPolicies?.Any() == false
                ? string.Empty
                : string.Join(";", oldGroupPolicies.Select(policy => $"{policy.GroupId},{policy.Policy}"));
        }

        public Result UpdateMultipleUserPolicies(int functionId, string policy, int userId, string ip)
        {
            var result = policyDAO.UpdateMultipleUserPolicy(new UpdateMultipleUserPolicyCriteria(
                functionId,
                policy,
                GetOldUserPolicies(functionId),
                userId,
                ip));

            if (result.IsSuccess)
            {
                result.ErrorMessage = ViewLang.UpdateSuccessfully;
            }

            return result;
        }
        
        private string GetOldUserPolicies(int functionId)
        {
            var oldUserPolicies = GetUserPolicies(functionId);

            return oldUserPolicies?.Any() == false
                ? string.Empty
                : string.Join(";", oldUserPolicies.Select(policy => $"{policy.CustId},{policy.Policy}"));
        }
    }
}
