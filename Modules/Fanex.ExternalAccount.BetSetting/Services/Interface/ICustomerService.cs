namespace Fanex.CustomerSetting.Module.BetSetting.Services.Interface
{
    using ViewModels;

    public interface ICustomerService
    {
        Customer UserInfo(int custId);
    }
}