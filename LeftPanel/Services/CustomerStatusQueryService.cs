namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Core.Authorization;
    using Core.Shared;
    using CustomerSetting.Module.Keno.KenoSetting.Configurations;
    using DataAccesses;
    using Extensions;
    using Models;
    using Service.Impl.Customer;
    using Service.Interface.Customer;
    using ViewModels;
    using static Core.Shared.ProductConstants;

    public interface CustomerStatusQueryService
    {
        CustomerStatus GetCustomerStatus(int userId, string groupName, string username, int uplineId = 0);
    }

    public class CustomerStatusServiceImpl : CustomerStatusQueryService
    {
        private readonly IAgentService agentService;
        private readonly AuthorizationService authorizationService;
        private readonly IMasterService masterService;
        private readonly IMemberService memberService;
        private readonly CustomerDataAccess dataAccess;
        private readonly ISuperService superService;
        private readonly Func<DataRow, bool, List<ProductStatus>> transformToProductStatusMapping;

        public CustomerStatusServiceImpl(
            CustomerDataAccess dataAccess = null,
            ISuperService superService = null,
            IMasterService masterService = null,
            IAgentService agentService = null,
            IMemberService memberService = null,
            AuthorizationService authorizationService = null,
            Func<DataRow, bool, List<ProductStatus>> transformToProductStatusMapping = null)
        {
            this.dataAccess = dataAccess ?? new CustomerDataAccessImpl();
            this.superService = superService ?? new SuperService();
            this.masterService = masterService ?? new MasterService();
            this.agentService = agentService ?? new AgentService();
            this.memberService = memberService ?? new MemberService();
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.transformToProductStatusMapping = transformToProductStatusMapping ?? AccountInfoProductConfiguration.TransformToProductStatus;
        }

        public CustomerStatus GetCustomerStatus(int userId, string groupName, string username, int uplineId = 0)
        {
            var customerInfo = dataAccess.EAS_CustomerSelUserInfo(groupName, 0, username, true);

            if (customerInfo == null)
            {
                return null;
            }

            var infoRow = customerInfo.Rows[0];
            DataTable custData;
            DataTable upperInfo = null;

            var roleId = infoRow.RoleId();
            var srecommend = infoRow.SRecommend();
            var mrecommend = infoRow.MRecommend();
            var recommend = infoRow.Recommend();
            var custId = infoRow.CustId();

            switch (roleId)
            {
                case Role.Super:
                    custData = superService.GetData(userId, groupName, custId);
                    break;

                case Role.Master:
                    custData = masterService.GetData(userId, srecommend, custId);
                    upperInfo = superService.GetData(userId, groupName, uplineId);
                    break;

                case Role.Agent:
                    custData = agentService.GetData(userId, mrecommend, custId);
                    upperInfo = masterService.GetData(userId, srecommend, uplineId);
                    break;

                default:
                    custData = memberService.GetData(userId, groupName, recommend, custId);
                    upperInfo = agentService.GetData(userId, mrecommend, uplineId);
                    break;
            }

            return GetCustStatus(userId, roleId, custData, upperInfo);
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
            SetVirtualSportsStatus(custStatus, statusRow);
            SetKenoStatus(custStatus, statusRow);
            SetEsportsStatus(custStatus, userId, statusRow);
            SetSportsbook2Status(custStatus, statusRow);
            SetCricketStatus(custStatus, statusRow);
            SetColossusBetsStatus(custStatus, statusRow);
            SetGoldDeluxeStatus(custStatus, statusRow);

            foreach (var productStatus in transformToProductStatusMapping(statusRow, false))
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
            custStatus.Closed = statusRow.Closed();
            custStatus.Suspended = statusRow.Suspended();
            custStatus.DisableTransfer = roleId.IsNotSuper() && statusRow.DisableTransfer();
            custStatus.AllowDoubleComm = statusRow.EnableDoubleComm();
            custStatus.AllowOutright = !statusRow.DisableOutright();
        }

        private static void SetNumberGameStatus(CustomerStatus custStatus, DataRow statusRow)
            => custStatus.DisableNumberGame = statusRow.DisableNumberGame();

        private static void SetColossusBetsStatus(CustomerStatus custStatus, DataRow statusRow)
            => custStatus.HideColossusBets = !ProductSettings.IsShowColossusBets || statusRow.HideColossusBet();

        private static void SetCricketStatus(CustomerStatus custStatus, DataRow statusRow)
            => custStatus.HideCricket = statusRow.HideCricket()
                                        || !ProductSettings.IsShowCricket
                                        || !statusRow.IsIMBC();
      
        private static void SetSportsbook2Status(CustomerStatus custStatus, DataRow statusRow)
            => custStatus.HideSportsbook2 = statusRow.HideMuayThai() || !ProductSettings.IsShowSportsbook2;

        private static void SetCasinoStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.SyncCasino = statusRow.SyncCasino();
            custStatus.DisableCasino = statusRow.DisableCasino();
        }

        private void SetEsportsStatus(
            CustomerStatus custStatus,
            int userId,
            DataRow statusRow)
        {
            custStatus.HideEsports = !authorizationService.IsAuthorized(userId, eFunctions.ShowEnableEsports)
                                     || ProductSettings.HideEsports;
            custStatus.DisableESports = statusRow.DisableESports();
        }

        private static void SetVirtualSportsStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideVirtualSports = statusRow.HideVirtualSports();
            custStatus.DisableVirtualSports = statusRow.DisableVirtualSports();            
        }

        private static void SetKenoStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideKeno = statusRow.HideKeno() || !KenoSection.Instance.ShowSetting;
            custStatus.DisableKeno = statusRow.DisableKeno();
        }

        private static void SetGoldDeluxeStatus(CustomerStatus custStatus, DataRow statusRow)
        {
            custStatus.HideGoldDeluxe = statusRow.HideGoldDeluxe() || !ProductSettings.IsShowProduct(GoldDeluxe);
            custStatus.SyncGoldDeluxeDownlines = statusRow.SyncedGoldDeluxeDownline();
        }

        private CustomerStatus GetUplineStatus(int roleId, DataRow uplineRow)
        {
            var uplineStatus = new CustomerStatus
            {
                RoleId = roleId + 1,
                SyncCasino = uplineRow.SyncCasino(),
                DisableTransfer = (roleId + 1).IsNotSuper() && uplineRow.DisableTransfer(),
                AllowDoubleComm = uplineRow.EnableDoubleComm(),
                DisableVirtualSports = uplineRow.DisableVirtualSports(),
                AllowOutright = !uplineRow.DisableOutright(),
                DisableNumberGame = uplineRow.DisableNumberGame(),
                DisableCasino = uplineRow.DisableCasino(),
                DisableGoldDeluxe = uplineRow.DisableGoldDeluxe(),
                HideGoldDeluxe = uplineRow.HideGoldDeluxe() || !ProductSettings.IsShowGoldDeluxe,
                DisableColossusBets = uplineRow.DisableColossusBet(),
                Closed = uplineRow.Closed(),
                Suspended = uplineRow.Suspended(),
                HideVirtualSports = uplineRow.HideVirtualSports(),                
                DisableESports = uplineRow.DisableESports()
            };

            foreach (var productStatus in transformToProductStatusMapping(uplineRow, false))
            {
                uplineStatus.ProductStatusDic.Add(productStatus.ProductId, productStatus);
            }

            return uplineStatus;
        }
    }
}