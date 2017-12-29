namespace Fanex.CustomerSetting.Module.BetSetting.ViewModels
{
    using Fanex.CustomerSetting.Module.Casino.Entities;
    using Fanex.CustomerSetting.Module.Casino.ViewModels;

    public class CasinoModel
    {
        public CasinoSettingModel SettingModel
        {
            get;
            set;
        }

        public CasinoCustomer CustomerModel
        {
            get;
            set;
        }
    }
}
