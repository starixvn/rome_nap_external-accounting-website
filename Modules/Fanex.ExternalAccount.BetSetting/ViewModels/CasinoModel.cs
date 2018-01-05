namespace Fanex.CustomerSetting.Module.BetSetting.ViewModels
{
    using Casino.Entities;
    using Casino.ViewModels;

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