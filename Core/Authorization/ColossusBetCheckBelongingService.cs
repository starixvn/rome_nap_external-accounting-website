namespace Fanex.ExternalAccount.Core.Authorization
{
    using CustomerSetting.Module.ColossusBetSetting.Services;

    public class ColossusBetCheckBelongingService : IColossusBetCheckBelongingService
    {
        public bool IsBelonging() => true;
    }
}