namespace Fanex.ExternalAccount.DTO.Customer
{
    public class Customer: CustomerInfo
    {
        public int RoleId { get; set; }

        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public int AgentId { get; set; }
    }
}