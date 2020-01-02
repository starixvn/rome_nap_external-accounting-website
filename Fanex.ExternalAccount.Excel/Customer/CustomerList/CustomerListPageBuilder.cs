namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using Fanex.ExternalAccount.DTO.Customer.CustomerList;
    using System.Collections.Generic;
    using System.IO;

    public class CustomerListPageBuilder
    {
        public CustomerListPageBuilder(List<CustomerListDTO> customerList, bool isBold, string status, int roleId, bool withDataLimited = false)
        {
            Status = status;
            RoleId = roleId;
            IsBold = isBold;
            Data = customerList;
            WithDataLimited = withDataLimited;
        }

        public List<CustomerListDTO> Data { get; }

        public bool IsBold { get; }

        public int RoleId { get; }

        public string Status { get; }

        public bool WithDataLimited { get; }

        public MemoryStream Build()
        {
            var mem = new MemoryStream();
            var settings = new TemplateSettings();
            var filePath = settings.CustomerListPath(RoleId);

            using (var report = new CustomerListPage(filePath, this))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}