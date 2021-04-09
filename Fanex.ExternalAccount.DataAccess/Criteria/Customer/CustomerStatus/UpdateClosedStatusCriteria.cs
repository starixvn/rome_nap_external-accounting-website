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
            string ip)
        {
            UserId = userId;
            CustId = custId;
            IsClosed = isClosed;
            Language = language;
            IP = ip;
        }

        public int UserId { get; }

        public int CustId { get; }

        [SpParam(SqlParamName = "Suspended")]
        public bool IsClosed { get; }

        public string Language { get; }

        public string IP { get; }

        public override string GetSettingKey() => "Acc_CustomerUpdSuspended";

        public override bool IsValid() => UserId > 0 && CustId > 0;
    }
}