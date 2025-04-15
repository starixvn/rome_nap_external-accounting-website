namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using Fanex.ExternalAccount.DTO.Customer.CustomerList;
    using System.Collections.Generic;
    using System.IO;

    public class CustomerListPageBuilder
    {
        public CustomerListPageBuilder(List<CustomerListDTO> customerList, bool isBold, string status, int roleId, int uplineRoleId, bool withDataLimited = false,bool IsNoDownlines =false, bool hasSubGroups = false, bool isMultipleCurrencyGroup = false, bool isDirectMember = false)
        {
            Status = status;
            RoleId = roleId;
            UplineRoleId = uplineRoleId;
            IsBold = isBold;
            Data = customerList;
            WithDataLimited = withDataLimited;
            WithNoDownlines = IsNoDownlines;
            HasSubGroups = hasSubGroups;
            IsMultipleCurrencyGroup = isMultipleCurrencyGroup;
            IsDirectMember = isDirectMember;
        }

        public List<CustomerListDTO> Data { get; }

        public bool IsBold { get; }

        public int RoleId { get; }
        
        public int UplineRoleId { get; }
        
        public bool IsDirectMember { get; }

        public string Status { get; }

        public bool WithDataLimited { get; }

        public bool WithNoDownlines { get; }

        public bool HasSubGroups { get; }

        public bool IsMultipleCurrencyGroup { get; }

        public MemoryStream Build()
        {
            var mem = new MemoryStream();
            var settings = new TemplateSettings();
            var filePath = IsDirectMember ? settings.DirectMemberListPath(UplineRoleId, IsMultipleCurrencyGroup) : settings.CustomerListPath(RoleId, HasSubGroups, IsMultipleCurrencyGroup);

            using (var report = new CustomerListPage(filePath, this))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}