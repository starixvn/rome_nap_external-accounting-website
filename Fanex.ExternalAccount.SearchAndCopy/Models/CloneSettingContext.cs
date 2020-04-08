namespace Fanex.ExternalAccount.SearchAndCopy.Models
{
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.SearchAndCopy.Requests;

    public class CloneSettingContext
    {
        public CloneSettingContext(CloneSettingRequest request, Result result, string creator)
        {
            Request = request;
            Result = result;
            Creator = creator;
        }

        public CloneSettingRequest Request { get; }

        public Result Result { get; }

        public string Creator { get; }
    }
}
