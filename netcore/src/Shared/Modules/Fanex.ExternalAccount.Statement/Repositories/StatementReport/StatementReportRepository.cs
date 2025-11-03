namespace Fanex.ExternalAccount.Statement.Repositories.StatementReport
{
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.ExternalAccount.Statement.Criteria;
    using Fanex.ExternalAccount.Statement.Criteria.StatementReport;
    using Fanex.ExternalAccount.Statement.Models;
    using Fanex.ExternalAccount.Statement.Models.StatementReport;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Data.Repository;
    using System.Linq;

    public class StatementReportRepository : IStatementReportRepository
    {
        private readonly IDynamicRepository repository;
        public StatementReportRepository (IDynamicRepository repository = null) => this.repository = repository ?? new DynamicRepository();
        public StatementReportModel SMAStatement(SMAStatementCriteria criteria)
        {
            var dbResult = repository.Fetch<StatementModelDTO>(criteria);
            return new StatementReportModel
            {
                StatementList = dbResult.Select(x => new StatementModel(x){StatementDateString = x.StatementDate.ToString("MM/dd/yyyy")}).ToList(),
                PageIndex = criteria.PageIndex
            };
        }

        public StatementReportModel MemberStatement(MemberStatementCriteria criteria)
        {
            var dbResult = repository.Fetch<StatementModelDTO>(criteria);
            return new StatementReportModel
            {
                StatementList = dbResult.Select(x => new StatementModel(x){StatementDateString = x.StatementDate.ToString("MM/dd/yyyy")}).ToList(),
                PageIndex = criteria.PageIndex
            };
        }

        public StatementReportModel TransferDetails(TransferDetailsCriteria criteria)
        {
            var dbResult = repository.Fetch<StatementModelDTO>(criteria).ToList();
            var sumTransferAmount = dbResult.Sum(x => x.TransferAmount) ?? 0;
            return new StatementReportModel
            {
                StatementList = dbResult.Select(x => new StatementModel(x)
                {
                    StatementDateString = x.StatementDate.ToString("dddd <br> MM/dd/yyyy <br> hh:mm:ss tt")
                }).ToList(),
                Total = new StatementModel
                {
                    TransferAmount = new DataColumnModel
                    {
                        HasData = true,
                        Value = Math.Round(sumTransferAmount, 2)
                    }
                }
            };
        }

        public CustomerInformationModel GetCustomerInfo(GetCustomerInformationByUsername criteria)
        {
            var dbResult = repository.Fetch<CustomerInformationModel>(criteria).FirstOrDefault();
            return dbResult;
        }
    }
}
