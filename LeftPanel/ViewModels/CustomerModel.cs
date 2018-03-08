namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Core.Shared;
    using Extensions;

    public class CustomerModel
    {
        public CustomerModel(DataSet customerInfoDataset, bool isUpdateCredit, Func<string, bool> canViewVNIP)
        {
            var custInfoRow = customerInfoDataset.Tables[0].Rows[0];
            var cashRow = customerInfoDataset.Tables[2].Rows[0];
            var lineRow = customerInfoDataset.Tables[1].Rows[0];

            HasDownline = custInfoRow.HasDownline();
            UplineHasSecurityCode = custInfoRow.UplineHasSecurityCode();
            CurrencyId = custInfoRow.CurrencyId();
            BalanceInfo = new BalanceInfo(isUpdateCredit, custInfoRow, cashRow);
            PersonalInfo = new PersonalInfo(custInfoRow, lineRow);
            Login = GetLoginInfo(cashRow, custInfoRow, canViewVNIP);
            DownlineList = new DownlineList();
            OnlineList = new OnlineList();
        }

        public PersonalInfo PersonalInfo { get; set; }

        public BalanceInfo BalanceInfo { get; set; }

        public DownlineList DownlineList { get; set; }

        public Setting Setting { get; set; }

        public List<ObjectInfo> Login { get; set; }

        public OnlineList OnlineList { get; set; }

        public bool HasDownline { get; set; }

        public int CurrencyId { get; set; }

        public bool UplineHasSecurityCode { get; set; }

        internal List<ObjectInfo> GetLoginInfo(DataRow cashRow, DataRow custInfoRow, Func<string, bool> canViewVNIP)
        {
            var roleId = custInfoRow.RoleId();
            var lastLoginIp = custInfoRow.LastLogInIP();

            var site = roleId.IsNotMember() ? custInfoRow.Site() : custInfoRow.MemberSite();
            var ip = canViewVNIP(lastLoginIp) ? lastLoginIp : string.Empty;

            var loginInfos = new List<ObjectInfo>
            {
                ObjectInfo.CreateInstance("Site", site),
                ObjectInfo.CreateInstance("Created Date", custInfoRow.CreateDate()),
                ObjectInfo.CreateInstance("Last Login", custInfoRow.LastLogged()),
                new ObjectInfo
                {
                    Value =  cashRow.LastTxn(),
                    Text = "Last Txn",
                    Tooltip = "Last Transaction"
                },
                ObjectInfo.CreateInstance("Password", custInfoRow.ExpiredPasswordDay()),
                ObjectInfo.CreateInstance("Last Login IP", ip),
                ObjectInfo.CreateInstance("Security Code", custInfoRow.LastChangedSecurityCodeDate())
            };

            if (roleId.IsNotMember())
            {
                loginInfos.Add(ObjectInfo.CreateInstance("Login Options", custInfoRow.LoginOptions()));
                loginInfos.Add(ObjectInfo.CreateInstance("OTP", custInfoRow.OTPCreateDate()));
            }

            return loginInfos;
        }
    }
}