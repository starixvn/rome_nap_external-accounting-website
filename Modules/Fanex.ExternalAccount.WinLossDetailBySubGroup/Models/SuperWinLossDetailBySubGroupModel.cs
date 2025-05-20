namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Models
{
    using System.Collections.Generic;

    public class SuperWinLossDetailBySubGroupModel
    {
        public SuperWinLossDetailBySubGroupModel()
        {
        }

        public SuperWinLossDetailBySubGroupModel(
            string username,
            decimal turnover,
            decimal netTurnover,
            decimal grossComm,
            decimal memberWinLoss,
            decimal memberComm,
            decimal memberTotal,
            decimal agentWinLoss,
            decimal agentComm,
            decimal agentTotal,
            decimal masterWinLoss,
            decimal masterComm,
            decimal masterTotal,
            decimal superWinLoss,
            decimal superComm,
            decimal superTotal,
            decimal compay)
        {
            Username = username;
            Turnover = turnover;
            NetTurnover = netTurnover;
            GrossComm = grossComm;
            MemberWinLoss = memberWinLoss;
            MemberComm = memberComm;
            MemberTotal = memberTotal;
            AgentWinLoss = agentWinLoss;
            AgentComm = agentComm;
            AgentTotal = agentTotal;
            MasterWinLoss = masterWinLoss;
            MasterComm = masterComm;
            MasterTotal = masterTotal;
            SuperWinLoss = superWinLoss;
            SuperComm = superComm;
            SuperTotal = superTotal;
            Company = compay;
        }

        public string SubGroup { get; set; }

        public string Username { get; set; }

        public decimal Turnover { get; set; }

        public decimal NetTurnover { get; set; }

        public decimal GrossComm { get; set; }

        public decimal MemberWinLoss { get; set; }

        public decimal MemberComm { get; set; }

        public decimal MemberTotal { get; set; }

        public decimal AgentWinLoss { get; set; }

        public decimal AgentComm { get; set; }

        public decimal AgentTotal { get; set; }

        public decimal MasterWinLoss { get; set; }

        public decimal MasterComm { get; set; }

        public decimal MasterTotal { get; set; }

        public decimal SuperWinLoss { get; set; }

        public decimal SuperComm { get; set; }

        public decimal SuperTotal { get; set; }

        public decimal Company { get; set; }

        public IEnumerable<SuperWinLossDetailBySubGroupModel> SuperWinLossDetails { get; set; }

        public static SuperWinLossDetailBySubGroupModel operator +(SuperWinLossDetailBySubGroupModel source,
            SuperWinLossDetailBySubGroupModel destination)
            => new SuperWinLossDetailBySubGroupModel(
                null,
                source.Turnover + destination.Turnover,
                source.NetTurnover + destination.NetTurnover,
                source.GrossComm + destination.GrossComm,
                source.MemberWinLoss + destination.MemberWinLoss,
                source.MemberComm + destination.MemberComm,
                source.MemberTotal + destination.MemberTotal,
                source.AgentWinLoss + destination.AgentWinLoss,
                source.AgentComm + destination.AgentComm,
                source.AgentTotal + destination.AgentTotal,
                source.MasterWinLoss + destination.MasterWinLoss,
                source.MasterComm + destination.MasterComm,
                source.MasterTotal + destination.MasterTotal,
                source.SuperWinLoss + destination.SuperWinLoss,
                source.SuperComm + destination.SuperComm,
                source.SuperTotal + destination.SuperTotal,
                source.Company + destination.Company);
    }
}