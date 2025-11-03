using Fanex.Common;
using Fanex.Data;
using Fanex.ExternalAccount.DataAccess.Criteria.AutoTransfer;
using Fanex.ExternalAccount.Core.Shared.Extensions;
using Fanex.ExternalAccount.DTO;
using Fanex.Data.Repository;

namespace Fanex.ExternalAccount.DataAccess.AutoTransfer
{
    public interface IAutoTransferDao
    {
        bool GetAutoTransferJobStatus(GetAutoTransferJobStatusCommand command);
        Result InsertToastMessage(InsertToastMessageCommand command);
        long InsertPersonalMessage(InsertPersonalMessageCommand command);
        void AssignPersonalMessage(AssignPersonalMessageCommand command);
    }

    public class AutoTransferDao : IAutoTransferDao
    {
        private readonly IDynamicRepository _dynamicRepository;

        public AutoTransferDao() : this(null)
        { }

        public AutoTransferDao(IDynamicRepository dynamicRepository)
        {
            this._dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public bool GetAutoTransferJobStatus(GetAutoTransferJobStatusCommand command)
        {
            _dynamicRepository.Execute(command);
            return command.IsAutoJobRunning;
        }

        public Result InsertToastMessage(InsertToastMessageCommand command)
        {
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public long InsertPersonalMessage(InsertPersonalMessageCommand command)
        {
            _dynamicRepository.Execute(command);
            return command.MessageIDOutput;
        }

        public void AssignPersonalMessage(AssignPersonalMessageCommand command)
            => _dynamicRepository.Execute(command);
    }
}
