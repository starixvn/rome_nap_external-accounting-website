namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Core.Context;
    using Core.Shared;
    using ViewModels;

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
                SyncItems = LeftPanelProductIntegration.CreateSyncItems(customerRow, table3rdDCS),
                EditItems = CustomerModelExtensions.EditItems(),
                Details = CustomerModelExtensions.Details(Role.Super)
            };

            model.AddProductSettings(table3rdDCS);

            AddBettingLimitForSuper(model, userId);

            model.AddDefaultMinMaxPTSetting();
        }

        private static void AddBettingLimitForSuper(CustomerModel model, int actorId)
        {
            if (AuthorizationService.Instance.HasReadPermissionOnFunctions(LeftPanelProductIntegration.VisibleBetSettingPermissions.Values, actorId))
            {
                model.Setting.Details.Add(new ObjectInfo("BetSetting", "Edit Betting Limit"));
            }
        }
    }
}