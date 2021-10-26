namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using NAP.Framework.Data;

    public class KickoutCustomerCriteria : CriteriaBase
    {
        public KickoutCustomerCriteria(
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

        public override string GetSettingKey()
            => "Age_OnUser_ForceKickedout";

        public override bool IsValid()
            => CustId > 0 && RoleId > 0;
    }
}
