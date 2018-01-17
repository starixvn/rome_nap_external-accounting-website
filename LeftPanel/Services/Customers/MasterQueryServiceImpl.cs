namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System.Data;
    using Extensions;
    using ViewModels;

    public class MasterInfoQueryServiceImpl : CustomerInfoQueryBaseService
    {
        internal override void SetCustomSetting(Setting setting, int userId, DataSet dataSet)
        {
            var cashRow = dataSet.Tables[2].Rows[0];
            setting.EnableItems.Insert(2, new ObjectInfo("AutoPT", cashRow.EnableAutoMasterPT(), "Allow Auto PT"));
        }
    }
}