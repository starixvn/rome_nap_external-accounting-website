namespace Fanex.CustomerSetting.Module.BetSetting.Repositories.Interface
{
    using Fanex.CustomerSetting.Module.BetSetting.Criteria;
    using ViewModels;

    public interface ICustomerDao
    {
        Customer UserInfo(GetUserInfoCriteria criteria);
    }
}