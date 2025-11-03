namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class GetUplineByCustIdCommand : INonQueryCommand
    {
        public int CustId { get; }

        public int UplineId { get; set; }

        public GetUplineByCustIdCommand(int custId)
        {
            CustId = custId;
        }

        public string GetSettingKey()
        {
            return "Age_CustSetting_GetUplineByCustID";
        }

        public bool IsValid() => true;
    }
}