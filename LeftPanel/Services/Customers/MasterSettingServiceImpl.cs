namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System.Data;
    using Extensions;
    using ViewModels;

    public class MasterSettingServiceImpl : SettingServiceImpl
    {
        public override Setting GetSetting(int actorId, string custName, int uplineId, string groupName,
            DataSet customerInfoDataset)
        {
            var setting = base.GetSetting(actorId, custName, uplineId, groupName, customerInfoDataset);

            var cashRow = customerInfoDataset.Tables[2].Rows[0];
            setting.EnableItems.Insert(2, new ObjectInfo("AutoPT", cashRow.EnableAutoMasterPT(), "Allow Auto PT"));

            return setting;
        }
    }
}