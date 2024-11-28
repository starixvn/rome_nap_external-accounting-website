namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;

    public static class Config
    {
        public static DateTime WinlostDate
#pragma warning disable S109 // Magic numbers should not be used
            => HttpContext.Current.IsDebuggingEnabled ? new DateTime(2012, 11, 6).Date : DateTime.Now.AddDays(-1).Date;

#pragma warning restore S109 // Magic numbers should not be used

        public static bool IsTest => Utils.ToBoolean(GetValue("IsTest"));

        public static string LbUrl => GetValue("LBUrl");

        public static string TimeFrom { get; set; }

        public static string TimeTo { get; set; }

        public static bool IsEnabledRecheckin => Utils.ToBoolean(GetValue("Recheckin"));

        public static bool IsShowVirtualSports => Utils.ToBoolean(GetValue("EnableVirtualSports"));

        public static string CssFiles => GetValue("CssFiles");

        public static string JsFiles => GetValue("JsFiles");

        public static string JsLogin => GetValue("JsLogin");

        public static string CssLogin => GetValue("CssLogin");

        public static string JsExtGroup => GetValue("JsExtGroup");

        public static string CssExtGroup => GetValue("CssExtGroup");

        public static string JsDailyWinLoss => GetValue("JsDailyWinLoss");

        public static string CssDailyWinLoss => GetValue("CssDailyWinLoss");

        public static string JsWinLossDetail => GetValue("JsWinLossDetail");

        public static string CssWinLossDetail => GetValue("CssWinLossDetail");

        public static string JsStatement => GetValue("JsStatement");

        public static string CssStatement => GetValue("CssStatement");

        public static string JsCustomer => GetValue("JsCustomer");

        public static string CssCustomer => GetValue("CssCustomer");

        public static string InternalAgentUMToken => GetValue("int_agent_UMToken");

        public static string InternalLoginUrl => GetValue("int_agent_url");

        public static string InternalLoginUrl7 => GetValue("int_agent_url7");

        public static string InternalLoginUrl9 => GetValue("int_agent_url9");

        public static string InternalLoginUrl8 => GetValue("int_agent_url8");

        public static bool IsCache => Utils.ToBoolean(GetValue("IsCache"));

        public static IList<int> AdminUsersEditSuperStatus
        {
            get
            {
                var ids = GetValue("AdminUsersEditSuperStatus")
                    .Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                IList<int> adminIds = new List<int>();

                foreach (var id in ids)
                {
                    adminIds.Add(Utils.ToInt32(id));
                }

                return adminIds;
            }
        }

        public static int SystemId => Utils.ToInt32(GetValue("SystemId"));

        public static byte SettingStatus => Utils.ToByte(GetValue("SettingStatus"));

        public static string[] PublicDomains => GetValue("PublicDomains").Split(',');

        public static bool IsRestrictAccessPublicDomain => Utils.ToBoolean(GetValue("IsRestrictAccessPublicDomain"));

        public static string IgnoreRecheckinCustomers
        {
            get
            {
                return GetValue("IgnoreRecheckinCustomers");
            }
        }

        public static string[] SpecialGroups()
        {
            return GetValue("SpecialGroups").Split(',').Select(g => g.Trim()).ToArray();
        }

        public static IList<int> ProductOfferProductDetails()
        {
            var allProducts = GetValue("ProductOfferProductDetails");

            return allProducts.Split(',').Select(id => Utils.ToInt32(id))
                .ToList();
        }

        public static bool IsOfferProductDetailGroup(string currentGroup)
        {
            var allGroups = GetValue("OfferProductDetailsGroups");

            return "*" == allGroups || IsOfferProductDetailsGroup(allGroups, currentGroup);
        }

        private static bool IsOfferProductDetailsGroup(string allGroups, string currentGroup)
            => !string.IsNullOrWhiteSpace(allGroups)
                && allGroups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(g => g.Trim())
                    .Contains(currentGroup, StringComparer.CurrentCultureIgnoreCase);

        public static IDictionary<int, bool> SpecialProductIds()
            => GetValue("SpecialProductIds")
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .ToDictionary(kvp => int.Parse(kvp), kvp => true);

        public static int SearchAndCopySuperMailingListId => Utils.ToInt32(GetValue("SearchAndCopySuperMailingListId"));

        public static string[] GroupsCanViewWinLossDetailForIBCBetLiveCasino()
        {
            return GetValue("GroupsCanViewWinLossDetailForIBCBetLiveCasino")
                .Split(',')
                .Select(group => group.Trim())
                .ToArray();
        }

        public static string[] GroupsCanViewExtraPT()
        {
            return GetValue("GroupsCanViewExtraPT")
                .Split(',')
                .Select(group => group.Trim())
                .ToArray();
        }

        public static string[] TestGroups()
        {
            return GetValue("TestGroups").Split(',').ToArray();
        }

        public static bool IsMultipleCurrencyGroup(string currentGroup)
        {
            var allGroups = GetValue("MultipleCurrencyGroup");

            return "*" == allGroups || !string.IsNullOrWhiteSpace(allGroups)
                && allGroups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(g => g.Trim())
                    .Contains(currentGroup, StringComparer.CurrentCultureIgnoreCase);
        }
        
        private static string GetValue(string key) => ConfigurationManager.AppSettings[key];
        
        public static bool IsMalaysiaGroup(string currentGroup)
        {
            var allGroups = GetValue("MalaysiaGroups");

            return "*" == allGroups || !string.IsNullOrWhiteSpace(allGroups)
                && allGroups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(g => g.Trim())
                    .Contains(currentGroup, StringComparer.CurrentCultureIgnoreCase);
        }
    }
}