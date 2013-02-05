// --------------------------------------------------------------------------
// <copyright file="WinLossDetailReportMAgBuilder.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// Creator: Simon
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport
{
    using System.Collections.Generic;
    using System.IO;
    using Fanex.ExternalAccount.DTO.Report;
    using NPOI.HSSF.UserModel;

    public class WinLossDetailReportMAgBuilder
    {
        public WinLossDetailReportMAgBuilder(List<WinLossDetailMAg> winLossDetailMAg, bool isBold, int roleId, string fromDate, string toDate)
        {
            this.FromDate = fromDate;
            this.ToDate = toDate;
            this.IsBold = isBold;
            this.RoleId = roleId;
            this.Data = winLossDetailMAg;
        }

        public List<WinLossDetailMAg> Data { get; private set; }

        public bool IsBold { get; private set; }

        public int RoleId { get; private set; }

        public string FromDate { get; private set; }

        public string ToDate { get; private set; }

        public MemoryStream Build()
        {
            TemplateReport report = null;
            TemplateSettings settings = new TemplateSettings();
            string filePath = settings.WinLossDetailPath(this.RoleId);

            report = new WinLossDetailReportMAg(filePath, this);

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