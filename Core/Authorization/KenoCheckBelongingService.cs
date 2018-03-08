namespace Fanex.ExternalAccount.Core.Authorization
{
    using CustomerSetting.Module.Keno.KenoSetting.Services;

    public class KenoCheckBelongingService : IKenoCheckBelongingService
    {
        private readonly AuthorizationService authorization = new AuthorizationServiceImpl();

        public bool IsBelonging() => authorization.IsBelong();
    }
}