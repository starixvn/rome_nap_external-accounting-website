namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System.Data;
    using System.IO;

    public class CustomerListPageBuilder
    {
        public CustomerListPageBuilder(DataTable customerList, bool isBold, string status, int roleId,
            bool isMyanmarOdds = false, bool withDataLimited = false)
        {
            Status = status;
            RoleId = roleId;
            IsBold = isBold;
            Data = customerList;
            IsMyanmarOdds = isMyanmarOdds;
            WithDataLimited = withDataLimited;
        }

        public DataTable Data { get; }

        public bool IsBold { get; }

        public int RoleId { get; }

        public string Status { get; }

        public bool IsMyanmarOdds { get; }

        public bool WithDataLimited { get; }

        public MemoryStream Build()
        {
            var mem = new MemoryStream();
            var settings = new TemplateSettings();
            var filePath = settings.CustomerListPath(RoleId, IsMyanmarOdds);

            using (var report = new CustomerListPage(filePath, this))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}