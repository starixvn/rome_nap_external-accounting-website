namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using Core.Context;
    using CustomerSetting.Module.Keno.KenoSetting.Configurations;
    using Service.Impl.Customer;
    using Service.Impl.Report;
    using Shared;
    using Utilities;
    using ViewModels;

    public static class CustomerStatusExtensions
    {
        private static readonly List<string> CrossLineSupportedProducts = new List<string>
            {
                "PT",
                "Casino",
                "LiveCasino",
                "VirtualSports",
                "Racing",
            };

        public static CustomerStatus GetCustomerStatus(this CustomerModel model, int userId, string groupName, string username, int uplineId = 0)
        {
            var report = new ReportService();
            var customerInfo = report.GetCustomerInfo(groupName, 0, username, true);

            if (customerInfo != null)
            {
                var infoRow = customerInfo.Rows[0];
                DataTable custData = null;

                var upperInfo = uplineId != 0
                    ? report.GetCustomerInfo(groupName, uplineId, null, true)
                    : null;
                var roleId = Utils.ToInt32(infoRow["roleid"]);
                var srecommend = Utils.ToInt32(infoRow["srecommend"]);
                var mrecommend = Utils.ToInt32(infoRow["mrecommend"]);
                var recommend = Utils.ToInt32(infoRow["recommend"]);
                var custId = Utils.ToInt32(infoRow["custid"]);

                if (roleId.IsSuper())
                {
                    custData = new SuperService().GetData(userId, custId, groupName: groupName);
                }
                else if (roleId.IsMaster())
                {
                    custData = new MasterService().GetData(userId, srecommend, custId);
                    upperInfo = new SuperService().GetData(userId, uplineId, groupName: groupName);
                }
                else if (roleId.IsAgent())
                {
                    custData = new AgentService().GetData(userId, mrecommend, custId);
                    upperInfo = new MasterService().GetData(userId, srecommend, uplineId);
                }
                else
                {
                    custData = new MemberService().GetData(userId, recommend, custId);
                    upperInfo = new AgentService().GetData(userId, mrecommend, uplineId);
                }

                return model.GetCustStatus(roleId, custData, upperInfo);
            }

            return null;
        }

        private static CustomerStatus GetCustStatus(this CustomerModel model, int roleId, DataTable result, DataTable upperInfo)
        {
            var custStatus = new CustomerStatus
            {
                UplineStatus = new CustomerStatus(),
                RoleId = roleId
            };

            if (result.Rows.Count == 0)
            {
                return custStatus;
            }

            var athenticationService = new ExternalAccountAuthenticateAttribute();
            var statusRow = result.Rows[0];

            SetNonProductStatus(roleId, custStatus, statusRow);

            SetNumberGameStatus(custStatus, statusRow);
            SetCasinoStatus(custStatus, statusRow);
            SetLiveCasinoStatus(custStatus, statusRow);
            SetVirtualSportsStatus(custStatus, statusRow);
            SetKenoStatus(custStatus, statusRow);
            SetEsportsStatus(custStatus, athenticationService, statusRow);
            SetSportsbook2Status(custStatus, statusRow);
            SetCockfightingStatus(custStatus, statusRow);
            SetCricketStatus(custStatus, statusRow);
            SetColossusBetsStatus(custStatus, statusRow);
            SetGoldDeluxeStatus(custStatus, statusRow);
            SetAllbetStatus(custStatus, statusRow);
            SetVoidbridgeStatus(custStatus, statusRow);
            SetNG2Status(custStatus, statusRow);
            SetFishingWorldStatus(custStatus, statusRow);
            SetLotteryStatus(custStatus, statusRow);
            SetLucky3Status(custStatus, statusRow);

            model.Setting.MinMaxPts.Add(new ObjectInfo { Key = "MinSB", Value = "Sportsbook" });
            model.Setting.MinMaxPts.Add(new ObjectInfo { Key = "MinRB", Value = "Racing" });
            model.Setting.SuperPts = model.Setting.MasterPts = model.Setting.Details.Where(o => CrossLineSupportedProducts.Contains(o.Key)).ToList();

            if (roleId.IsNotSuper()
                && upperInfo.Rows.Count > 0)
            {
                custStatus.UplineStatus = GetUplineStatus(roleId, upperInfo.Rows[0]);
            }

            return custStatus;
        }

        private static void SetNonProductStatus(int roleId, CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.Closed = statusRow.ToBoolean("closed");
            custStatus.Suspended = statusRow.ToBoolean("suspended");
            custStatus.DisableTransfer = roleId.IsNotSuper() && statusRow.ToBoolean("disableTransfer");
            custStatus.AllowDoubleComm = statusRow.ToBoolean("doubleCommLosing");
            custStatus.AllowOutright = !statusRow.ToBoolean("disableOutright");
        }

        private static void SetNumberGameStatus(CustomerStatus custStatus, DataRow statusRow)
        => custStatus.DisableNumberGame = statusRow.ToBoolean("disableNumber");

        private static void SetColossusBetsStatus(CustomerStatus custStatus, DataRow statusRow)
        => custStatus.HideColossusBets = ConfigurationManager.AppSettings["IsShowColossusBets"] == "0" || statusRow.ToBoolean("HideColossusBet");

        private static void SetCricketStatus(CustomerStatus custStatus, DataRow statusRow)
        => custStatus.HideCricket = statusRow.ToBoolean("HideCricket")
                            || ConfigurationManager.AppSettings["IsShowCricket"] == "0" || !statusRow.ToBoolean("IsIMBC");

        private static void SetCockfightingStatus(CustomerStatus custStatus, DataRow statusRow)
        => custStatus.HideCockfighting = statusRow.ToBoolean("HideCockfighting") || ConfigurationManager.AppSettings["IsShowCockfighting"] == "0";

        private static void SetSportsbook2Status(CustomerStatus custStatus, DataRow statusRow)
        => custStatus.HideSportsbook2 = statusRow.ToBoolean("HideMuayThai") || ConfigurationManager.AppSettings["IsShowSportsbook2"] == "0";

        private static void SetLiveCasinoStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.SyncLiveCasino = statusRow.ToBoolean("syncLiveCasino");
            custStatus.DisableLiveCasino = statusRow.ToBoolean("disableLiveCasino");
        }

        private static void SetCasinoStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.SyncCasino = statusRow.ToBoolean("syncCasino");
            custStatus.DisableCasino = statusRow.ToBoolean("disableCasino");
        }

        private static void SetEsportsStatus(CustomerStatus custStatus, ExternalAccountAuthenticateAttribute athenticationService, DataRow statusRow)
        {
            custStatus.HideEsports = !athenticationService.AllowReadByAuthenticationParam(eFunctions.ShowEnableEsports)
                || Utils.ToBoolean(ConfigurationManager.AppSettings["HideEsports"]);
            custStatus.DisableESports = statusRow.ToBoolean("DisableESports");
        }

        private static void SetVirtualSportsStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideVirtualSports = statusRow.ToBoolean("hideVirtualSports");
            custStatus.DisableVirtualSports = statusRow.ToBoolean("disableVirtualSports");
            custStatus.DisableHRFixedOdds = statusRow.ToBoolean("disableHRFixedOdds");
        }

        private static void SetKenoStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideKeno = statusRow.ToBoolean("HideKeno") || !KenoSection.Instance.ShowSetting;
            custStatus.DisableKeno = statusRow.ToBoolean("DisableKeno");
        }

        private static void SetGoldDeluxeStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideGoldDeluxe = statusRow.ToBoolean("HideGoldDeluxe")
                || ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"] == "0";
            custStatus.SyncGoldDeluxeDownlines = statusRow.ToBoolean("SyncedGoldDeluxeDownline");
        }

        private static void SetLucky3Status(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideLucky3 = ConfigurationManager.AppSettings["IsShowLucky3"] == "0"
                || statusRow.ToBoolean("HideLucky3");
            custStatus.SyncLucky3 = true;
            custStatus.SyncLucky3Downlines = true;
            custStatus.DisableLucky3 = statusRow.ToBoolean("DisableLucky3");
        }

        private static void SetLotteryStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideLottery = ConfigurationManager.AppSettings["IsShowLottery"] == "0"
                || statusRow.ToBoolean("HideLottery");
            custStatus.SyncLottery = true;
            custStatus.SyncLotteryDownlines = true;
            custStatus.DisableLottery = statusRow.ToBoolean("DisableLottery");
        }

        private static void SetFishingWorldStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideFishingWorld = ConfigurationManager.AppSettings["IsShowFishingWorld"] == "0"
                || statusRow.ToBoolean("HideFishingWorld");
            custStatus.SyncFishingWorld = statusRow.ToBoolean("SyncedFishingWorld");
            custStatus.SyncFishingWorldDownlines = statusRow.ToBoolean("SyncedFishingWorldDownline");
            custStatus.DisableFishingWorld = statusRow.ToBoolean("DisableFishingWorld");
        }

        private static void SetNG2Status(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideNG2 = ConfigurationManager.AppSettings["IsShowNG2"] == "0"
                || statusRow.ToBoolean("HideNG2");
            custStatus.SyncNG2 = statusRow.ToBoolean("SyncedNG2");
            custStatus.DisableNG2 = statusRow.ToBoolean("DisableNG2");
        }

        private static void SetVoidbridgeStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideVoidbridge = ConfigurationManager.AppSettings["IsShowVoidbridge"] == "0"
                || statusRow.ToBoolean("HideVoidBridge");
            custStatus.SyncVoidbridge = statusRow.ToBoolean("SyncedVoidBridge");
            custStatus.SyncVoidbridgeDownlines = statusRow.ToBoolean("SyncedVoidBridgeDownline");
            custStatus.DisableVoidbridge = statusRow.ToBoolean("DisableVoidBridge");
        }

        private static void SetAllbetStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideAllbet = ConfigurationManager.AppSettings["IsShowAllbet"] == "0"
                || statusRow.ToBoolean("HideAllbet");
            custStatus.SyncAllbet = statusRow.ToBoolean("SyncedAllBet");
            custStatus.SyncAllbetDownlines = statusRow.ToBoolean("SyncedAllBetDownline");
            custStatus.DisableAllbet = statusRow.ToBoolean("DisableAllbet");
        }

        private static CustomerStatus GetUplineStatus(int roleId, DataRow uplineRow)
        {
            var uplineStatus = new CustomerStatus();

            uplineStatus.RoleId = roleId + 1;
            uplineStatus.SyncCasino = uplineRow.ToBoolean("syncCasino");
            uplineStatus.SyncLiveCasino = uplineRow.ToBoolean("syncLiveCasino");
            uplineStatus.DisableTransfer = roleId + 1 != 4 && uplineRow.ToBoolean("disableTransfer");
            uplineStatus.AllowDoubleComm = uplineRow.ToBoolean("doubleCommLosing");
            uplineStatus.DisableVirtualSports = uplineRow.ToBoolean("disableVirtualSports");
            uplineStatus.AllowOutright = !uplineRow.ToBoolean("disableOutright");
            uplineStatus.DisableNumberGame = uplineRow.ToBoolean("disableNumber");
            uplineStatus.DisableCasino = uplineRow.ToBoolean("disableCasino");
            uplineStatus.DisableLiveCasino = uplineRow.ToBoolean("disableLiveCasino");
            uplineStatus.DisableGoldDeluxe = uplineRow.ToBoolean("DisableGoldDeluxe");
            uplineStatus.HideGoldDeluxe = uplineRow.ToBoolean("HideGoldDeluxe")
                || ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"] == "0";

            uplineStatus.DisableColossusBets = uplineRow.ToBoolean("DisableColossusBet");
            uplineStatus.Closed = uplineRow.ToBoolean("closed");
            uplineStatus.Suspended = uplineRow.ToBoolean("suspended");
            uplineStatus.HideVirtualSports = uplineRow.ToBoolean("hideVirtualSports");
            uplineStatus.DisableHRFixedOdds = uplineRow.ToBoolean("disableHRFixedOdds");
            uplineStatus.DisableESports = uplineRow.ToBoolean("DisableESports");

            SetUplineAllBetStatus(uplineStatus, uplineRow);
            SetUplineVoidbridgeStatus(uplineStatus, uplineRow);
            SetUplineNG2Status(uplineStatus, uplineRow);
            SetUplineFishingWorldStatus(uplineStatus, uplineRow);
            SetUplineLotteryStatus(uplineStatus, uplineRow);
            SetUplineLucky3Status(uplineStatus, uplineRow);

            return uplineStatus;
        }

        private static void SetUplineLucky3Status(CustomerStatus uplineStatus, DataRow uplineRow)
        {
            uplineStatus.DisableLucky3 = uplineRow.ToBoolean("DisableLucky3");
            uplineStatus.HideLucky3 = uplineRow.ToBoolean("HideLucky3");
            uplineStatus.SyncLucky3 = true;
        }

        private static void SetUplineLotteryStatus(CustomerStatus uplineStatus, DataRow uplineRow)
        {
            uplineStatus.DisableLottery = uplineRow.ToBoolean("DisableLottery");
            uplineStatus.HideLottery = uplineRow.ToBoolean("HideLottery");
            uplineStatus.SyncLottery = true;
        }

        private static void SetUplineFishingWorldStatus(CustomerStatus uplineStatus, DataRow uplineRow)
        {
            uplineStatus.DisableFishingWorld = uplineRow.ToBoolean("DisableFishingWorld");
            uplineStatus.HideFishingWorld = uplineRow.ToBoolean("HideFishingWorld");
            uplineStatus.SyncFishingWorld = uplineRow.ToBoolean("SyncedFishingWorld");
        }

        private static void SetUplineNG2Status(CustomerStatus uplineStatus, DataRow uplineRow)
        {
            uplineStatus.DisableNG2 = uplineRow.ToBoolean("DisableNG2");
            uplineStatus.HideNG2 = uplineRow.ToBoolean("HideNG2");
            uplineStatus.SyncNG2 = uplineRow.ToBoolean("SyncedNG2");
        }

        private static void SetUplineVoidbridgeStatus(CustomerStatus uplineStatus, DataRow uplineRow)
        {
            uplineStatus.DisableVoidbridge = uplineRow.ToBoolean("DisableVoidBridge");
            uplineStatus.HideVoidbridge = uplineRow.ToBoolean("HideVoidBridge");
            uplineStatus.SyncVoidbridge = uplineRow.ToBoolean("SyncedVoidBridge");
        }

        private static void SetUplineAllBetStatus(CustomerStatus uplineStatus, DataRow uplineRow)
        {
            uplineStatus.DisableAllbet = uplineRow.ToBoolean("DisableAllbet");
            uplineStatus.HideAllbet = uplineRow.ToBoolean("HideAllbet");
            uplineStatus.SyncAllbet = uplineRow.ToBoolean("SyncedAllBet");
        }
    }
}