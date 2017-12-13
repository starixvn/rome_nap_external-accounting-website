namespace Fanex.ExternalAccount.Core.Base
{
    using System.Web.Mvc;
    using Newtonsoft.Json;

    public static class ControllerExtentions
    {
        public static JsonBase JsonExt(this Controller controller, object data, JsonSerializerSettings settings = null)
        {
            return new JsonBase(data, settings);
        }
    }
}