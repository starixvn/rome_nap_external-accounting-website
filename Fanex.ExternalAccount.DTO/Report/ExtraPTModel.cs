namespace Fanex.ExternalAccount.DTO.Report
{
    using Fanex.ExternalAccount.Core.Shared;

    public class ExtraPTModel
    {
        public int ProductId { get; set; }

        public float ExtraPT { get; set; }

        public string ProductName
            => CacheRef.GetProductName(ProductId);
    }
}