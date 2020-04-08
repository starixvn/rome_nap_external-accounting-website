namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.NAP.Framework.Data;

    public class GetNG2StatusCriteria : CriteriaBase
    {
        public GetNG2StatusCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public int ProductId { get; } = 23;

        public override string GetSettingKey() => "Acc_CustSetting_DisableGameStatusSelect";

        public override bool IsValid() => CustId > 0;
    }
}
