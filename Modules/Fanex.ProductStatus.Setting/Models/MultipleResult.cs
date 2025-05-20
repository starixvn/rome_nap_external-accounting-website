namespace Fanex.ProductStatus.Setting.Models
{
    public class MultipleResult  : Result
    {
        public MultipleResult(Result result, int custId, string custName)
               : base(result.Code, result.Message)
        {
            CustId = custId;
            CustName = custName;
        }

        public int CustId { get; }

        public string CustName { get; }
    }
}
