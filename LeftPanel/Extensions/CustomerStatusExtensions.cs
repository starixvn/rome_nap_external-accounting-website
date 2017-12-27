namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Module.Keno.KenoSetting.Configurations;
    using Service.Impl.Customer;
    using Service.Impl.Report;
    using Utilities;
    using ViewModels;

    public static class CustomerStatusExtensions
    {
        public static CustomerStatus GetCustomerStatus(int userId, string groupName, string username, int uplineId = 0)
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

                return GetCustStatus(roleId, custData, upperInfo);
            }

            return null;
        }

        private static CustomerStatus GetCustStatus(
            int roleId,
            DataTable result,
            DataTable upperInfo)
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

            foreach (var productStatus in LeftPanelProductIntegration.CreateProductStatusList(statusRow))
            {
                custStatus.ProductStatusDic.Add(productStatus.ProductId, productStatus);
            }

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
            => custStatus.HideColossusBets = !AppSettings.IsShowColossusBets
                                            || statusRow.ToBoolean("HideColossusBet");

        private static void SetCricketStatus(CustomerStatus custStatus, DataRow statusRow)
            => custStatus.HideCricket = statusRow.ToBoolean("HideCricket")
                                        || !AppSettings.IsShowCricket
                                        || !statusRow.ToBoolean("IsIMBC");

        private static void SetCockfightingStatus(CustomerStatus custStatus, DataRow statusRow)
            => custStatus.HideCockfighting = statusRow.ToBoolean("HideCockfighting") ||
                                             !AppSettings.IsShowCockfighting;

        private static void SetSportsbook2Status(CustomerStatus custStatus, DataRow statusRow)
            => custStatus.HideSportsbook2 = statusRow.ToBoolean("HideMuayThai") ||
                                            !AppSettings.IsShowSportsbook2;

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

        private static void SetEsportsStatus(CustomerStatus custStatus,
            ExternalAccountAuthenticateAttribute athenticationService, DataRow statusRow)
        {
            custStatus.HideEsports = !athenticationService.AllowReadByAuthenticationParam(eFunctions.ShowEnableEsports)
                                     || AppSettings.HideEsports;
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
            custStatus.HideGoldDeluxe = statusRow.ToBoolean("HideGoldDeluxe") || !AppSettings.IsShowGoldDeluxeSettingForCredit;
            custStatus.SyncGoldDeluxeDownlines = statusRow.ToBoolean("SyncedGoldDeluxeDownline");
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
                                          || !AppSettings.IsShowGoldDeluxeSettingForCredit;

            uplineStatus.DisableColossusBets = uplineRow.ToBoolean("DisableColossusBet");
            uplineStatus.Closed = uplineRow.ToBoolean("closed");
            uplineStatus.Suspended = uplineRow.ToBoolean("suspended");
            uplineStatus.HideVirtualSports = uplineRow.ToBoolean("hideVirtualSports");
            uplineStatus.DisableHRFixedOdds = uplineRow.ToBoolean("disableHRFixedOdds");
            uplineStatus.DisableESports = uplineRow.ToBoolean("DisableESports");

            foreach (var productStatus in LeftPanelProductIntegration.CreateProductStatusList(uplineRow))
            {
                uplineStatus.ProductStatusDic.Add(productStatus.ProductId, productStatus);
            }

            return uplineStatus;
        }
    }
}