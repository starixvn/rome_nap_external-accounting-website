namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using System.Data;
    using Core.Shared;
    using CustomerSetting.Module.Keno.KenoSetting.Configurations;
    using Core.Authorization;
    using Service.Impl.Customer;
    using Service.Interface.Customer;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Utilities;
    using ViewModels;

    public interface CustomerStatusService
    {
        CustomerStatus GetCustomerStatus(int userId, string groupName, string username, int uplineId = 0);
    }

    public class CustomerStatusServiceImpl : CustomerStatusService
    {
        private readonly IReportService reportService;
        private readonly ISuperService superService;
        private readonly IMasterService masterService;
        private readonly IAgentService agentService;
        private readonly IMemberService memberService;
        private readonly AuthorizationService authorizationService;

        public CustomerStatusServiceImpl(
            IReportService reportService = null,
            ISuperService superService = null,
            IMasterService masterService = null,
            IAgentService agentService = null,
            IMemberService memberService = null,
            AuthorizationService authorizationService = null)
        {
            this.reportService = reportService ?? new ReportService();
            this.superService = superService ?? new SuperService();
            this.masterService = masterService ?? new MasterService();
            this.agentService = agentService ?? new AgentService();
            this.memberService = memberService ?? new MemberService();
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
        }

        public CustomerStatus GetCustomerStatus(int userId, string groupName, string username, int uplineId = 0)
        {
            var customerInfo = reportService.GetCustomerInfo(groupName, 0, username, true);

            if (customerInfo != null)
            {
                var infoRow = customerInfo.Rows[0];
                DataTable custData;
                DataTable upperInfo = null;

                var roleId = Utils.ToInt32(infoRow["roleid"]);
                var srecommend = Utils.ToInt32(infoRow["srecommend"]);
                var mrecommend = Utils.ToInt32(infoRow["mrecommend"]);
                var recommend = Utils.ToInt32(infoRow["recommend"]);
                var custId = Utils.ToInt32(infoRow["custid"]);

                switch (roleId)
                {
                    case Role.Super:
                        custData = superService.GetData(userId, custId, groupName: groupName);
                        break;

                    case Role.Master:
                        custData = masterService.GetData(userId, srecommend, custId);
                        upperInfo = superService.GetData(userId, uplineId, groupName: groupName);
                        break;

                    case Role.Agent:
                        custData = agentService.GetData(userId, mrecommend, custId);
                        upperInfo = masterService.GetData(userId, srecommend, uplineId);
                        break;

                    default:
                        custData = memberService.GetData(userId, recommend, custId);
                        upperInfo = agentService.GetData(userId, mrecommend, uplineId);
                        break;
                }

                return GetCustStatus(userId, roleId, custData, upperInfo);
            }

            return null;
        }

        private CustomerStatus GetCustStatus(
            int userId,
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

            var statusRow = result.Rows[0];

            SetNonProductStatus(roleId, custStatus, statusRow);

            SetNumberGameStatus(custStatus, statusRow);
            SetCasinoStatus(custStatus, statusRow);
            SetLiveCasinoStatus(custStatus, statusRow);
            SetVirtualSportsStatus(custStatus, statusRow);
            SetKenoStatus(custStatus, statusRow);
            SetEsportsStatus(custStatus, userId, statusRow);
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
                && upperInfo != null
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
            => custStatus.HideColossusBets = !ProductSettings.IsShowColossusBets
                                            || statusRow.ToBoolean("HideColossusBet");

        private static void SetCricketStatus(CustomerStatus custStatus, DataRow statusRow)
            => custStatus.HideCricket = statusRow.ToBoolean("HideCricket")
                                        || !ProductSettings.IsShowCricket
                                        || !statusRow.ToBoolean("IsIMBC");

        private static void SetCockfightingStatus(CustomerStatus custStatus, DataRow statusRow)
            => custStatus.HideCockfighting = statusRow.ToBoolean("HideCockfighting")
                                        || !ProductSettings.IsShowCockfighting;

        private static void SetSportsbook2Status(CustomerStatus custStatus, DataRow statusRow)
            => custStatus.HideSportsbook2 = statusRow.ToBoolean("HideMuayThai")
                                        || !ProductSettings.IsShowSportsbook2;

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

        private void SetEsportsStatus(
            CustomerStatus custStatus,
            int userId,
            DataRow statusRow)
        {
            custStatus.HideEsports = !authorizationService.IsAuthorized(userId, eFunctions.ShowEnableEsports)
                                     || ProductSettings.HideEsports;
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
            custStatus.HideGoldDeluxe = statusRow.ToBoolean("HideGoldDeluxe") || !ProductSettings.IsShowGoldDeluxeSettingForCredit;
            custStatus.SyncGoldDeluxeDownlines = statusRow.ToBoolean("SyncedGoldDeluxeDownline");
        }

        private static CustomerStatus GetUplineStatus(int roleId, DataRow uplineRow)
        {
            var uplineStatus = new CustomerStatus
            {
                RoleId = roleId + 1,
                SyncCasino = uplineRow.ToBoolean("syncCasino"),
                SyncLiveCasino = uplineRow.ToBoolean("syncLiveCasino"),
                DisableTransfer = (roleId + 1).IsNotSuper() && uplineRow.ToBoolean("disableTransfer"),
                AllowDoubleComm = uplineRow.ToBoolean("doubleCommLosing"),
                DisableVirtualSports = uplineRow.ToBoolean("disableVirtualSports"),
                AllowOutright = !uplineRow.ToBoolean("disableOutright"),
                DisableNumberGame = uplineRow.ToBoolean("disableNumber"),
                DisableCasino = uplineRow.ToBoolean("disableCasino"),
                DisableLiveCasino = uplineRow.ToBoolean("disableLiveCasino"),
                DisableGoldDeluxe = uplineRow.ToBoolean("DisableGoldDeluxe"),
                HideGoldDeluxe = uplineRow.ToBoolean("HideGoldDeluxe")
                                 || !ProductSettings.IsShowGoldDeluxeSettingForCredit,
                DisableColossusBets = uplineRow.ToBoolean("DisableColossusBet"),
                Closed = uplineRow.ToBoolean("closed"),
                Suspended = uplineRow.ToBoolean("suspended"),
                HideVirtualSports = uplineRow.ToBoolean("hideVirtualSports"),
                DisableHRFixedOdds = uplineRow.ToBoolean("disableHRFixedOdds"),
                DisableESports = uplineRow.ToBoolean("DisableESports")
            };

            foreach (var productStatus in LeftPanelProductIntegration.CreateProductStatusList(uplineRow))
            {
                uplineStatus.ProductStatusDic.Add(productStatus.ProductId, productStatus);
            }

            return uplineStatus;
        }
    }
}