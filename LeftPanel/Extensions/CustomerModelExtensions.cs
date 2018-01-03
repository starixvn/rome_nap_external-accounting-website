namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.IP.Utils;
    using DTO.Customer;
    using Models;
    using Utilities;
    using ViewModels;

    public static class CustomerModelExtensions
    {
        private const string DateTimeFormat = "M/d/yyyy H:mm tt";
        private const string ThisAccountWas = "This account was ";
        public static readonly AuthorizationHandler AuthorizationHandler = new AuthorizationHandler();

        public static string GetOutstandingColumn(int roleId) => roleId.IsMember() ? "outstanding" : $"{roleId.DisplayRoleName()}PositionTaking";

        public static string GetUsedCreditColumn(int roleId)
        {
            if (roleId.IsMember())
            {
                throw new ArgumentOutOfRangeException(nameof(roleId), "there is no UsedCredit on member level");
            }

            return $"{roleId.DisplayRoleName()}UsedCredit";
        }

        public static string GetYesterdayCashColumn(int roleId)
            => roleId.IsMember() ? "yesterday_cash" : $"yesterday_{roleId.DisplayRoleName()}Cash";

        public static string GetCashColumn(int roleId)
            => roleId.IsMember() ? "cash" : $"{roleId.DisplayRoleName()}Cash";

        public static CustomerModel LoginInfo(this CustomerModel model, DataSet ds)
        {
            var tableCash = ds.Tables[2];
            var customerRow = ds.Tables[0].Rows[0];
            var roleId = Utils.ToInt32(customerRow["roleid"]);

            model.Login = new List<ObjectInfo>
                {
                    new ObjectInfo { Value = roleId.IsNotMember() ? Utils.ToString(customerRow["site"]) : Utils.ToString(customerRow["membersite"]), Text = "Site" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["createdate"]).ToStringExt(DateTimeFormat), Text = "Created Date" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["lastlogged"]).ToStringExt(DateTimeFormat), Text = "Last Login" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCash.Rows[0]["lastTxn"]).ToStringExt(DateTimeFormat), Text = "Last Txn", Tooltip = "Last Transaction" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["pwdexpiry"]).ToStringExt(DateTimeFormat), Text = "Password" },
                    new ObjectInfo { Value = CheckVNIP(customerRow.ToString("lastloginip")), Text = "Last Login IP" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["LastChangedSecurityCodeDate"]).ToStringExt(DateTimeFormat), Text = "Security Code" }
                };

            if (!roleId.IsNotMember())
            {
                return model;
            }

            model.Login.Add(
                new ObjectInfo
                {
                    Value = Utils.ToBoolean(customerRow["NicknameLoginOnly"]) ? "Nickname Only" : "Username/Nickname",
                    Text = "Login Options"
                });

            model.Login.Add(
                new ObjectInfo
                {
                    Value = Utils.ToDateTime(customerRow["OTPCreateDate"]).ToStringExt(DateTimeFormat),
                    Text = "OTP"
                });

            model.BalanceInfo.Balances.Add(new ObjectInfo { Value = tableCash.Rows[0][GetUsedCreditColumn(roleId)], Text = "Used Credit" });

            return model;
        }

        public static CustomerModel PersonalInfo(this CustomerModel model, DataSet ds)
        {
            var customerRow = ds.Tables[0].Rows[0];
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

            model.PersonalInfo = new PersonalInfo
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

            return model;
        }

        public static CustomerModel BalanceInfo(this CustomerModel model, DataSet ds)
        {
            var customerRow = ds.Tables[0].Rows[0];
            var roleId = Utils.ToInt32(customerRow["roleid"]);
            var cashRow = ds.Tables[2].Rows[0];

            model.BalanceInfo = new BalanceInfo
            {
                CurrencyId = new ObjectInfo {Value = Utils.ToInt32(customerRow["exchangeid"]), Text = "Currency Id"},
                Currency = new ObjectInfo {Value = Utils.ToString(customerRow["currency"]), Text = "Currency"},
                Balances = new List<ObjectInfo>
                {
                    new ObjectInfo {Value = Utils.ToDecimal(cashRow[GetCashColumn(roleId)]), Text = "Total Cash"},
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
                IsUpdateCredit = AuthorizationHandler.HasUpdateCreditPermission(CurrentSession.Instance.UserId)
            };


            return model;
        }

        public static CustomerModel RemoveNotSupportedProduct(this CustomerModel model, int userId, string groupName, string username)
        {
            var customerStatus = CustomerStatusExtensions.GetCustomerStatus(
                userId,
                groupName,
                username,
                (int?) model.PersonalInfo.UplineId?.Value ?? 0);

            model.RemoveProductSetting(customerStatus);

            if (customerStatus.RoleId.IsSuper())
            {
                model.RemoveProductNotVisibleToSite();

                return model;
            }

            model.DisableStatusSettingFollowUplineConstrain(customerStatus);

            return model;
        }

        public static string CheckVNIP(string ip)
         => string.IsNullOrWhiteSpace(ip)
            || (IpUtils.IsVnIp(ip)
                && !AuthorizationHandler.IsAuthorized(CurrentSession.Instance.UserId, eFunctions.ViewVNIP))
                    ? string.Empty : ip;

        public static CustomerModel AddProductSetting(
            this CustomerModel model,
            DataTable table3rdDCS,
            ProductSettingParam settingParams)
        {
            if (!IsProductVisibledToSite(table3rdDCS, settingParams.ProductId))
            {
                return model;
            }

            AddEditSettingLink(model, settingParams.ProductKey, settingParams.ProductName);

            if (settingParams.StatusType == StatusType.Checkbox)
            {
                AddEnableCheckboxItem(model, settingParams.ProductKey, !table3rdDCS.IsProductEnabled(settingParams.ProductId), settingParams.ProductName);
            }
            else
            {
                AddEnableLinkItem(model, settingParams.ProductKey, settingParams.ProductName);
            }

            return model;
        }

        private static void AddEditSettingLink(CustomerModel model, string productNameNoSpace, string productName)
            => model.Setting.Details.Add(new ObjectInfo { Key = productNameNoSpace, Value = $"Edit {productName}" });

        private static bool IsProductVisibledToSite(DataTable table3rdDCS, int productId)
            => table3rdDCS.Select($"ProductID = {productId}").Length > 0 && AppSettings.IsShowProduct(productId);

        private static void AddEnableLinkItem(CustomerModel model, string productNameNoSpace, string name)
            => model.Setting.EditItems.Add(new ObjectInfo
            {
                Key = $"Enable{productNameNoSpace}",
                Text = $"Enable {name}"
            });

        private static void AddEnableCheckboxItem(
            CustomerModel model,
            string productNameNoSpace,
            bool isDisabled,
            string name)
            => model.Setting.EnableItems.Add(new ObjectInfo
            {
                Key = $"Enable{productNameNoSpace}",
                Value = !isDisabled,
                Text = name,
                Tooltip = $"Enable {name}"
            });

        public static List<ObjectInfo> Details(int roleId)
        {
            var details = new List<ObjectInfo>
            {
                    new ObjectInfo ("Info", "Edit Info"),
                    new ObjectInfo ("PT", "Edit Position Taking")
            };

            if (roleId.IsNotMember())
            {
                details.Add(new ObjectInfo("AddNew", "Add New"));
                details.Add(new ObjectInfo("Transfer", "Edit Transfer Condition"));
            }

            details.Add(new ObjectInfo("Racing", "Edit Racing"));
            details.Add(new ObjectInfo("Casino", "Edit Casino"));
            details.Add(new ObjectInfo("LiveCasino", "Edit Live Casino"));
            details.Add(new ObjectInfo("VirtualSports", "Edit Virtual Sports"));
            details.Add(new ObjectInfo("Keno", "Edit Keno"));
            details.Add(new ObjectInfo("GoldDeluxe", "Edit Gold Deluxe"));
            details.Add(new ObjectInfo("ColossusBets", "Edit Colossus Bets"));
            details.Add(new ObjectInfo("Sportsbook2", "Edit Sportsbook 2"));
            details.Add(new ObjectInfo("Cockfighting", "Edit Cockfighting"));
            details.Add(new ObjectInfo("Cricket", "Edit Cricket"));

            return details;
        }

        public static List<ObjectInfo> EditItems()
            => new List<ObjectInfo>
            {
                new ObjectInfo { Key = "EnableRacing", Text = "Enable Racing" },
                new ObjectInfo { Key = "EnableKeno", Text = "Enable Keno" },
                new ObjectInfo { Key = "EnableCockfighting", Text = "Enable Cockfighting" }
            };

        public static List<ObjectInfo> EnableItems(DataRow customerRow, DataTable table3rdDCS)
            => new List<ObjectInfo>
            {
                new ObjectInfo("Closed", customerRow.Closed(),"Closed"),
                new ObjectInfo("Suspended",customerRow.Suspended(),"Suspened"),
                new ObjectInfo("Outright", customerRow.EnableOutright(),"Allow Outright"),
                new ObjectInfo("EnableCasino",  customerRow.EnableCasino(),"Enable Casino"),
                new ObjectInfo("EnableNumberGame", customerRow.EnableNumberGame(),"Enable Num Game", "Enable Number Game"),
                new ObjectInfo("EnableLiveCasino", customerRow.EnableLiveCasino(),"Enable Live Casino"),
                new ObjectInfo("EnableVirtualSports", table3rdDCS.GetVirtualSportsStatus(),"Virtual Sports"),
                new ObjectInfo("DoubleComm", customerRow.EnableDoubleComm(),"Double Comm",  "Enable Double Commission"),
                new ObjectInfo("EnableHRFixedOdds",  customerRow.EnableHorseRacingFixedOdds(),"HR Fixed Odds",  "Enable Horse Racing Fixed Odds"),
                new ObjectInfo("EnableESports",customerRow.EnableESports(),"E-Sports",  "Enable E-Sports"),
                new ObjectInfo("EnableColossusBets",  customerRow.EnableColossusBet(),"Colossus Bets", "Enable Colossus Bets" ),
                new ObjectInfo("EnableGoldDeluxe", customerRow.EnableGoldDeluxe(), "Gold Deluxe")
            };

        public static List<ObjectInfo> GetDownlines(DataRow lineRow, int roleId)
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

        public static void AddDefaultMinMaxPTSetting(this CustomerModel model)
        {
            model.Setting.MinMaxPts.Add(new ObjectInfo { Key = "MinSB", Value = "Sportsbook" });
            model.Setting.MinMaxPts.Add(new ObjectInfo { Key = "MinRB", Value = "Racing" });
        }

        public static void AddProductSettings(this CustomerModel model, DataTable productTable)
        {
            foreach (var productSettingParam in LeftPanelProductIntegration.ProductSettingParamList)
            {
                model.AddProductSetting(productTable, productSettingParam);
            }
        }
    }
}