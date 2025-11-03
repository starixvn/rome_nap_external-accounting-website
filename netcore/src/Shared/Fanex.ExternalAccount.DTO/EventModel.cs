namespace Fanex.ExternalAccount.DTO
{
    public class EventModel
    {
        public int MatchId { get; set; }
        public string MatchCode { get; set; }
        public int HomeId { get; set; }
        public int AwayId { get; set; }
        public string MatchName { get; set; }
        public bool IsNeutral { get; set; }
    }
}
