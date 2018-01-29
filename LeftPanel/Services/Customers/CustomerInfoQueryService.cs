namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Core.Authorization;
    using Core.Shared;
    using Extensions;
    using Utilities;
    using ViewModels;

    public interface CustomerInfoQueryService
    {
        CustomerModel GetCustomerModel(int actorId, string custName, string groupName, DataSet customerInfoDataset);
    }

    public class CustomerInfoQueryServiceImpl : CustomerInfoQueryService
    {
        private static readonly IReadOnlyDictionary<int, CustomerSettingQueryService> settingServiceInstances =
            new Dictionary<int, CustomerSettingQueryService>
            {
                [Role.Super] = new SuperSettingServiceImpl(),
                [Role.Master] = new MasterSettingServiceImpl(),
                [Role.Agent] = new CustomerSettingServiceImpl(),
                [Role.Member] = new MemberSettingServiceImpl()
            };

        protected readonly AuthorizationService authorizationService;
        protected readonly IReadOnlyDictionary<int, CustomerSettingQueryService> settingServices;

        public CustomerInfoQueryServiceImpl(
            AuthorizationService authorizationService = null,
            IReadOnlyDictionary<int, CustomerSettingQueryService> settingServices = null)
        {
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.settingServices = settingServices ?? settingServiceInstances;
        }

        public CustomerModel GetCustomerModel(int actorId, string custName, string groupName, DataSet customerInfoDataset)
        {
            var custInfoRow = customerInfoDataset.Tables[0].Rows[0];
            var cashRow = customerInfoDataset.Tables[2].Rows[0];
            var lineRow = customerInfoDataset.Tables[1].Rows[0];
            var roleId = custInfoRow.RoleId();
            var isUpdateCredit = authorizationService.HasUpdateCreditPermission(actorId);

            var customerModel = new CustomerModel
            {
                HasDownline = custInfoRow.HasDownline(),
                UplineHasSecurityCode = custInfoRow.UplineHasSecurityCode(),
                CurrencyId = custInfoRow.CurrencyId(),
                BalanceInfo = GetBalanceInfo(isUpdateCredit, custInfoRow, cashRow),
                PersonalInfo = GetPersonalInfo(custInfoRow, lineRow),
                Login = GetLoginInfo(cashRow, custInfoRow)
            };

            var uplineId = roleId.IsSuper() ? 0 : (int)customerModel.PersonalInfo.UplineId.Value;

            if (!settingServices.ContainsKey(roleId))
            {
                throw new ArgumentException($"RoleId {roleId} is out of range");
            }

            customerModel.Setting =
                settingServices[roleId].GetSetting(actorId, custName, uplineId, groupName, customerInfoDataset);

            return customerModel;
        }

        internal BalanceInfo GetBalanceInfo(bool isUpdateCredit, DataRow customerRow, DataRow cashRow)
        {
            var roleId = customerRow.RoleId();

            var balanceInfo = new BalanceInfo
            {
                CurrencyId = ObjectInfo.CreateInstance("Currency Id", customerRow.CurrencyId()),
                Currency = ObjectInfo.CreateInstance("Currency", customerRow.Currency()),
                Balances = new List<ObjectInfo>
                {
                    ObjectInfo.CreateInstance("Total Cash", Utils.ToDecimal(cashRow[GetCashColumn(roleId)])),
                    ObjectInfo.CreateInstance("Yesterday Cash", Utils.ToDecimal(cashRow[GetYesterdayCashColumn(roleId)])),
                    ObjectInfo.CreateInstance("Outstanding", Utils.ToDecimal(cashRow[GetOutstandingColumn(roleId)])),
                    ObjectInfo.CreateInstance("Given Credit", customerRow.Credit())
                },
                IsUpdateCredit = isUpdateCredit
            };

            if (roleId.IsNotMember())
            {
                balanceInfo.Balances.Add(ObjectInfo.CreateInstance("Used Credit", cashRow[GetUsedCreditColumn(roleId)]));
                balanceInfo.Balances.Add(ObjectInfo.CreateInstance("Total Balance", cashRow.TotalBalance(roleId)));
                balanceInfo.Balances.Add(ObjectInfo.CreateInstance("Yesterday Balance", cashRow.TotalYesterdayCash(roleId)));

                balanceInfo.Balances.Add(ObjectInfo.CreateInstance(
                    $"{roleId.DirectDownlineLevel().DisplayRoleName()} Outstanding",
                    cashRow.CustomerOutstanding()));
            }

            return balanceInfo;
        }

        internal static string GetOutstandingColumn(int roleId) =>
            roleId.IsMember() ? "outstanding" : $"{roleId.DisplayRoleName()}PositionTaking";

        internal static string GetUsedCreditColumn(int roleId)
        {
            if (roleId.IsMember())
            {
                throw new ArgumentOutOfRangeException(nameof(roleId), "there is no UsedCredit on member level");
            }

            return $"{roleId.DisplayRoleName()}UsedCredit";
        }

        internal static string GetYesterdayCashColumn(int roleId)
            => roleId.IsMember() ? "yesterday_cash" : $"yesterday_{roleId.DisplayRoleName()}Cash";

        internal static string GetCashColumn(int roleId)
            => roleId.IsMember() ? "cash" : $"{roleId.DisplayRoleName()}Cash";

        internal List<ObjectInfo> GetLoginInfo(DataRow cashRow, DataRow customerRow)
        {
            var roleId = customerRow.RoleId();
            var lastLoginIp = customerRow.LastLogInIP();

            var site = roleId.IsNotMember() ? customerRow.Site() : customerRow.MemberSite();
            var ip = authorizationService.CanViewVNIP(lastLoginIp) ? lastLoginIp : string.Empty;

            var loginInfos = new List<ObjectInfo>
            {
                ObjectInfo.CreateInstance("Site", site),
                ObjectInfo.CreateInstance("Created Date", customerRow.CreateDate()),
                ObjectInfo.CreateInstance("Last Login", customerRow.LastLogged()),
                new ObjectInfo
                {
                    Value =  cashRow.LastTxn(),
                    Text = "Last Txn",
                    Tooltip = "Last Transaction"
                },
                ObjectInfo.CreateInstance("Password", customerRow.ExpiredPasswordDay()),
                ObjectInfo.CreateInstance("Last Login IP", ip),
                ObjectInfo.CreateInstance("Security Code", customerRow.LastChangedSecurityCodeDate())
           };

            if (roleId.IsNotMember())
            {
                loginInfos.Add(ObjectInfo.CreateInstance("Login Options", customerRow.LoginOptions()));
                loginInfos.Add(ObjectInfo.CreateInstance("OTP", customerRow.OTPCreateDate()));
            }

            return loginInfos;
        }

        internal virtual PersonalInfo GetPersonalInfo(DataRow customerInfoRow, DataRow lineRow)
        {
            var role = customerInfoRow.RoleId();
            var enabled = customerInfoRow.Enabled();
            var closed = customerInfoRow.Closed();
            var suspended = customerInfoRow.Suspended();

            var personalInfo = new PersonalInfo
            {
                CustId = ObjectInfo.CreateInstance("Cust Id", customerInfoRow.CustId()),
                Fax = ObjectInfo.CreateInstance("Fax", customerInfoRow.Fax()),
                Mobile = ObjectInfo.CreateInstance("Mobile", customerInfoRow.MobilePhone()),
                Name = ObjectInfo.CreateInstance("Name", customerInfoRow.Name()),
                Nickname = ObjectInfo.CreateInstance("Nickname", customerInfoRow.Nickname()),
                Phone = ObjectInfo.CreateInstance("Phone", customerInfoRow.Phone()),
                RoleId = ObjectInfo.CreateInstance("Role Id", role),
                UplineRoleId = ObjectInfo.CreateInstance("Upline Role Id", role.DirectUplineLevel()),
                CustList = new ObjectInfo
                {
                    Key = "CustList",
                    Value = $"{(role.IsNotMember() ? role.DirectDownlineLevel().DisplayRoleName() : string.Empty)} List"
                },
                Status = new ObjectInfo { Tooltip = customerInfoRow.StatusTooltip(enabled, closed, suspended) },
                BackgroundColor = new ObjectInfo { Value = customerInfoRow.StatusBackground(enabled, closed, suspended) },
                IsInternal = new ObjectInfo { Value = customerInfoRow.IsInternal() }
            };

            SetLineInfo(role, personalInfo, lineRow);

            return personalInfo;
        }

        internal static List<ObjectInfo> GetLines(DataRow lineRow, int roleId)
        {
            var downLines = new List<ObjectInfo>
            {
                new ObjectInfo(lineRow.CustomerId(Role.Super).ToString(), lineRow.CustomerName(Role.Super), Role.Super.DisplayRoleName())
            };

            if (roleId.IsSuper())
            {
                return downLines;
            }

            downLines.Add(new ObjectInfo(lineRow.CustomerId(Role.Master).ToString(), lineRow.CustomerName(Role.Master), Role.Master.DisplayRoleName()));

            if (roleId.IsNotMaster())
            {
                downLines.Add(new ObjectInfo(lineRow.CustomerId(Role.Agent).ToString(), lineRow.CustomerName(Role.Agent), Role.Agent.DisplayRoleName()));
            }

            return downLines;
        }

        internal static void SetLineInfo(int role, PersonalInfo personalInfo, DataRow lineRow)
        {
            personalInfo.CurrentLevel = ObjectInfo.CreateInstance(text: role.DisplayRoleName(), value: lineRow.CustomerName(role));

            personalInfo.Downlines = GetLines(lineRow, role);

            if (role.IsNotSuper())
            {
                personalInfo.UplineId = ObjectInfo.CreateInstance(
                    text: $"{role.DirectUplineLevel().DisplayRoleName()} ID",
                    value: lineRow.CustomerId(role.DirectUplineLevel()));
            }
        }
    }
}