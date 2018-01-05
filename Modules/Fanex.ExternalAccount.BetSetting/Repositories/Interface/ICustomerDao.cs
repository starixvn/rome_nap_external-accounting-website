namespace Fanex.CustomerSetting.Module.BetSetting.Repositories.Interface
{
    using ViewModels;

    public interface ICustomerDao
    {
        Customer UserInfo(int custId);

        int UserGroupId(int custId);
    }
}