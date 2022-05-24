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

        public string LeagueGroupId { get; set; }

        public string[] LeagueGroupIds => LeagueGroupId != null ? LeagueGroupId.Split(',') : new string[] { };

        public string TicketType { get; set; }

        public string[] TicketTypeIds => TicketType != null ? TicketType.Split(',') : new string[] { };
    }
}
