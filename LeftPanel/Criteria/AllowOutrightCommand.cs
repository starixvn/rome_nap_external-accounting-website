namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class AllowOutrightCommand : INonQueryCommand
    {
        public int CustId { get; }
        public bool DisableOutright { get; }
        public int UserId { get; }
        public string Language { get; }
        public string Ip { get; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public AllowOutrightCommand(int custId, bool disableOutright, int userId, string language, string ip)
        {
            CustId = custId;
            DisableOutright = disableOutright;
            UserId = userId;
            Language = language;
            Ip = ip;
        }

        public string GetSettingKey()
        {
            return "Acc_CustomerUpdDisableOutright";
        }

        public bool IsValid()
        {
            return CustId > 0;
        }
    }
}