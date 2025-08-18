namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using System.Collections;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateSMAGivenCreditCommand : INonQueryCommand
    {
        [SpParam(SqlParamName = "custid")]
        public int Id { get; }

        public int UserId { get; }

        [SpParam(SqlParamName = "newcredit")]
        public decimal NewGivenCredit { get; }

        public string Ip { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public UpdateSMAGivenCreditCommand(int id, int userId, decimal newGivenCredit, string ip)
        {
            Id = id;
            UserId = userId;
            NewGivenCredit = newGivenCredit;
            Ip = ip;
        }

        public virtual string GetSettingKey()
        {
            return "Acc_CustomerUpdCredit";
        }

        public virtual bool IsValid()
        {
            return Id > 0 && UserId > 0;
        }
    }
}
