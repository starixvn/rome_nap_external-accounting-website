namespace Fanex.ExternalAccount.DataAccess.Customer.MarkAccountType
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.DTO.Customer.MarkAccount;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer.MarkAccountType;

    public class MarkAccountTypeDao : IMarkAccountTypeDao
    {
        private const string ErrorUsernameFormat = "{0}: {1}";
        private const string CommonErrorCode = "1";
        private readonly IDynamicRepository _dynamicRepository;

        public MarkAccountTypeDao() : this(null)
        {
        }

        public MarkAccountTypeDao(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public Result Add(AddMarkAccountCommand criteria)
        {
            var addResult = _dynamicRepository.Fetch<ErrorMarkAccount>(criteria);
            var result = new Result();
            if (addResult.Any(r => !r.Isfinished))
            {
                result.ErrorCode = CommonErrorCode;
                result.ErrorMessage = string.Join(", ", addResult.Where(r => !r.Isfinished).Select(r => r.Username));
            }

            return result;
        }

        public Result Check(CheckMarkAccountCriteria criteria)
        {
            var errorUsernames = _dynamicRepository.Fetch<ErrorMarkAccount>(criteria);
            var result = new Result();
            if (!errorUsernames.Any(e => e.ErrCode > 0))
            {
                return result;
            }

            var errors = errorUsernames.Where(err => err.ErrCode > 0).GroupBy(err => err.ErrMsg);
            result.ErrorCode = CommonErrorCode;
            var errorMsg = new StringBuilder();
            foreach (var error in errors)
            {
                var usernames = string.Join(", ", error.Select(cust => cust.Username)) + "</br>";

                errorMsg.AppendFormat(ErrorUsernameFormat, error.Key, usernames);
            }

            result.ErrorMessage = errorMsg.ToString();

            return result;
        }

        public MarkAccountViewModel SearchMarkAccount(SearchMarkAccountCriteria criteria)
        {
            var result = new MarkAccountViewModel();
            result.Customers = _dynamicRepository.Fetch<MarkAccount>(criteria);
            result.PageIndex = criteria.PageIndex;
            result.TotalRows = criteria.TotalRows;
            return result;
        }

        public Result Unmark(UnmarkMarkAccountCommand command)
        {
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public Result Update(UpdateMarkAccountCommand command)
        {
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }
    }
}
