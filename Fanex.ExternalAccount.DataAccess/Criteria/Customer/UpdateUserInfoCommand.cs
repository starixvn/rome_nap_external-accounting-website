namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateUserInfoCommand : INonQueryCommand
    {
        public int CustId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Phone { get; }
        public string MobilePhone { get; }
        public string Email { get; }
        public string Fax { get; }
        public string Ip { get; }
        public int UserId { get; }
        public int SubAccId { get; }
        public string Language { get; }
        public string Remark { get; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public UpdateUserInfoCommand(int custId, string firstName, string lastName, string phone, string mobilePhone, string email, string fax, string ip, int userId, int subAccId, string language, string remark)
        {
            CustId = custId;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            Phone = phone ?? string.Empty;
            MobilePhone = mobilePhone ?? string.Empty;
            Email = email ?? string.Empty;
            Fax = fax ?? string.Empty;
            Ip = ip;
            UserId = userId;
            SubAccId = subAccId;
            Language = language;
            Remark = remark ?? string.Empty;
        }

        public string GetSettingKey() => "Age_CustomerUpdUserInfo";

        public bool IsValid() => CustId > 0;
    }
}