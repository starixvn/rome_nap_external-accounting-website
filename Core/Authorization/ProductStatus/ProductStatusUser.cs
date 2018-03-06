namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System;
    using Context;
    using Fanex.ProductStatus.Setting.Services;

    public class ProductStatusUser : IProductStatusUser
    {
        public Func<int> GetActorId()
            => () => CurrentSession.Instance.UserId;

        public Func<string> GetClientIP()
            => () => CurrentContext.Instance.Ip4Address;
    }
}
