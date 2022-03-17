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

        public string Username { get; }

        public decimal Turnover { get; }

        public decimal NetTurnover { get; }

        public decimal GrossComm { get; }

        public decimal MemberWinLoss { get; }

        public decimal MemberComm { get; }

        public decimal MemberTotal { get; }

        public decimal AgentWinLoss { get; }

        public decimal AgentComm { get; }

        public decimal AgentTotal { get; }

        public decimal MasterWinLoss { get; }

        public decimal MasterComm { get; }

        public decimal MasterTotal { get; }

        public decimal SuperWinLoss { get; }

        public decimal SuperComm { get; }

        public decimal SuperTotal { get; }

        public decimal Company { get; }

        public IEnumerable<SuperWinLossDetailBySubGroupModel> SuperWinLossDetails { get; set; }

        public static SuperWinLossDetailBySubGroupModel operator +(SuperWinLossDetailBySubGroupModel source, SuperWinLossDetailBySubGroupModel destination)
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
