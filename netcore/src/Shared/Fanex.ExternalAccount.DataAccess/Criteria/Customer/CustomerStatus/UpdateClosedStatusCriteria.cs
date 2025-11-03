namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateClosedStatusCriteria : INonQueryCommand
    {
        public UpdateClosedStatusCriteria(
            int userId,
            int custId,
            bool isClosed,
            string language,
            string ip,
            int systemId)
        {
            UserId = userId;
            CustId = custId;
            IsClosed = isClosed;
            Language = language;
            IP = ip;
            SystemId = systemId;
        }

        public int UserId { get; }

        public int CustId { get; }

        [SpParam(SqlParamName = "Closed")]
        public bool IsClosed { get; }

        public string Language { get; }

        public string IP { get; }

        public int SystemId { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey() => "Acc_CustomerUpdClosed";

        public bool IsValid() => UserId > 0 && CustId > 0;
    }
}