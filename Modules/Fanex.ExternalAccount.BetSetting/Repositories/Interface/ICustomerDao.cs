namespace Fanex.CustomerSetting.Module.BetSetting.Repositories.Interface
{
    using Fanex.CustomerSetting.Module.BetSetting.ViewModels;

    public interface ICustomerDao
    {
        Customer UserInfo(int custId);

        int UserGroupId(int custId);
    }
}
