namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Core.Shared;
    using Extensions;

    public class CustomerModel
    {
        public CustomerModel(
            DataSet customerInfoDataset, 
            DataSet loginInfoDataset,
            DataSet balanceInfoDataset,
            DataSet customerStatusInfoDataset,
            bool isUpdateCredit, 
            Func<string, bool> canViewVNIP,
            decimal balance,
            decimal member3rdPendingTransfer,
            decimal memberOustanding)
        {
            var custInfoRow = customerInfoDataset.Tables[0].Rows[0];
            var custStatusRow = customerStatusInfoDataset.Tables[0].Rows[0];
            var cashRow = balanceInfoDataset.Tables[0].Rows[0];
            var isCashService = custInfoRow.ServiceType() == 1;
            HasDownline = custStatusRow.HasDownline();
            UplineHasSecurityCode = custInfoRow.UplineHasSecurityCode();
            CurrencyId = cashRow.CurrencyId();
            BalanceInfo = new BalanceInfo(isUpdateCredit && !isCashService, custInfoRow.RoleId(), cashRow, balance, member3rdPendingTransfer, memberOustanding);
            RewardInfo = new RewardInfo(custInfoRow.RoleId(), cashRow);
            PersonalInfo = new PersonalInfo(custInfoRow);
            Login = GetLoginInfo(loginInfoDataset, canViewVNIP, cashRow.LastTxn());
            DownlineList = new DownlineList();
            OnlineList = new OnlineList();
            IsCashService = isCashService;
        }

        public PersonalInfo PersonalInfo { get; set; }

        public BalanceInfo BalanceInfo { get; set; }
        
        public RewardInfo RewardInfo { get; set; }

        public DownlineList DownlineList { get; set; }

        public Setting Setting { get; set; }

        public List<ObjectInfo> Login { get; set; }

        public OnlineList OnlineList { get; set; }

        public bool HasDownline { get; set; }

        public int CurrencyId { get; set; }

        public bool UplineHasSecurityCode { get; set; }

        internal List<ObjectInfo> GetLoginInfo(DataSet loginInfoDataset, Func<string, bool> canViewVNIP, string lastTxn)
        {
            var loginInfoDataRow = loginInfoDataset.Tables[0].Rows[0];
            var roleId = loginInfoDataRow.RoleId();
            var lastLoginIp = loginInfoDataRow.LastLogInIP();

            var site = loginInfoDataRow.Site();
            var ip = canViewVNIP(lastLoginIp) ? lastLoginIp : string.Empty;

            var loginInfos = new List<ObjectInfo>
            {
                ObjectInfo.CreateInstance("Site", $"{site} ({loginInfoDataRow.SiteLabel()})"),
                ObjectInfo.CreateInstance("Created Date", loginInfoDataRow.CreatedDate()),
                ObjectInfo.CreateInstance("Last Login", loginInfoDataRow.LastLoggedIn()),
                new ObjectInfo
                {
                    Value =  lastTxn,
                    Text = "Last Txn",
                    Tooltip = "Last Transaction"
                },
                ObjectInfo.CreateInstance("Password", loginInfoDataRow.PasswordExpired()),
                ObjectInfo.CreateInstance("Last Login IP", ip),
                ObjectInfo.CreateInstance("Security Code", loginInfoDataRow.LastChangedSecurityCodeDate())
            };

            if (roleId.IsNotMember())
            {
                loginInfos.Add(ObjectInfo.CreateInstance("Login Options", loginInfoDataRow.LoginOptions()));
                loginInfos.Add(ObjectInfo.CreateInstance("OTP", loginInfoDataRow.OTPCreateDate()));
            }

            return loginInfos;
        }

        public string GroupName { get; set; }
        
        public bool IsCashService { get; set; }
    }
}