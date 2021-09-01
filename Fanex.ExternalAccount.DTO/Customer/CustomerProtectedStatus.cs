namespace Fanex.ExternalAccount.DTO.Customer
{
    public class CustomerProtectedStatus
    {
        public CustomerProtectedStatus()
        {
        }

        public CustomerProtectedStatus(
            int custId,
            string username,
            byte roleId,
            bool isProtectedMode)
        {
            CustId = custId;
            Username = username;
            RoleId = roleId;
            IsProtectedMode = isProtectedMode;
        }

        public int CustId { get; set; }

        public string Username { get; set; }

        public byte RoleId { get; set; }

        public bool IsProtectedMode { get; set; }
    }
}
