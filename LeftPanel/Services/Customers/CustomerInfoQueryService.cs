namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using Core.Authorization;
    using Core.Shared;
    using DTO.Customer;
    using Extensions;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Utilities;
    using ViewModels;

    public interface CustomerInfoQueryService
    {
        CustomerModel GetCustomer(int actorId, string custName, string groupName, DataSet customerInfoDataset);
    }

    public abstract class CustomerInfoQueryBaseService : CustomerInfoQueryService
    {
        private const string DateTimeFormat = "M/d/yyyy H:mm tt";
        private const string ThisAccountWas = "This account was ";

        protected readonly AuthorizationService authorizationHandler;
        protected readonly ProductService productService;

        protected CustomerInfoQueryBaseService(
            AuthorizationService authorizationHandler = null,
            ProductService productService = null)
        {
            this.authorizationHandler = authorizationHandler ?? new AuthorizationServiceImpl();
            this.productService = productService ?? new ProductServiceImpl();
        }

        public CustomerModel GetCustomer(int actorId, string custName, string groupName, DataSet customerInfoDataset)
        {
            var custInfoTable = customerInfoDataset.Tables[0];
            var custInfoRow = custInfoTable.Rows[0];
            var cashRow = customerInfoDataset.Tables[2].Rows[0];
            var lineRow = customerInfoDataset.Tables[1].Rows[0];
            var roleId = Utils.ToInt32(custInfoRow["roleid"]);

            var customerModel = new CustomerModel
            {
                HasDownline = Utils.ToBoolean(custInfoRow["HasDownline"]),
                UplineHasSecurityCode = Utils.ToBoolean(custInfoRow["UplineHasSecurityCode"]),
                CurrencyId = custInfoRow.ToInt32("exchangeid"),
                BalanceInfo = GetBalanceInfo(actorId, custInfoRow, cashRow),
                PersonalInfo = GetPersonalInfo(custInfoRow, lineRow),
                Login = GetLoginInfo(cashRow, custInfoRow)
            };

            customerModel.Setting = productService.GetProductSetting(
                actorId,
                custName,
                roleId.IsSuper()
                  ? 0
                  : (int)customerModel.PersonalInfo.UplineId.Value,
                groupName,
                customerInfoDataset);

            SetCustomSettingByLevel(customerModel.Setting, actorId, customerInfoDataset);

            return customerModel;
        }

        internal virtual BalanceInfo GetBalanceInfo(int userId, DataRow customerRow, DataRow cashRow)
        {
            var roleId = Utils.ToInt32(customerRow["roleid"]);

            var balanceInfo = new BalanceInfo
            {
                CurrencyId = new ObjectInfo { Value = Utils.ToInt32(customerRow["exchangeid"]), Text = "Currency Id" },
                Currency = new ObjectInfo { Value = Utils.ToString(customerRow["currency"]), Text = "Currency" },
                Balances = new List<ObjectInfo>
                {
                    new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetCashColumn(roleId)]), Text = "Total Cash" },
                    new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetYesterdayCashColumn(roleId)]), Text = "Yesterday Cash" },
                    new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetOutstandingColumn(roleId)]), Text = "Outstanding" },
                    new ObjectInfo { Value = Utils.ToDecimal(customerRow["credit"]), Text = "Given Credit" },
                },
                IsUpdateCredit = authorizationHandler.HasUpdateCreditPermission(userId)
            };

            if (roleId.IsNotMember())
            {
                balanceInfo.Balances.Add(new ObjectInfo { Value = cashRow[GetUsedCreditColumn(roleId)], Text = "Used Credit" });
            }

            return balanceInfo;
        }

        internal static string GetOutstandingColumn(int roleId) => roleId.IsMember() ? "outstanding" : $"{roleId.DisplayRoleName()}PositionTaking";

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
            var roleId = Utils.ToInt32(customerRow["roleid"]);
            var lastLoginIp = customerRow.ToString("lastloginip");

            var loginInfos = new List<ObjectInfo>
                {
                    new ObjectInfo { Value = roleId.IsNotMember() ? Utils.ToString(customerRow["site"]) : Utils.ToString(customerRow["membersite"]), Text = "Site" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["createdate"]).ToStringExt(DateTimeFormat), Text = "Created Date" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["lastlogged"]).ToStringExt(DateTimeFormat), Text = "Last Login" },
                    new ObjectInfo { Value = Utils.ToDateTime(cashRow["lastTxn"]).ToStringExt(DateTimeFormat), Text = "Last Txn", Tooltip = "Last Transaction" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["pwdexpiry"]).ToStringExt(DateTimeFormat), Text = "Password" },
                    new ObjectInfo { Value = authorizationHandler.CanViewVNIP(lastLoginIp) ? lastLoginIp : string.Empty, Text = "Last Login IP" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["LastChangedSecurityCodeDate"]).ToStringExt(DateTimeFormat), Text = "Security Code" }
                };

            if (roleId.IsNotMember())
            {
                loginInfos.Add(new ObjectInfo
                {
                    Value = Utils.ToBoolean(customerRow["NicknameLoginOnly"]) ? "Nickname Only" : "Username/Nickname",
                    Text = "Login Options"
                });

                loginInfos.Add(new ObjectInfo
                {
                    Value = Utils.ToDateTime(customerRow["OTPCreateDate"]).ToStringExt(DateTimeFormat),
                    Text = "OTP"
                });
            }

            return loginInfos;
        }

        internal virtual PersonalInfo GetPersonalInfo(DataRow customerRow, DataRow lineRow)
        {
            var roleId = Utils.ToInt32(customerRow["roleid"]);
            var enabled = customerRow.ToBoolean("enabled");
            var closed = customerRow.ToBoolean("closed");
            var suspended = customerRow.ToBoolean("suspended");
            var status = ThisAccountWas;
            var closedReason = string.Empty;

            if (closed)
            {
                var customerClosedReason = new CustomerClosedReason(customerRow["closedreason"]);
                closedReason = customerClosedReason.ClassName;
                status = customerClosedReason.ClosedReason;
            }

            var personalInfo = new PersonalInfo
            {
                CustId = new ObjectInfo { Value = Utils.ToInt32(customerRow["custid"]), Text = "Cust Id" },
                Fax = new ObjectInfo { Value = Utils.ToString(customerRow["fax"]), Text = "Fax" },
                Mobile = new ObjectInfo { Value = Utils.ToString(customerRow["mobilephone"]), Text = "Mobile" },
                Name = new ObjectInfo { Value = Utils.ToString(customerRow["firstname"]) + " " + Utils.ToString(customerRow["lastname"]), Text = "Name" },
                Nickname = new ObjectInfo { Value = Utils.ToString(customerRow["username2"]), Text = "Nickname" },
                Phone = new ObjectInfo { Value = Utils.ToString(customerRow["phone"]), Text = "Phone" },
                RoleId = new ObjectInfo { Value = roleId, Text = "Role Id" },
                UplineRoleId = new ObjectInfo { Value = roleId + 1, Text = "Upline Role Id" },
                CustList = new ObjectInfo { Key = "CustList", Value = roleId.IsSuper() ? "Master List" : roleId.IsMaster() ? "Agent List" : "Member List" },
                Status = new ObjectInfo { Tooltip = !enabled ? status + "disabled" : closed ? status : suspended ? status + "suspended" : string.Empty },
                BackgroundColor = new ObjectInfo { Value = !enabled ? "disabled" : closed ? closedReason : suspended ? "suspended" : string.Empty },
                IsInternal = new ObjectInfo { Value = Utils.ToBoolean(customerRow["isinternal"]) }
            };

            SetLineInfo(personalInfo, lineRow);

            return personalInfo;
        }

        internal static List<ObjectInfo> GetDownlines(DataRow lineRow, int roleId)
        {
            if (roleId.IsSuper())
            {
                return new List<ObjectInfo>
                {
                    new ObjectInfo(lineRow.SuperId().ToString(), lineRow.SuperName(), "Super")
                };
            }

            if (roleId.IsMaster())
            {
                return new List<ObjectInfo>
                {
                    new ObjectInfo(lineRow.SuperId().ToString(), lineRow.SuperName(),  "Super"),
                    new ObjectInfo(lineRow.MasterId().ToString(), lineRow.MasterName(), "Master")
                };
            }

            return new List<ObjectInfo>
            {
                new ObjectInfo(lineRow.SuperId().ToString(), lineRow.SuperName(),  "Super"),
                new ObjectInfo(lineRow.MasterId().ToString(), lineRow.MasterName(),  "Master"),
                new ObjectInfo(lineRow.AgentId().ToString(), lineRow.AgentName(),  "Agent")
            };
        }

        internal abstract void SetLineInfo(PersonalInfo personalInfo, DataRow lineRow);

        /// <summary>
        /// this method should be overrided by each customer query service
        /// </summary>
        internal virtual void SetCustomSettingByLevel(Setting setting, int userId, DataSet dataSet)
        {
        }

    }
}