// --------------------------------------------------------------------------
// <copyright file="CustomerListPageBuilder.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// Creator: Simon
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.ExternalAccount.Excel.Customer.CustomerListPage
{
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using Fanex.ExternalAccount.DTO.Customer;
    using NPOI.HSSF.UserModel;

    /// <summary>
    /// Customer list builder.
    /// </summary>
    public class CustomerListPageBuilder
    {
        public CustomerListPageBuilder(DataTable customerList, bool isBold, string status, int roleId, bool isMyanmarOdds = false)
        {
            this.Status = status;
            this.RoleId = roleId;
            this.IsBold = isBold;
            this.Data = customerList;
            this.IsMyanmarOdds = isMyanmarOdds;
        }

        public DataTable Data { get; private set; }

        public bool IsBold { get; private set; }

        public int RoleId { get; private set; }

        public string Status { get; private set; }

        public bool IsMyanmarOdds { get; private set; }

        /// <summary>
        /// Builds customer list stream.
        /// </summary>
        /// <returns>MemoryStream object.</returns>
        public MemoryStream Build()
        {
            TemplateReport report = null;
            TemplateSettings settings = new TemplateSettings();
            string filePath = settings.CustomerListPath(this.RoleId, IsMyanmarOdds);

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