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

        public int? CustId { get; set; }

        public string Username { get; set; }

        public int RoleId { get; set; }

        public bool IsExistGroup { get; set; }

        public int Currency { get; set; }

        public string Site { get; set; }

        public bool IsValidSuperPrefix { get; set; }
    }
}
