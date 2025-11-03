namespace Fanex.ExternalAccount.DTO.Customer.Casino
{
    public class CasinoCustomer
    {
        public bool UplineDisabledCasino { get; set; }

        public bool UplineCasinoSync { get; set; }

        public bool EnableDownline { get; set; }

        public decimal GivenCredit { get; set; }

        public string CurrencyName { get; set; }

        public string UserPwd { get; set; }

        public bool CasinoDisabled { get; set; }

        public bool CasinoSync { get; set; }        

        public int Currency { get; set; }

        public string UserName { get; set; }

        public string UserName2 { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string MobilePhone { get; set; }

        public int LangID { get; set; }

        public int ExchangeID { get; set; }

        public int CountryID { get; set; }

        public string Fax { get; set; }

        public bool Closed { get; set; }

        public bool Suspend { get; set; }

        public int SIBCCustID { get; set; }

        public int MIBCCustID { get; set; }

        public int AIBCCustID { get; set; }

        public int IBCCustID { get; set; }

        public int CSCustID { get; set; }

        public bool IsEnablePT { get; set; }

        public double MaxWin { get; set; }

        public double MaxLose { get; set; }

        public double MinCreditBase { get; set; }

        public bool IsMaxWin { get; set; }

        public bool IsMaxLose { get; set; }

        public bool PTByGameKind { get; set; }

        public bool PTByGameType { get; set; }

        public bool PTByGameTypeUpline { get; set; }

        public double TotalWinLoss { get; set; }

        public int RoleId { get; set; }

        public int UpperCustID { get; set; }

        public bool NoCSData { get; set; }

        public bool NoIBCData { get; set; }

        public bool IsAutoReset { get; set; }

        public bool IsDisableAutoMPT { get; set; }
    }
}
