namespace Fanex.ExternalAccount.DTO.Report
{
    public class SuperPositionTakingModel
    {
        public SuperPositionTakingModel()
        {
        }

        public SuperPositionTakingModel(
            int custId,
            decimal positionTaking,
            int productId,
            string productName)
        {
            CustId = custId;
            PositionTaking = positionTaking;
            ProductId = productId;
            ProductName = productName;
        }


        public int CustId { get; }

        public decimal PositionTaking { get; }

        public int ProductId { get; }

        public string ProductName { get; }
    }
}
