namespace Fanex.ExternalAccount.DataAccess.Customer
{
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Utilities;

    public class CheckSuperEntity
    { 
        public int CustId { get; set; }

        public string UserName { get; set; }
    }

    public interface IEditSuperListDao
    {
        Result CheckMultipleSupers(CheckMultipleSupersCriteria criteria);

        string ExecuteUpdate(INonQueryBaseCommand command);
    }

    public class EditSuperListDao : IEditSuperListDao
    {
        private readonly IDynamicRepository _dynamicRepository;

        public EditSuperListDao() : this(null)
        {
        }

        public EditSuperListDao(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public Result CheckMultipleSupers(CheckMultipleSupersCriteria criteria)
        {
            var result = new Result();

            var checkSuperEntities = _dynamicRepository.Fetch<CheckSuperEntity>(criteria);
            result.ErrorCode = criteria.ErrCode.ToString();
            result.ErrorMessage = criteria.ErrMsg;
            result.ResultData = string.Join(",", checkSuperEntities.Select(x => $"{x.CustId};{x.UserName}"));
            return result;
        }

        public string ExecuteUpdate(INonQueryBaseCommand command)
        {
            _dynamicRepository.Execute(command);
            return command.ErrMsg;
        }
    }
}
