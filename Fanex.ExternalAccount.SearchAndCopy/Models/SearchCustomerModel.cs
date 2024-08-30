namespace Fanex.ExternalAccount.SearchAndCopy.Models
{
    public class SearchCustomerModel
    {
        public SearchCustomerModel()
        {
        }

        public SearchCustomerModel(int? custId, string username, int roleId, bool isExistGroup, bool isValidSuperPrefix)
        {
            CustId = custId;
            Username = username;
            RoleId = roleId;
            IsExistGroup = isExistGroup;
            IsValidSuperPrefix = isValidSuperPrefix;
        }

        public int? CustId { get; private set; }

        public string Username { get; private set; }

        public int RoleId { get; private set; }

        public bool IsExistGroup { get; private set; }

        public int Currency { get; private set; }

        public string Site { get; private set; }

        public bool IsValidSuperPrefix { get; private set; }
    }
}
