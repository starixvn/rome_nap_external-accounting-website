namespace Fanex.ExternalAccount.DTO
{
    public class Request
    {
        public string NetSessionId { get; set; }

        public string CountryCode { get; set; }

        public string DomainName { get; set; }

        public string IP { get; set; }

        public string Browser { get; set; }

        public string Platform { get; set; }

        public int SubAccId { get; set; }

        public int UserId { get; set; }

        public string Language { get; set; }
    }
}