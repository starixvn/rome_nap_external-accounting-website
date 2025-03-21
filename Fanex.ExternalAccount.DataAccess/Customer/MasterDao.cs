namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using DTO;
    using DTO.Customer;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer;
    using Interface.Customer;

    public class MasterDao : CustomerDao, IMasterDao
    {
        private readonly IDynamicRepository _dynamicRepository;

        public MasterDao() : this(null)
        {
        }

        public MasterDao(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<int> GetSuperInAutomateTransfer(GetSuperInAutomateTransferCriteria criteria)
            => _dynamicRepository.Fetch<int>(criteria);

        public Transfer GetTransferOption(GetTransferOptionCriteria criteria)
        {
            var result = _dynamicRepository.Fetch<dynamic>(criteria).FirstOrDefault();
            var transferOption = result?.transferoption.ToString();
            return transferOption?.Length > 0 ? new Transfer
            {
#pragma warning disable S109 // Magic numbers should not be used
                Weekly = Convert.ToBoolean(int.Parse(transferOption[7].ToString())),
                Mon = Convert.ToBoolean(int.Parse(transferOption[1].ToString())),
                Tue = Convert.ToBoolean(int.Parse(transferOption[2].ToString())),
                Wed = Convert.ToBoolean(int.Parse(transferOption[3].ToString())),
                Thu = Convert.ToBoolean(int.Parse(transferOption[4].ToString())),
                Fri = Convert.ToBoolean(int.Parse(transferOption[5].ToString())),
                Sat = Convert.ToBoolean(int.Parse(transferOption[6].ToString())),
                Sun = Convert.ToBoolean(int.Parse(transferOption[0].ToString()))
#pragma warning restore S109 // Magic numbers should not be used
            } : null;
        }

        public Result EditTransferOption(EditTransferOptionCriteria criteria)
        {
            var dataSet = _dynamicRepository.FetchDataSet(criteria);
            return new Result
            {
                ErrorCode = criteria.ErrCode.ToString(),
                ErrorMessage = criteria.ErrMsg,
                ResultData = dataSet
            };
        }
    }
}