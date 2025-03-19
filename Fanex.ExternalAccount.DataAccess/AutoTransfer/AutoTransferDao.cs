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
        bool GetAutoTransferJobStatus(GetAutoTransferJobStatusCriteria command);
        Result InsertToastMessage(ICriteria criteria);
        long InsertPersonalMessage(ICriteria criteria);
        void AssignPersonalMessage(long messageId, int custId, string custName);
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

        public bool GetAutoTransferJobStatus(GetAutoTransferJobStatusCriteria command)
        {
            _dynamicRepository.Execute(command);
            return command.IsAutoJobRunning;
        }

        public Result InsertToastMessage(ICriteria criteria)
        {
            Result result;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());

                var outputs = db.GetOuputParams();
                result = new Result(outputs["errcode"].ToString(), outputs["errmsg"].ToString());
            }

            return result;
        }

        public long InsertPersonalMessage(ICriteria criteria)
        {
            long result = 0;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = db.ExecuteReader(criteria.GetSpParams());

                while (reader.Read())
                {
                    result = long.Parse(reader["MessageId"].ToString());
                }
            }

            return result;
        }

        public void AssignPersonalMessage(long messageId, int custId, string custName)
        {
            var criteria = new AssignPersonalMessageCriteria(messageId, custId, custName);

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
            }
        }
    }
}
