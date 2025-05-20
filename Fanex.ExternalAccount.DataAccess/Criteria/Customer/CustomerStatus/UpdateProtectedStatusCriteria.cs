namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using Fanex.Data.Repository;

    public class UpdateProtectedStatusCriteria : INonQueryCommand
    {
        public UpdateProtectedStatusCriteria(int custId, bool isProtectionMode, int userId, string ip)
        {
            CustId = custId;
            IsProtectionMode = isProtectionMode;
            UserId = userId;
            IP = ip;
        }

        public int CustId { get; }

        public bool IsProtectionMode { get; }

        public int UserId { get; }

        public string IP { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey()
            => "Age_CustomerUpdProtectedMode";

        public bool IsValid()
            => CustId > 0 && UserId > 0;
    }
}
