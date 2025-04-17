namespace Fanex.CustomerSetting.Module.BetSetting.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetUserInfoCriteria : CriteriaBase
    {
        public GetUserInfoCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public byte SettingStatus => Config.SettingStatus;

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Age_CustomerSelUserInfo";
    }
}
