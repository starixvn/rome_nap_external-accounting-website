namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using Core.Context;
    using CustomerSetting.IP.Utils;
    using CustomerSetting.Security.Authenticate;
    using DTO;
    using DTO.Customer;
    using RemotingRef;
    using Service.Impl.UserControl;
    using Shared;
    using Utilities;
    using ViewModels;

    public static class CustomerModelExtensions
    {
        private const string DateTimeFormat = "M/d/yyyy H:mm tt";
        private const string ThisAccountWas = "This account was ";

        public static string GetOutstandingColumn(int roleId)
        {
            switch (roleId)
            {
                case Role.Super:
                    return "superPositionTaking";

                case Role.Master:
                    return "masterPositionTaking";

                case Role.Agent:
                    return "agentPositionTaking";

                case Role.Member:
                    return "outstanding";

                default:
                    throw new ArgumentOutOfRangeException(nameof(roleId));
            }
        }

        public static string GetUsedCreditColumn(int roleId)
        {
            switch (roleId)
            {
                case Role.Super:
                    return "superUsedCredit";

                case Role.Master:
                    return "masterUsedCredit";

                case Role.Agent:
                    return "agentUsedCredit";

                default:
                    throw new ArgumentOutOfRangeException(nameof(roleId), "there is no UsedCredit on member level");
            }
        }

        public static string GetYesterdayCashColumn(int roleId)
        {
            switch (roleId)
            {
                case Role.Super:
                    return "yesterday_superCash";

                case Role.Master:
                    return "yesterday_masterCash";

                case Role.Agent:
                    return "yesterday_agentCash";

                case Role.Member:
                    return "yesterday_cash";

                default:
                    throw new ArgumentOutOfRangeException(nameof(roleId));
            }
        }

        public static string GetCashColumn(int roleId)
        {
            switch (roleId)
            {
                case Role.Super:
                    return "superCash";

                case Role.Master:
                    return "masterCash";

                case Role.Agent:
                    return "agentCash";

                case Role.Member:
                    return "cash";

                default:
                    throw new ArgumentOutOfRangeException(nameof(roleId));
            }
        }

        public static bool IsAuthorized(int userId, string functionKey, AuthenticateAction.AuthenticationType policy)
        {
            var userCache = new CachingService().GetUserCache(userId);

            var menuList = userCache.MenuList;
            var foundItems = userCache.PolicyList.Where(x => x.FuncKey == functionKey).ToList();

            var hasPerm = menuList.Any(menuItem => !string.IsNullOrWhiteSpace(menuItem.Url));

            if (!hasPerm && foundItems.Any())
            {
                return false;
            }

            if (foundItems.Count <= 0)
            {
                return false;
            }

            var firstItem = foundItems.First();

            if (policy == AuthenticateAction.AuthenticationType.Read)
            {
                if (firstItem.Policy == EPolicy.Read || firstItem.Policy == EPolicy.Update ||
                    firstItem.Policy == EPolicy.Delete
                    || firstItem.Policy == EPolicy.Create)
                {
                    return true;
                }
            }
            else
            {
                return firstItem.Policy != EPolicy.Read;
            }

            return false;
        }

        public static CustomerModel LoginInfo(this CustomerModel model, DataSet ds)
        {
            var tableCash = ds.Tables[2];
            var customerRow = ds.Tables[0].Rows[0];
            var roleId = Utils.ToInt32(customerRow["roleid"]);

            model.Login = new List<ObjectInfo>
                {
                    new ObjectInfo { Value = roleId != 1 ? Utils.ToString(customerRow["site"]) : Utils.ToString(customerRow["membersite"]), Text = "Site" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["createdate"]).ToStringExt(DateTimeFormat), Text = "Created Date" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["lastlogged"]).ToStringExt(DateTimeFormat), Text = "Last Login" },
                    new ObjectInfo { Value = Utils.ToDateTime(tableCash.Rows[0]["lastTxn"]).ToStringExt(DateTimeFormat), Text = "Last Txn", Tooltip = "Last Transaction" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["pwdexpiry"]).ToStringExt(DateTimeFormat), Text = "Password" },
                    new ObjectInfo { Value = CheckVNIP(customerRow.ToString("lastloginip")), Text = "Last Login IP" },
                    new ObjectInfo { Value = Utils.ToDateTime(customerRow["LastChangedSecurityCodeDate"]).ToStringExt(DateTimeFormat), Text = "Security Code" }
                };

            if (roleId != 1)
            {
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
            }

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
                CustList = new ObjectInfo { Key = "CustList", Value = roleId == 4 ? "Master List" : roleId == 3 ? "Agent List" : "Member List" },
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
                CurrencyId = new ObjectInfo { Value = Utils.ToInt32(customerRow["exchangeid"]), Text = "Currency Id" },
                Currency = new ObjectInfo { Value = Utils.ToString(customerRow["currency"]), Text = "Currency" },
                Balances = new List<ObjectInfo>
                {
                        new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetCashColumn(roleId)]), Text = "Total Cash" },
                        new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetYesterdayCashColumn(roleId)]), Text = "Yesterday Cash" },
                        new ObjectInfo { Value = Utils.ToDecimal(cashRow[GetOutstandingColumn(roleId)]), Text = "Outstanding" },
                        new ObjectInfo { Value = Utils.ToDecimal(customerRow["credit"]), Text = "Given Credit" }
                    }
            };

            model.BalanceInfo.IsUpdateCredit
                = IsAuthorized(CurrentSession.Instance.UserId, eFunctions.UpdateCredit, AuthenticateAction.AuthenticationType.Host);

            return model;
        }

        public static CustomerModel BuildCustomerSetting(this CustomerModel model, int userId, string groupName, string username)
        {
            var customerStatus = model.GetCustomerStatus(
                userId,
                groupName,
                username,
                model.PersonalInfo.UplineId == null
                    ? 0
                    : (int)model.PersonalInfo.UplineId.Value);

            model.GetSuperMasterSettingOnMember();
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
                && !IsAuthorized(CurrentSession.Instance.UserId, eFunctions.ViewVNIP, AuthenticateAction.AuthenticationType.Read))
                    ? string.Empty : ip;

        public static CustomerModel AddProductSetting(
            this CustomerModel model,
            DataTable table3rdDCS,
            int productId,
            string defaultProductName,
            bool statusSettingIsCheckbox = true,
            bool getFromRemotingRef = false)
        {
            var productNameNoSpace = defaultProductName.Replace(" ", string.Empty);

            if (IsProductVisibledToSite(table3rdDCS, productId, defaultProductName))
            {
                var productDataRow = table3rdDCS.Select($"ProductID = {productId}")[0];
                var isDisabled = Utils.ToBoolean(productDataRow["isDisabled"]);
                var productName = GetProductName(productId, defaultProductName, getFromRemotingRef);

                AddEditSettingLink(model, productNameNoSpace, productName);

                if (statusSettingIsCheckbox)
                {
                    AddEnableCheckboxItem(model, productNameNoSpace, isDisabled, productName);
                }
                else
                {
                    AddEnableLinkItem(model, productNameNoSpace, productName);
                }
            }

            return model;
        }

        private static void AddEditSettingLink(CustomerModel model, string productNameNoSpace, string productName)
        => model.Setting.Details.Add(
                                new ObjectInfo
                                {
                                    Key = productNameNoSpace,
                                    Value = $"Edit {productName}"
                                });

        private static bool IsProductVisibledToSite(DataTable table3rdDCS, int productId, string defaultProductName)
         => table3rdDCS.Select("ProductID = " + productId).Length > 0
                && Utils.ToBoolean(ConfigurationManager.AppSettings["IsShow" + defaultProductName.Replace(" ", string.Empty)]);

        private static string GetProductName(int productId, string productName, bool getFromRemotingRef)
        => getFromRemotingRef
                ? RefData.GetResourceDataByRefIdAndResourceId(
                        "Product",
                        productId.ToString(),
                        "en-US",
                        productName)
                : productName;

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
    }
}