namespace Fanex.ExternalAccount.DTO.ProductStatusSetting
{
    using System.Collections;

    public class MultipleResult : ProductStatus.Setting.Models.Result
    {
        public MultipleResult(ProductStatus.Setting.Models.Result result, int custId, string custName)
            : base(result.Code, result.Message)
        {
            CustId = custId;
            CustName = custName;
        }

        public MultipleResult(Hashtable outputParams)
            : base(outputParams)
        {
        }

        public int CustId { get; }

        public string CustName { get; }
    }
}
