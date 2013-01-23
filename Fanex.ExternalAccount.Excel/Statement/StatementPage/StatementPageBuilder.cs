namespace Fanex.ExternalAccount.Excel.Statement.StatementPage
{
    using System.Collections.Generic;
    using System.IO;
    using Fanex.ExternalAccount.DTO.Statement;
    using NPOI.HSSF.UserModel;

    public class StatementPageBuilder
    {
        public StatementPageBuilder(List<Statement> Statement, bool isBold, int roleId, string winlostDate)
        {
            this.WinlostDate = winlostDate;
            this.IsBold = isBold;
            this.RoleId = roleId;
            this.Data = Statement;
        }

        public List<Statement> Data { get; private set; }

        public bool IsBold { get; private set; }

        public int RoleId { get; private set; }

        public string WinlostDate { get; private set; }

        public MemoryStream Build()
        {
            TemplateReport report = null;
            TemplateSettings settings = new TemplateSettings();
            string filePath = settings.StatementPath();

            report = new StatementPage(filePath, this);

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