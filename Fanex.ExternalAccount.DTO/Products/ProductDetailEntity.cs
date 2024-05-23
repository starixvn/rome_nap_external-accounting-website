namespace Fanex.ExternalAccount.DTO.Products
{
    public class ProductDetailEntity
    {
        public string RefId { get; set; }

        public string ResourceId { get; set; }

        public int ProductId { get; set; }

        public int SportType { get; set; }

        public int BetType { get; set; }

        public int Order { get; set; }

        public int LeagueGroupId { get; set; }        

        public int TicketType { get; set; }

        public int ChildResourceId { get; set; }

        public override string ToString()
            => string.Format(
                    "<ProductDetail ProductId=\"{0}\" Sporttype=\"{1}\" Bettype=\"{2}\" LeagueGroupId=\"{3}\" TicketType=\"{4}\"/>",
                    ProductId,
                    SportType,
                    BetType,
                    LeagueGroupId,
                    TicketType);
    }
}
