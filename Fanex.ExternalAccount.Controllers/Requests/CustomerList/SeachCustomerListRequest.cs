namespace Fanex.ExternalAccount.Controllers.Requests.CustomerList
{
    public class SeachCustomerListRequest
    {
        public string GroupName { get; set; }

        public int GroupId { get; set; }

        public string UserName { get; set; }

        public string CurrencyIds { get; set; }
        

        public short? Status { get; set; }

        public int? UplineRoleId { get; set; }

        public bool? IsShowSuperLimitDate { get; set; }

        public bool? IsNoDownlines { get; set; }

        public int? UplineId { get; set; }

        public int? PageSize { get; set; }

        public int? PageIndex { get; set; }

#pragma warning disable S109 // Magic numbers should not be used
        public string GetStatusString()
        {
            switch (Status)
            {
                case 1:
                    return "Open";
                case 2:
                    return "Suppended";
                case 3:
                    return "Closed";
                case 4:
                    return "Disabled";
                default:
                    return string.Empty;
            }
        }
#pragma warning restore S109 // Magic numbers should not be used
        
        public bool IsDirectMember { get; set; }

        public int ServiceType { get; set; }
    }
}
