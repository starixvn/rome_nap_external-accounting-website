namespace Fanex.ExternalAccount.Module.LeftPanel.Extensions
{
    using System.Data;
    using Core.Shared;
    using ViewModels;

    public static class AgentModelExtensions
    {
        public static CustomerModel AgentInfo(this CustomerModel model, DataSet ds)
        {
            SetAgentPersonalInfo(model, ds);

            SetAgentBalanceInfo(model, ds);

            SetAgentSettings(model, ds);

            return model;
        }

        private static void SetAgentPersonalInfo(CustomerModel model, DataSet ds)
        {
            var lineRow = ds.Tables[1].Rows[0];

            model.PersonalInfo.UplineId = new ObjectInfo { Value = lineRow.MasterId(), Text = "Master ID" };
            model.PersonalInfo.CurrentLevel = new ObjectInfo { Value = lineRow.AgentName(), Text = "Agent" };
            model.PersonalInfo.Downlines = CustomerModelExtensions.GetDownlines(lineRow, Role.Agent);
        }

        private static void SetAgentBalanceInfo(CustomerModel model, DataSet ds)
        {
            var cashRow = ds.Tables[2].Rows[0];

            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalAgentBalance(),
                Text = "Total Balance"
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalAgentYesterdayCash(),
                Text = "Yesterday Balance"
            });
            model.BalanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.CustomerOutstanding(),
                Text = "Member Outstanding"
            });
        }

        private static void SetAgentSettings(CustomerModel model, DataSet ds)
        {
            var tableCustomer = ds.Tables[0];
            var table3rdDCS = ds.Tables[4];

            var customerRow = tableCustomer.Rows[0];
            model.Setting = new Setting
            {
                EnableItems = CustomerModelExtensions.EnableItems(customerRow, table3rdDCS),
                SyncItems = CustomerModelExtensions.SyncItems(customerRow, table3rdDCS),
                EditItems = CustomerModelExtensions.EditItems(),
                Details = CustomerModelExtensions.Details(Role.Agent)
            };

            CustomerModelExtensions.AddDCSProductSettings(model, table3rdDCS);
        }
    }
}