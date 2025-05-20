namespace Fanex.ExternalAccount.MatchWinLossReports.Models
{
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.Data;
    public class SportModel
    {
        public int SportType { get; set; }

        public int ResourceId { get; set; }

        public string RefId { get; set; }
        
        public string SportName
        {
            get
            {
                if (SportType == -99)
                {
                    return RefId;
                }

                var sportName = CacheRef.GetSportName(SportType.ToString());

                if (string.IsNullOrEmpty(sportName))
                {
                    sportName = CacheRef.GetResourceDataByRefIdAndResourceId(RefId, ResourceId.ToString());
                }

                return sportName;
            }
        }
    }
}