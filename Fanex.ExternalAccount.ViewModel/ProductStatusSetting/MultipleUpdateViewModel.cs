namespace Fanex.ExternalAccount.ViewModel.ProductStatusSetting
{
    using ProductStatus.Setting.Models;

    public class MultipleUpdateViewModel
    {
        public int ProductId { get; set; }

        public int[] CustIds { get; set; }

        public string[] CustNames { get; set; }

        public ProductStatusCommand[] ProductStatus { get; set; }

        public bool IsValid()
        {
            var isValidCustomers = CustIds?.Length > 0
                && CustNames?.Length > 0
                && CustIds.Length == CustNames.Length;

            return ProductId > 0
                && isValidCustomers
                && ProductStatus?.Length > 0;
        }
    }
}
