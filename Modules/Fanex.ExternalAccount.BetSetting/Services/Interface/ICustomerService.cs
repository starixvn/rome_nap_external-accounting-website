namespace Fanex.CustomerSetting.Module.BetSetting.Services.Interface
{
    using Fanex.CustomerSetting.Module.BetSetting.ViewModels;

    public interface ICustomerService
    {
        Customer UserInfo(int custId);

        int UserGroupId(int custId);
    }
}
