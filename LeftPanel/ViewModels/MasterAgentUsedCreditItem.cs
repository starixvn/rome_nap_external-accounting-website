namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    public class MasterAgentUsedCreditItem
    {
        public string Username => MasterName ?? AgentName;

        public string MasterName { get; set; }

        public string AgentName { get; set; }

        public decimal GivenCredit { get; set; }

        public decimal UsedCredit { get; set; }
    }
}