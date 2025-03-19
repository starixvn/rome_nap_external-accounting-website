namespace Fanex.ExternalAccount.DataAccess.Products
{
    using Fanex.Data.Repository;

    public class ProductStatusCriteria : CriteriaBase
    {
        public ProductStatusCriteria(int custId, int roleId, byte settingStatus)
        {
            CustId = custId;
            RoleId = roleId;
            SettingStatus = settingStatus;
        }

        public int CustId { get; }
        public int RoleId { get; }
        public byte SettingStatus { get; }

        public override string GetSettingKey() => "Acc_CustSetting_StatusSettingSelect";
        
        public override bool IsValid() => true;
    }
}