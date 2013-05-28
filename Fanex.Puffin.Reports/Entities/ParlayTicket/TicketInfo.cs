// -----------------------------------------------------------------------
// <copyright file="TicketInfo.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Puffin.Report.Entities.ParlayTicket
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TicketInfo:BaseTicketInfo
    {
        public bool IsDelete { get; set; }

        public string WinLoss { get; set; }

        public string SubTransId { get; set; }
                
    }

    public class SubTicket
    {
        public string SubTransId { get; set; }

        public string Choice { get; set; }
    }
}
