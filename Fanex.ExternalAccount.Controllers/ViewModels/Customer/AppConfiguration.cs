namespace Fanex.ExternalAccount.Controllers.ViewModels.Customer
{
    public class AppConfiguration
    {
        public AppConfiguration(string appPath, int systemId = 4, int systemCode = 4)
        {
            ApplicationPath = appPath;
            SystemId = systemId;
            SystemCode = systemCode;
        }

        public string ApplicationPath { get; }

        public int SystemId { get; }

        public int SystemCode { get; }
    }
}