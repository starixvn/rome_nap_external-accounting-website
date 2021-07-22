namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using NAP.Framework.Data;

    public class UpdateClosedStatusCriteria : CriteriaBase
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

        public override string GetSettingKey() => "Acc_CustomerUpdClosed";

        public override bool IsValid() => UserId > 0 && CustId > 0;
    }
}