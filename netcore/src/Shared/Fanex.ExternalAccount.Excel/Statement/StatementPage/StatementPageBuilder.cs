namespace Fanex.ExternalAccount.Excel.Statement.StatementPage
{
    using System.Collections.Generic;
    using System.IO;
    using DTO.Statement;

    public class StatementPageBuilder
    {
        public StatementPageBuilder(List<Statement> statement, bool isBold, string username,
            string fromDate, string toDate)
        {
            Username = username;
            FromDate = fromDate;
            ToDate = toDate;
            IsBold = isBold;
            Data = statement;
        }

        public List<Statement> Data { get; }

        public bool IsBold { get; }

        public string Username { get; }

        public string FromDate { get; }

        public string ToDate { get; }

        public MemoryStream Build()
        {
            var settings = new TemplateSettings();
            var filePath = settings.StatementPath();
            var mem = new MemoryStream();
            using (var report = new StatementPage(filePath, this))
            {
                var hssfWorkbook = report.Build();
                hssfWorkbook.Write(mem);
            }

            return mem;
        }
    }
}