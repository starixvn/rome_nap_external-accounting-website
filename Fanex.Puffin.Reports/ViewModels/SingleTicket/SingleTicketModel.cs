// -----------------------------------------------------------------------
// <copyright file="SingleTicketModel.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Fanex.Puffin.Core.Models;
using Fanex.Puffin.Report.Entities.SingleTicket;

namespace Fanex.Puffin.Report.ViewModels.SingleTicket
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SingleTicketModel
    {
        public SingleTicketModel()
        {
            MatchInfo = new MatchInfo();
            Groups = new List<Group>();
        }

        public MatchInfo MatchInfo { get; set; }

        public List<Group> Groups { get; set; }

    }

    /// <summary>
    /// We have 4 groups: Normal, betdaq, 12eu, mhc
    /// </summary>
    public class Group
    {
        public Group()
        {
            Tickets = new List<TicketInfo>();
            Name = string.Empty;
            TicketInfo c = new TicketInfo();
        }

        public string Name { get; set; }

        public List<TicketInfo> Tickets { get; set; }
    }
}
