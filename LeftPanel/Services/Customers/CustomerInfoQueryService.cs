namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Core.Authorization;
    using Core.Shared;
    using DTO.Customer;
    using Extensions;
    using Utilities;
    using ViewModels;

    public interface CustomerInfoQueryService
    {
        CustomerModel GetCustomerModel(int actorId, string custName, string groupName, DataSet customerInfoDataset);
    }

    public abstract class CustomerInfoQueryBaseService : CustomerInfoQueryService
    {
        private const string DateTimeFormat = "M/d/yyyy H:mm tt";
        private const string ThisAccountWas = "This account was ";

        protected readonly AuthorizationService authorizationService;
        protected readonly ProductService productService;

        protected CustomerInfoQueryBaseService(
            AuthorizationService authorizationService = null,
            ProductService productService = null)
        {
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.productService = productService ?? new ProductServiceImpl();
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
                HasDownline = Utils.ToBoolean(custInfoRow["HasDownline"]),
                UplineHasSecurityCode = Utils.ToBoolean(custInfoRow["UplineHasSecurityCode"]),
                CurrencyId = custInfoRow.ToInt32("exchangeid"),
                BalanceInfo = GetBalanceInfo(isUpdateCredit, custInfoRow, cashRow),
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

            SetCustomSetting(customerModel.Setting, actorId, customerInfoDataset);

            return customerModel;
        }

        internal BalanceInfo GetBalanceInfo(bool isUpdateCredit, DataRow customerRow, DataRow cashRow)
        {
            var roleId = Utils.ToInt32(customerRow["roleid"]);

            var balanceInfo = new BalanceInfo
            {
                CurrencyId = new ObjectInfo { Value = Utils.ToInt32(customerRow["exchangeid"]), Text = "Currency Id" },
                Currency = new ObjectInfo { Value = Utils.ToString(customerRow["currency"]), Text = "Currency" },
                Balances = new List<ObjectInfo>
                {
                    new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetCashColumn(roleId)]), Text = "Total Cash"},
                    new ObjectInfo
                    {
                        Value = Utils.ToDecimal(cashRow[GetYesterdayCashColumn(roleId)]),
                        Text = "Yesterday Cash"
                    },
                    new ObjectInfo
                    {
                        Value = Utils.ToDecimal(cashRow[GetOutstandingColumn(roleId)]),
                        Text = "Outstanding"
                    },
                    new ObjectInfo {Value = Utils.ToDecimal(customerRow["credit"]), Text = "Given Credit"}
                },
                IsUpdateCredit = isUpdateCredit
            };

            if (roleId.IsNotMember())
            {
                balanceInfo.Balances.Add(new ObjectInfo
                {
                    Value = cashRow[GetUsedCreditColumn(roleId)],
                    Text = "Used Credit"
                });

                balanceInfo.Balances.Add(new ObjectInfo
                {
                    Value = cashRow.TotalBalance(roleId),
                    Text = "Total Balance"
                });

                balanceInfo.Balances.Add(new ObjectInfo
                {
                    Value = cashRow.TotalYesterdayCash(roleId),
                    Text = "Yesterday Balance"
                });

                balanceInfo.Balances.Add(new ObjectInfo
                {
                    Value = cashRow.CustomerOutstanding(),
                    Text = $"{roleId.DirectDownlineLevel().DisplayRoleName()} Outstanding"
                });
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
            var roleId = Utils.ToInt32(customerRow["roleid"]);
            var lastLoginIp = customerRow.ToString("lastloginip");

            var loginInfos = new List<ObjectInfo>
            {
                new ObjectInfo
                {
                    Value = roleId.IsNotMember()
                        ? Utils.ToString(customerRow["site"])
                        : Utils.ToString(customerRow["membersite"]),
                    Text = "Site"
                },
                new ObjectInfo
                {
                    Value = Utils.ToDateTime(customerRow["createdate"]).ToStringExt(DateTimeFormat),
                    Text = "Created Date"
                },
                new ObjectInfo
                {
                    Value = Utils.ToDateTime(customerRow["lastlogged"]).ToStringExt(DateTimeFormat),
                    Text = "Last Login"
                },
                new ObjectInfo
                {
                    Value = Utils.ToDateTime(cashRow["lastTxn"]).ToStringExt(DateTimeFormat),
                    Text = "Last Txn",
                    Tooltip = "Last Transaction"
                },
                new ObjectInfo
                {
                    Value = Utils.ToDateTime(customerRow["pwdexpiry"]).ToStringExt(DateTimeFormat),
                    Text = "Password"
                },
                new ObjectInfo
                {
                    Value = authorizationService.CanViewVNIP(lastLoginIp) ? lastLoginIp : string.Empty,
                    Text = "Last Login IP"
                },
                new ObjectInfo
                {
                    Value = Utils.ToDateTime(customerRow["LastChangedSecurityCodeDate"]).ToStringExt(DateTimeFormat),
                    Text = "Security Code"
                }
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

        internal virtual PersonalInfo GetPersonalInfo(DataRow customerInfoRow, DataRow lineRow)
        {
            var role = customerInfoRow.RoleId();
            var enabled = customerInfoRow.Enabled();
            var closed = customerInfoRow.Closed();
            var suspended = customerInfoRow.Suspended();
            var status = ThisAccountWas;
            var closedReason = string.Empty;

            if (closed)
            {
                var customerClosedReason = new CustomerClosedReason(customerInfoRow["closedreason"]);
                closedReason = customerClosedReason.ClassName;
                status = customerClosedReason.ClosedReason;
            }

            var personalInfo = new PersonalInfo
            {
                CustId = new ObjectInfo { Value = Utils.ToInt32(customerInfoRow["custid"]), Text = "Cust Id" },
                Fax = new ObjectInfo { Value = Utils.ToString(customerInfoRow["fax"]), Text = "Fax" },
                Mobile = new ObjectInfo { Value = Utils.ToString(customerInfoRow["mobilephone"]), Text = "Mobile" },
                Name = new ObjectInfo
                {
                    Value = Utils.ToString(customerInfoRow["firstname"]) + " " + Utils.ToString(customerInfoRow["lastname"]),
                    Text = "Name"
                },
                Nickname = new ObjectInfo { Value = Utils.ToString(customerInfoRow["username2"]), Text = "Nickname" },
                Phone = new ObjectInfo { Value = Utils.ToString(customerInfoRow["phone"]), Text = "Phone" },
                RoleId = new ObjectInfo { Value = role, Text = "Role Id" },
                UplineRoleId = new ObjectInfo { Value = role.DirectUplineLevel(), Text = "Upline Role Id" },
                CustList = new ObjectInfo
                {
                    Key = "CustList",
                    Value = $"{role.DirectDownlineLevel().DisplayRoleName()} List"
                },
                Status = new ObjectInfo
                {
                    Tooltip = !enabled
                        ? status + "disabled"
                        : closed
                            ? status
                            : suspended
                                ? status + "suspended"
                                : string.Empty
                },
                BackgroundColor = new ObjectInfo
                {
                    Value = !enabled ? "disabled" : closed ? closedReason : suspended ? "suspended" : string.Empty
                },
                IsInternal = new ObjectInfo { Value = Utils.ToBoolean(customerInfoRow["isinternal"]) }
            };

            SetLineInfo(role, personalInfo, lineRow);

            return personalInfo;
        }

        internal static List<ObjectInfo> GetDownlines(DataRow lineRow, int roleId)
        {
            var downLines = new List<ObjectInfo>
            {
                new ObjectInfo(lineRow.CustomerId(Role.Super).ToString(), lineRow.CustomerName(Role.Super), Role.Super.DisplayRoleName())
            };

            if (roleId.IsSuper())
            {
                return downLines;
            }

            downLines.Add(new ObjectInfo(lineRow.CustomerId(Role.Master).ToString(), lineRow.CustomerName(Role.Master), roleId.DisplayRoleName()));

            if (roleId.IsNotMaster())
            {
                downLines.Add(new ObjectInfo(lineRow.CustomerId(Role.Agent).ToString(), lineRow.CustomerName(Role.Agent), roleId.DisplayRoleName()));
            }

            return downLines;
        }

        internal static void SetLineInfo(int role, PersonalInfo personalInfo, DataRow lineRow)
        {
            personalInfo.CurrentLevel = new ObjectInfo
            {
                Value = lineRow.CustomerName(role),
                Text = role.DisplayRoleName()
            };

            personalInfo.Downlines = GetDownlines(lineRow, role);

            if (role.IsNotSuper())
            {
                personalInfo.UplineId = new ObjectInfo
                {
                    Value = lineRow.CustomerId(role.DirectUplineLevel()),
                    Text = $"{role.DirectUplineLevel().DisplayRoleName()} ID"
                };
            }
        }

        /// <summary>
        ///     this method should be overrided by each customer query service
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="userId"></param>
        /// <param name="dataSet"></param>
        internal virtual void SetCustomSetting(Setting setting, int userId, DataSet dataSet)
        {
        }
    }
}