namespace Fanex.ExternalAccount.Core.Base
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public static class ControllerExtentions
    {
        public static JsonBase JsonExt(this ControllerBase controller, object data, JsonSerializerSettings settings = null) =>
            new JsonBase(data, settings);
    }
}
