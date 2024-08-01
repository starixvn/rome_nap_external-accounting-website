namespace Fanex.ExternalAccount.DataAccess.Products
{
    using Fanex.NAP.Framework.Data;

    public class GetShowProductsCriteria : CriteriaBase
    {
        public GetShowProductsCriteria(int? groupId)
        {
            GroupId = groupId;
        }

        public int? GroupId { get; }


        public override string GetSettingKey() => "EAS_GetProductID_ByGroupConfiguration";

        public override bool IsValid() => true;
    }
}
