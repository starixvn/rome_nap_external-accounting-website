namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateCrmHeadlineInfoCriteria : INonQueryCommand
    {
        public UpdateCrmHeadlineInfoCriteria(
            int custId,
            string userName,
            int @case,
            int oldHeadline,
            int newHeadline,
            int userId,
            string loginIP)
        {
            CustId = custId;
            UserName = userName;
            Case = @case;
            OldHeadline = oldHeadline;
            NewHeadline = newHeadline;
            UserId = userId;
            LoginIP = loginIP;
        }

        public int CustId { get; }

        public string UserName { get; }

        public int Case { get; }

        public int OldHeadline { get; }

        public int NewHeadline { get; }

        public int UserId { get; }

        public string LoginIP { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey() => "Acc_CRM_HeadlineUpd";

        public bool IsValid() => true;
    }
}
