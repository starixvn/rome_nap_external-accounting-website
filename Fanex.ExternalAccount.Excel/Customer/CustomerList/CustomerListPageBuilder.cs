namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System.Collections.Generic;
    using System.IO;
    using Fanex.ExternalAccount.DTO.Customer;
    using NPOI.HSSF.UserModel;
    using System.Data;

    public class CustomerListPageBuilder
    {
        public CustomerListPageBuilder(DataTable CustomerList, bool isBold, string status, int roleId)
        {
            this.Status = status;
            this.RoleId = roleId;
            this.IsBold = isBold;
            this.Data = CustomerList;
        }

        public DataTable Data { get; private set; }

        public bool IsBold { get; private set; }

        public int RoleId { get; private set; }

        public string Status { get; private set; }

        public MemoryStream Build()
        {
            TemplateReport report = null;
            TemplateSettings settings = new TemplateSettings();
            string filePath = settings.CustomerListPath(this.RoleId);

            report = new CustomerListPage(filePath, this);

            if (report != null)
            {
                if (report is IReportBuilder)
                {
                    MemoryStream mem = new MemoryStream();
                    HSSFWorkbook hssfWorkbook = ((IReportBuilder)report).Build();
                    if (hssfWorkbook != null)
                    {
                        hssfWorkbook.Write(mem);
                        return mem;
                    }

                    report.Dispose();
                }
            }

            return null;
        }
    }
}