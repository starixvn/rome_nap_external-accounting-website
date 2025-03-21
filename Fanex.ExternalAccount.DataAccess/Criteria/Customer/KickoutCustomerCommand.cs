namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data.Repository;

    public class KickoutCustomerCommand : INonQueryCommand
    {
        public KickoutCustomerCommand(
            int custId,
            byte roleId,
            string ip,
            string mode)
        {
            CustId = custId;
            RoleId = roleId;
            IP = ip;
            Mode = mode;
        }

        public int CustId { get; }

        public byte RoleId { get; }

        public string IP { get; }

        public string Mode { get; }

        public string GetSettingKey()
            => "Age_OnUser_ForceKickedout";

        public bool IsValid()
            => CustId > 0 && RoleId > 0;
    }
}
