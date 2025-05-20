namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class AutoMasterPTCommand : INonQueryCommand
    {
        public int CustId { get; }
        public bool DisableAutoMPT { get; }
        public int UserId { get; }
        public string Language { get; }
        public string Ip { get; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public AutoMasterPTCommand(int custId, bool disableAutoMPT, int userId, string language, string ip)
        {
            CustId = custId;
            DisableAutoMPT = disableAutoMPT;
            UserId = userId;
            Language = language;
            Ip = ip;
        }

        public string GetSettingKey()
        {
            return "Acc_CustomerUpdMaster_DisableAutoMPT";
        }

        public bool IsValid()
        {
            return CustId > 0;
        }
    }
}