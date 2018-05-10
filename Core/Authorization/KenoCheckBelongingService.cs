namespace Fanex.ExternalAccount.Core.Authorization
{
    using CustomerSetting.Module.Keno.KenoSetting.Services;

    public class KenoCheckBelongingService : IKenoCheckBelongingService
    {
        public bool IsBelonging() => true;
    }
}