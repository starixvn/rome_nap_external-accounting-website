namespace Fanex.ExternalAccount.Service.Customer.CustomerList.Params
{
    public class SearchCustomerListParams
    {
        public string GroupName { get; set; }

        public int GroupId { get; set; }

        public string UserName { get; set; }

        public string CurrencyIds { get; set; }

        public short Status { get; set; }

        public bool IsDataLimited { get; set; }

        public bool IsNoDownlines { get; set; }

        public int UserId { get; set; }

        public int UplineId { get; set; }

        public int UplineRoleId { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
        
        public bool IsDirectMember { get; set; }

        public int ServiceType { get; set; }
    }
}
