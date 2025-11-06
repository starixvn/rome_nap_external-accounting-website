namespace Fanex.ExternalAccount.Core.Context
{
    using Microsoft.AspNetCore.Http;
    using Helpers;

    /// <summary>
    /// Helper class to initialize HttpContextAccessor for static context classes
    /// Call this in Program.cs or Startup.cs after services are configured
    /// </summary>
    public static class HttpContextInitializer
    {
        public static void Initialize(IHttpContextAccessor httpContextAccessor)
        {
            CurrentSession.SetHttpContextAccessor(httpContextAccessor);
            CurrentContext.SetHttpContextAccessor(httpContextAccessor);
            ExtRequest.SetHttpContextAccessor(httpContextAccessor);
            Encryptor.SetHttpContextAccessor(httpContextAccessor);
            RequestHelper.SetHttpContextAccessor(httpContextAccessor);
            Authorization.AuthorizationServiceImpl.SetHttpContextAccessor(httpContextAccessor);
        }
    }
}

