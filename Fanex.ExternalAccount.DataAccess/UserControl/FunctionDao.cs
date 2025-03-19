namespace Fanex.ExternalAccount.DataAccess.Impl.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Base;
    using Criteria.UserControl;
    using Data;
    using DTO;
    using DTO.UserControl;
    using Fanex.Data.Repository;
    using Interface.UserControl;
    using Utilities;

    public class FunctionDao : DAOBase, IFunctionDao
    {
        private readonly IDynamicRepository dynamicRepository;

        public FunctionDao(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Result CreateFunction(Function function, int custId, string ip, string assignedGroupIDs)
        {
            var criteria = new CreateFunctionCriteria
            {
                UserId = custId,
                ParentId = function.ParentID,
                DisplayOrder = function.DisplayOrder,
                IsTest = false,
                ParentName = function.ParentName,
                FuncName = function.FuncName,
                FuncDesc = function.FuncDesc,
                FuncKey = function.FuncKey,
                IP = ip,
                AssignedGroupIDs = assignedGroupIDs
            };

            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result UpdateFunction(Function function, int custId, string ip, string oldAssignedGroupIDs, string assignedGroupIDs)
        {
            var criteria = new UpdateFunctionCriteria
            {
                UserId = custId,
                FuncId = function.FuncID,
                ParentId = function.ParentID,
                DisplayOrder = function.DisplayOrder,
                ParentName = function.ParentName,
                FuncName = function.FuncName,
                FuncDesc = function.FuncDesc,
                IP = ip,
                AssignedGroupIDs = assignedGroupIDs,
                OldAssignedGroupIDs = oldAssignedGroupIDs
            };

            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public IEnumerable<Function> GetAllFunctions()
        {
            var criteria = new GetAllFunctionsCriteria();
            return dynamicRepository.Fetch<Function>(criteria) ?? new List<Function>();
        }

        public Function GetFunctionPolicy(int funcId)
        {
            var func = GetAllFunctions().FirstOrDefault(o => o.FuncID == funcId);
            if (func == null)
            {
                return null;
            }

            var criteria = new GetFunctionPolicyCriteria { FuncId = funcId };
            var groups = dynamicRepository.Fetch<FunctionPolicy>(criteria) ?? new List<FunctionPolicy>();
            func.AssignGroups = groups;

            return func;
        }

        public Result UpdateStatus(int id, bool isTest, int custId, string ip)
        {
            var criteria = new UpdateStatusCriteria
            {
                UserId = custId,
                FuncId = id,
                IsTest = isTest,
                IP = ip
            };

            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }
    }
}