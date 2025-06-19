namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System.Net;
    using DataAccess.Criteria.Customer;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using DTO.Customer;
    using Fanex.ExternalAccount.Core.Shared;
    using Newtonsoft.Json;

    public interface IAssociationAccountService
    {
        AssociationAccountViewModel Get(GetAssociationAccountCriteria criteria);
    }

    public class AssociationAccountService : IAssociationAccountService
    {
        private readonly ICustomerDao customerDAO;
        private readonly IAssociationAccountDao associationAccountDAO;

        public AssociationAccountService() : this(null, null)
        {
        }

        public AssociationAccountService(ICustomerDao dataAccess, IAssociationAccountDao associationAccountDAO)
        {
            this.associationAccountDAO = associationAccountDAO ?? new AssociationAccountDao();
            customerDAO = dataAccess ?? new CustomerDao();
        }

        public AssociationAccountViewModel Get(GetAssociationAccountCriteria criteria)
        {
            var customer = customerDAO.GetCustomerByUserName(criteria.CustomerName, criteria.GroupName, criteria.UserId);
            if (customer != null)
            {
                var ctsClient = new WebClient();                
                ctsClient.QueryString.Add("CustId", customer.CustId.ToString());
                string content = ctsClient.DownloadString(AppSettings.CTS_Url);
                Logging.LogService.Error(content);
                criteria.AssociationList = JsonConvert.DeserializeObject<string>(content);
              
                if (!string.IsNullOrEmpty(criteria.AssociationList))
                {
                    return associationAccountDAO.Get(criteria);
                }
                ctsClient.Dispose();
            }

            return new AssociationAccountViewModel();
        }
    }
}