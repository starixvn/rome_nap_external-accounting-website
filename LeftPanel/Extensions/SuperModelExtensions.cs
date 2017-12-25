namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using ViewModels;
    using static eFunctions;

    public static class SuperModelExtensions
    {
        public static CustomerModel SuperInfo(this CustomerModel model, DataSet ds, int actorId)
        {
            SetSuperBalanceInfo(model, ds);

            SetSuperPersonalInfo(model, ds);

            SetSuperSettings(model, ds, actorId);

            return model;
        }

        private static void SetSuperPersonalInfo(CustomerModel model, DataSet ds)
        {
            var lineRow = ds.Tables[1].Rows[0];

            model.PersonalInfo.Downlines = CustomerModelExtensions.GetDownlines(lineRow, Role.Super);
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = lineRow.SuperName(), Text = "Super" };
        }

        private static void SetSuperBalanceInfo(CustomerModel model, DataSet ds)
        {
            var cashRow = ds.Tables[2].Rows[0];

            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalSuperBalance(),
                Text = "Total Balance"
            });

            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalSuperYesterdayCash(),
                Text = "Yesterday Balance"
            });

            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.CustomerOutstanding(),
                Text = "Master Outstanding"
            });
        }

        private static void SetSuperSettings(CustomerModel model, DataSet ds, int userId)
        {
            var tableCustomer = ds.Tables[0];
            var table3rdDCS = ds.Tables[4];
            var customerRow = tableCustomer.Rows[0];

            model.Setting = new Setting
            {
                EnableItems = CustomerModelExtensions.EnableItems(customerRow, table3rdDCS),
                SyncItems = CustomerModelExtensions.SyncItems(customerRow, table3rdDCS),
                EditItems = CustomerModelExtensions.EditItems(),
                Details = CustomerModelExtensions.Details(Role.Super)
            };

            CustomerModelExtensions.AddDCSProductSettings(model, table3rdDCS);

            AddBettingLimitForSuper(model, userId);
        }

        private static readonly List<string> BetLimitSettingProductKeys = new List<string>
        {
            CasinoBetting,
            GoldDeluxeBetting,
            KenoBetting,
            LiveCasinoBetting,
            RacingBetting,
            SportsbookBetting,
            VirtualSportsBetting,
            AllbetBetting,
            VoidbridgeBetting,
            NG2Betting,
            LotteryBetting
        };

        private static void AddBettingLimitForSuper(CustomerModel model, int actorId)
        {
            var showBetSetting = BetLimitSettingProductKeys.Any(
                productKey => CustomerModelExtensions.AuthorizationHandler.IsAuthorized(actorId, productKey, AuthenticateAction.AuthenticationType.Read));

            if (showBetSetting)
            {
                model.Setting.Details.Add(new ObjectInfo("BetSetting", "Edit Betting Limit"));
            }
        }
    }
}