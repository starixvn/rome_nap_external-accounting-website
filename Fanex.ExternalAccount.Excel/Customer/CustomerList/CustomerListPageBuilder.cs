namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using Fanex.ExternalAccount.DTO.Customer.CustomerList;
    using System.Collections.Generic;
    using System.IO;

    public class CustomerListPageBuilder
    {
        public CustomerListPageBuilder(List<CustomerListDTO> customerList, bool isBold, string status, int roleId, bool isMyanmarOdds = false, bool withDataLimited = false, bool hasSubGroups = false, bool isMultipleCurrencyGroup = false)
        {
            Status = status;
            RoleId = roleId;
            IsBold = isBold;
            Data = customerList;
            IsMyanmarOdds = isMyanmarOdds;
            WithDataLimited = withDataLimited;
            HasSubGroups = hasSubGroups;
            IsMultipleCurrencyGroup = isMultipleCurrencyGroup;
        }

        public List<CustomerListDTO> Data { get; }

        public bool IsBold { get; }

        public int RoleId { get; }

        public string Status { get; }

        public bool IsMyanmarOdds { get; }

        public bool WithDataLimited { get; }

        public bool HasSubGroups { get; }

        public bool IsMultipleCurrencyGroup { get; }

        public MemoryStream Build()
        {
            var mem = new MemoryStream();
            var settings = new TemplateSettings();
            var filePath = settings.CustomerListPath(RoleId, IsMyanmarOdds, HasSubGroups, IsMultipleCurrencyGroup);

            using (var report = new CustomerListPage(filePath, this))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}