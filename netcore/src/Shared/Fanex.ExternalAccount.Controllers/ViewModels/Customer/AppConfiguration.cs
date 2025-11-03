namespace Fanex.ExternalAccount.Controllers.ViewModels.Customer
{
    public class AppConfiguration
    {
        public AppConfiguration(string appPath, int systemId)
        {
            ApplicationPath = appPath;
            SystemId = systemId;
        }

        public string ApplicationPath { get; }

        public int SystemId { get; }
    }
}