namespace Fanex.ExternalAccount.SearchAndCopy.Models
{
    public class SearchCustomerModel
    {
        public SearchCustomerModel()
        {
        }

        public SearchCustomerModel(int? custId, string username, int roleId)
        {
            CustId = custId;
            Username = username;
            RoleId = roleId;
        }

        public int? CustId { get; private set; }

        public string Username { get; private set; }

        public int RoleId { get; private set; }

        public bool IsExistGroup { get; private set; }
    }
}
