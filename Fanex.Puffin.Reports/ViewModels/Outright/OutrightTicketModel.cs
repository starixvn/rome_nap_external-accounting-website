using System.Collections.Generic;
using Fanex.Puffin.Report.Entities.Outright;

namespace Fanex.Puffin.Report.ViewModels.Outright
{
    public class OutrightTicketModel
    {
        public OutrightTicketModel() 
        {
            Groups = new List<Group>();
        }
        public List<Group> Groups { get; set; }
    }

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
