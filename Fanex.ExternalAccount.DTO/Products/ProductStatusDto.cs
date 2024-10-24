namespace Fanex.ExternalAccount.DTO.Products
{
    public class ProductStatusDto
    {
        public int ProductId { get; set; }

        public bool IsDisabledUpline { get; set; }

        public bool IsHidden { get; set; }

        public bool IsHiddenUpline { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsDisabledDownline { get; set; }

        public bool HasDownline { get; set; }

        public int? GroupId { get; set; }
    }
}