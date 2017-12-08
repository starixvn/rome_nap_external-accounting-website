namespace Fanex.ExternalAccount.DTO.Customer.Casino
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Stakes
    {
        public int GameTypeID
        {
            get;
            set;
        }

        public string GameTypeName
        {
            get;
            set;
        }

        public List<StakeLevelDetail> StakeLevels
        {
            get;
            set;
        }

        public GameTypeDetail Pt
        {
            get;
            set;
        }
    }
}
