namespace Fanex.ExternalAccount.Core.Authorization
{
    using CustomerSetting.Module.ColossusBetSetting.Services;

    public class ColossusBetCheckBelongingService : IColossusBetCheckBelongingService
    {
        private readonly AuthorizationService authorization = new AuthorizationServiceImpl();

        public bool IsBelonging() => authorization.IsBelong();
    }
}