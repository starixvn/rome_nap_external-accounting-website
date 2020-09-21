namespace Fanex.CustomerSetting.Module.BetSetting.Base
{
    using Casino.Services.Impl;
    using Casino.Services.Interface;
    using Core.Web.Controllers;
    using Fanex.CustomerSetting.Module.Core.Models;
    using Fanex.CustomerSetting.Module.Core.Services.Impl;
    using Fanex.CustomerSetting.Module.Core.Services.Interface;
    using Fanex.ExternalAccount.Core.Context;
    using Keno.KenoSetting.Services;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Services.Impl;
    using Services.Interface;
    using System.Web.Mvc;
    using ViewModels;

    public class BettingControllerBase : BaseController
    {
        private readonly PTEngine.Services.IBettingLimitService _sportsBookService = new PTEngine.Services.BettingLimitService();

        private readonly IKenoSettingService _kenoService = new KenoSettingService();

        public PTEngine.Services.IBettingLimitService SportsBookService => _sportsBookService ?? new PTEngine.Services.BettingLimitService();

        public SuperServices superCasinoService => new SuperServicesImpl();

        public CasinoServiceBase casinoServiceBaseImpl => new CasinoServiceBaseImpl();

        public IKenoSettingService KenoService => _kenoService ?? new KenoSettingService();

        public IServiceBase serviceBase = new ServiceBase();

        public Customer CustomerInfo(int id, int userId)
        {
            ICustomerService customerService = new CustomerService();

            var customer = customerService.UserInfo(id);
            customer.GroupId = customerService.UserGroupId(userId);

            return customer;
        }

        protected ActionResult UmJsonResult(Result result)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return JsonBase(new { result.ErrorCode, result.ErrorMessage }, settings);
        }

        protected int GetDefaultMinBetValue()
            => CurrentSession.Instance.IsBelongToSpecialGroups ? 1 : 0;
    }
}