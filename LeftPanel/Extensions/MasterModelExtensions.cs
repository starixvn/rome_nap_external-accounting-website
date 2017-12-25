namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Core.Shared;
    using ViewModels;

    public static class MasterModelExtensions
    {
        public static CustomerModel MasterInfo(this CustomerModel model, DataSet ds)
        {
            SetMasterBalanceInfo(model, ds);

            SetMasterPersonal(model, ds);

            SetMasterSettings(model, ds);

            return model;
        }

        private static void SetMasterPersonal(CustomerModel model, DataSet ds)
        {
            var lineRow = ds.Tables[1].Rows[0];

            model.PersonalInfo.UplineId = new ObjectInfo { Value = lineRow.SuperId(), Text = "Super ID" };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = lineRow.MasterName(), Text = "Master" };
            model.PersonalInfo.Downlines = CustomerModelExtensions.GetDownlines(lineRow, Role.Master);
        }

        private static void SetMasterBalanceInfo(CustomerModel model, DataSet ds)
        {
            var cashRow = ds.Tables[2].Rows[0];

            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalMasterBalance(),
                Text = "Total Balance"
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalMasterYesterdayCash(),
                Text = "Yesterday Balance"
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.CustomerOutstanding(),
                Text = "Agent Outstanding"
            });
        }

        private static void SetMasterSettings(CustomerModel model, DataSet ds)
        {
            var table3rdDCS = ds.Tables[4];

            var customerRow = ds.Tables[0].Rows[0];

            var cashRow = ds.Tables[2].Rows[0];

            model.Setting = new Setting
            {
                EnableItems = CustomerModelExtensions.EnableItems(customerRow, table3rdDCS),
                SyncItems = LeftPanelProductIntegration.CreateSyncItems(customerRow, table3rdDCS),
                EditItems = CustomerModelExtensions.EditItems(),
                Details = CustomerModelExtensions.Details(Role.Master)
            };

            model.Setting.EnableItems.Insert(2, new ObjectInfo("AutoPT", cashRow.EnableAutoMasterPT(), "Allow Auto PT"));

            model.AddProductSettings(table3rdDCS);

            model.AddDefaultMinMaxPTSetting();
        }
    }
}