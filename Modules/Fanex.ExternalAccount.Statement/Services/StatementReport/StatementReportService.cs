namespace Fanex.ExternalAccount.Statement.Services.StatementReport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Fanex.ExternalAccount.Statement.Criteria;
    using Fanex.ExternalAccount.Statement.Criteria.StatementReport;
    using Fanex.ExternalAccount.Statement.Models;
    using Fanex.ExternalAccount.Statement.Models.StatementReport;
    using Fanex.ExternalAccount.Statement.Repositories.StatementReport;
    using Fanex.Logging;

    public class StatementReportService : IStatementReportService
    {
        private readonly IStatementReportRepository _statementReportRepository;

        public StatementReportService() :
            this(null)
        {
        }

        public StatementReportService(IStatementReportRepository statementReportRepository)
        {
            _statementReportRepository = statementReportRepository ?? new StatementReportRepository();
        }

        public StatementReportModel SMAStatement(SMAStatementCriteria criteria)
        {
            StatementReportModel result = new StatementReportModel
            {
                StatementList = new List<StatementModel>()
            };
         
            try
            {
                List<StatementReportCriteria> dateRangeList = GetReportDateRange(criteria);
                for (var i = 0; i < dateRangeList.Count; i++)
                {
                    var dateRange = dateRangeList[i];
                    var data = _statementReportRepository.SMAStatement(new SMAStatementCriteria
                    {
                        FromDate = dateRange.FromDate,
                        ToDate = dateRange.ToDate,
                        RoleId = criteria.RoleId,
                        PageSize = criteria.PageSize,
                        PageIndex = criteria.PageIndex,
                        CustId = criteria.CustId
                    });

                    result.StatementList = Enumerable.Concat(result.StatementList, data.StatementList).ToList();
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
            }

            return result;
        }

        public StatementReportModel MemberStatement(MemberStatementCriteria criteria)
        {
            StatementReportModel result = new StatementReportModel
            {
                StatementList = new List<StatementModel>()
            };

            if (criteria.IsValid())
            {
                try
                {
                    List<StatementReportCriteria> dateRangeList = GetReportDateRange(criteria);
                    for (var i = 0; i < dateRangeList.Count; i++)
                    {
                        var dateRange = dateRangeList[i];
                        var data = _statementReportRepository.MemberStatement(new MemberStatementCriteria
                        {
                            FromDate = dateRange.FromDate,
                            ToDate = dateRange.ToDate,
                            RoleId = criteria.RoleId,
                            PageSize = criteria.PageSize,
                            PageIndex = criteria.PageIndex,
                            CustId = criteria.CustId
                        });

                        result.StatementList = Enumerable.Concat(result.StatementList, data.StatementList).ToList();
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message, ex);
                }
            }

            return result;
        }

        public StatementReportModel TransferDetails(TransferDetailsCriteria criteria)
        {
            StatementReportModel result = new StatementReportModel
            {
                StatementList = new List<StatementModel>(),
                Total = new StatementModel()
            };

            try
            {
                result = _statementReportRepository.TransferDetails(criteria);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
            }

            return result;
        }

        public MemoryStream ExportMemberStatement(MemberStatementCriteria criteria, StatementReportModel data)
        {
            if (data == null)
            {
                data = MemberStatement(criteria);
            }

            StatementReportExportExcelService _exportExcelService = new StatementReportExportExcelService();
            return _exportExcelService.ExportMemberStatement(criteria, data);
        }

        public MemoryStream ExportSMAStatement(SMAStatementCriteria criteria, StatementReportModel data)
        {
            if (data == null)
            {
                data = SMAStatement(criteria);
            }

            StatementReportExportExcelService _exportExcelService = new StatementReportExportExcelService();
            return _exportExcelService.ExportSMAStatement(criteria, data);
        }

        public MemoryStream ExportTransferDetails(TransferDetailsCriteria criteria, StatementReportModel data)
        {
            if (data == null)
            {
                data = TransferDetails(criteria);
            }

            StatementReportExportExcelService _exportExcelService = new StatementReportExportExcelService();
            return _exportExcelService.ExportTransferDetails(criteria, data);
        }

        public CustomerInformationModel GetCustomerInfo(GetCustomerInformationByUsername criteria)
        {
            CustomerInformationModel result = new CustomerInformationModel();
            if (!criteria.IsValid())
            {
                return result;
            }

            try
            {
                result = _statementReportRepository.GetCustomerInfo(criteria);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
            }

            return result;
        }
        
        private List<StatementReportCriteria> GetReportDateRange(StatementReportCriteria criteria)
        {
            criteria.FromDate = criteria.FromDate.Year <= 1753 ||
                criteria.FromDate.Year >= 9999 
                ? DateTime.Now : criteria.FromDate;
            criteria.ToDate = criteria.ToDate.Year <= 1753 ||
                criteria.ToDate.Year >= 9999
                ? DateTime.Now : criteria.ToDate;

            if (criteria.FromDate > criteria.MoveBetsDay ||
                (criteria.BalanceUpDay <= criteria.FromDate && criteria.ToDate <= criteria.MoveBetsDay) ||
                 criteria.ToDate < criteria.BalanceUpDay)
            {
                return new List<StatementReportCriteria>() 
                { 
                    new StatementReportCriteria
                    {
                        FromDate = criteria.FromDate,
                        ToDate = criteria.ToDate 
                    }
                };
            }

            if (criteria.FromDate >= criteria.BalanceUpDay && criteria.ToDate > criteria.MoveBetsDay)
            {
                return new List<StatementReportCriteria>() 
                { 
                    new StatementReportCriteria
                    {
                        FromDate = criteria.FromDate,
                        ToDate = criteria.MoveBetsDay
                    },
                    new StatementReportCriteria
                    {
                        FromDate = criteria.MoveBetsDay.AddDays(1),
                        ToDate = criteria.ToDate
                    }
                };
            }

            if (criteria.ToDate > criteria.MoveBetsDay)
            {
                return new List<StatementReportCriteria>() 
                { 
                    new StatementReportCriteria
                    {
                        FromDate = criteria.FromDate,
                        ToDate = criteria.BalanceUpDay.AddDays(-1)
                    },
                    new StatementReportCriteria
                    {
                        FromDate = criteria.BalanceUpDay,
                        ToDate = criteria.MoveBetsDay
                    },
                    new StatementReportCriteria
                    {
                        FromDate = criteria.MoveBetsDay.AddDays(1),
                        ToDate = criteria.ToDate
                    }
                };
            }

            return new List<StatementReportCriteria>() 
            { 
                    new StatementReportCriteria
                    {
                        FromDate = criteria.FromDate,
                        ToDate = criteria.BalanceUpDay.AddDays(-1)
                    },
                    new StatementReportCriteria
                    {
                        FromDate = criteria.BalanceUpDay,
                        ToDate = criteria.ToDate
                    }
                };
        }
    }
}
