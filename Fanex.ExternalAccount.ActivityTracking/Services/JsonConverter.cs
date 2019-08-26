namespace Fanex.ExternalAccount.ActivityTracking.Services
{
    using Fanex.ActivityTracking.Services;

    public class JsonConverter : IJsonConverter
    {
        public string SerializeObject(object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }
    }
}
