namespace Fanex.ExternalAccount.SearchAndCopy.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.SearchAndCopy.Criteria;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.Logging;

    public interface ISearchAndCopyRepository
    {
        Result ValidateSettingComm(ValidateCloneSettingComCommand command);

        Result RegisterCustomer(RegisterCustomerCommand command);

        string GetGetGroupParentNameByGroupName(GetGroupParentNameByGroupNameCriteria criteria);

        void RollBackCustomer(RollbackCustomerInsertCommand command);

        Result CloneSettingComm(CloneSettingComCommand command);

        Result CloneSettingExtraPT(CloneSettingExtraPTCommand command);

        IEnumerable<SearchCustomerModel> SearchCustomer(string username, string groupName, int custDBGroupId);

        Result UpdateCRMHeadlineInfo(UpdateCrmHeadlineInfoCriteria criteria);

        IEnumerable<SuperPrefix> GetSuperPrefixes(ICriteria criteria);

        void AddMsgAfterCreateNewSMA(AddMsgAfterCreateNewSMACommand command);
    }

    public class SearchAndCopyRepository : ISearchAndCopyRepository
    {
        private readonly IDynamicRepository _dynamicRepository;

        public SearchAndCopyRepository()
            : this(null)
        {
        }

        public SearchAndCopyRepository(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public Result ValidateSettingComm(ValidateCloneSettingComCommand command)
        {
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public Result RegisterCustomer(RegisterCustomerCommand command)
        {
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg, new { NewCustId = command.CustId });
        }

        public string GetGetGroupParentNameByGroupName(GetGroupParentNameByGroupNameCriteria criteria)
            => string.Join(",", _dynamicRepository.Fetch<string>(criteria).ToArray());

        public Result CloneSettingComm(CloneSettingComCommand command)
        {
            if (!command.IsValid())
            {
                return new Result(1, "Invalid operation");
            }

            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg, new
            {
                command.CustId,
                command.Case,
                HeadlineID_Old = command.Old_Headline,
                HeadlineID_New = command.New_Headline
            });
        }

        public void RollBackCustomer(RollbackCustomerInsertCommand command)
            => _dynamicRepository.Execute(command);

        public Result CloneSettingExtraPT(CloneSettingExtraPTCommand command)
        {
            if (!command.IsValid())
            {
                return new Result(1, "Invalid operation");
            }
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public IEnumerable<SearchCustomerModel> SearchCustomer(string username, string groupName, int custDBGroupId)
        {
            var criteria = new SearchCustomerCriteria(username, groupName, custDBGroupId);
            if (!criteria.IsValid())
            {
                return Enumerable.Empty<SearchCustomerModel>();
            }

            return _dynamicRepository.Fetch<SearchCustomerModel>(criteria);
        }

        public Result UpdateCRMHeadlineInfo(UpdateCrmHeadlineInfoCriteria criteria)
        {
            _dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public IEnumerable<SuperPrefix> GetSuperPrefixes(ICriteria criteria)
            => _dynamicRepository.Fetch<SuperPrefix>(criteria);

        public void AddMsgAfterCreateNewSMA(AddMsgAfterCreateNewSMACommand command)
        {
            try
            {
                _dynamicRepository.Execute(command);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
            }
        }
    }
}