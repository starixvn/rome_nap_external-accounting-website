// -----------------------------------------------------------------------
// <copyright file="TicketInfo.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Puffin.Report.Entities
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BaseTicketInfo
    {
        public int No { get; set; }

        public string BetId { get; set; }

        public string Player { get; set; }

        public string TransDate { get; set; }

        public int BetType { get; set; }

        public string BetTypeName
        {
            get;
            set;
        }

        public string Choice { get; set; }

        public string Stake { get; set; }

        public string Odds { get; set; }

        public string CurrencyName { get; set; }

        public string Status { get; set; }

        public string HDP { get; set; }

        public string Score { get; set; }

        public string SA { get; set; }

        
    }
}
