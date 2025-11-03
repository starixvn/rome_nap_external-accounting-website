namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.ExternalAccount.Core.Shared;

    public class UpdateSuperInfoCommand : INonQueryBaseCommand
    {
        public UpdateSuperInfoCommand(
            int superId,
            string firstName,
            string lastName,
            string phone,
            string mobilePhone,
            string fax,
            string remark,
            string email,
            string ip,
            int userId)
        {
            CustId = superId;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            Phone = phone ?? string.Empty;
            MobilePhone = mobilePhone ?? string.Empty;
            Fax = fax ?? string.Empty;
            Remark = remark ?? string.Empty;
            Email = email ?? string.Empty;
            Ip = ip;
            UserId = userId;
        }

        public int CustId { get; }

        public string Email { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Phone { get; }

        public string MobilePhone { get; }

        public string Fax { get; }

        public string Remark { get; }

        public string Ip { get; }

        public int UserId { get; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey()
            => "EAS_CustomerUpdSuperInfo";

        public bool IsValid()
            => CustId > 0 && UserId > 0;
    }
}
