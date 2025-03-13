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
            // StatementReportModel result = new StatementReportModel
            // {
            //     StatementList = new List<StatementModel>(),
            //     PageIndex = criteria.PageIndex
            // };
            //
            // using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     objDb.AssignParamValues(criteria.GetSpParams());
            //     var reader = objDb.ExecuteReader();
            //     while (reader.Read())
            //     {
            //         var statementModel = new StatementModel
            //         {
            //             StatementDate = Converter.ToDateTime(reader["winlostdate"]),
            //             OpeningBalance = new DataColumnModel
            //             {
            //                 HasData = !Convert.IsDBNull(reader["OpeningBalance"]),
            //                 Value = Converter.To<double>(reader["OpeningBalance"])
            //             },
            //             Turnover = new DataColumnModel
            //             {
            //                 HasData = !Convert.IsDBNull(reader["turnover"]),
            //                 Value = Converter.To<double>(reader["turnover"])
            //             },
            //             WinLoss = new DataColumnModel
            //             {
            //                 HasData = !Convert.IsDBNull(reader["WinLost"]),
            //                 Value = Converter.To<double>(reader["WinLost"])
            //             },
            //             Commission = new DataColumnModel
            //             {
            //                 HasData = !Convert.IsDBNull(reader["Comm"]),
            //                 Value = Converter.To<double>(reader["Comm"])
            //             },
            //             TransferAmount = new DataColumnModel
            //             {
            //                 HasData = !Convert.IsDBNull(reader["TransferAmt"]),
            //                 Value = Converter.To<double>(reader["TransferAmt"])
            //             },
            //             AccumulatedTotal = new DataColumnModel
            //             {
            //                 HasData = !Convert.IsDBNull(reader["ClosingBalance"]),
            //                 Value = Converter.To<double>(reader["ClosingBalance"])
            //             }
            //         };
            //
            //         statementModel.StatementDateString = statementModel.StatementDate.ToString("MM/dd/yyyy");
            //         result.StatementList.Add(statementModel);
            //     }
            // }

            //return result;
            var dbResult = repository.Fetch<StatementModelDTO>(criteria);
            return new StatementReportModel
            {
                StatementList = dbResult.Select(x => new StatementModel(x)).ToList(),
                PageIndex = criteria.PageIndex
            };
        }

        public StatementReportModel MemberStatement(MemberStatementCriteria criteria)
        {
            // StatementReportModel result = new StatementReportModel
            // {
            //     StatementList = new List<StatementModel>(),
            //     PageIndex = criteria.PageIndex
            // };
            //
            // if (criteria.IsValid())
            // {
            //     using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            //     {
            //         objDb.AssignParamValues(criteria.GetSpParams());
            //         var reader = objDb.ExecuteReader();
            //         while (reader.Read())
            //         {
            //             var statementModel = new StatementModel
            //             {
            //                 StatementDate = Converter.ToDateTime(reader["winlostdate"]),
            //                 OpeningBalance = new DataColumnModel
            //                 {
            //                     HasData = !Convert.IsDBNull(reader["OpeningBalance"]),
            //                     Value = Converter.To<double>(reader["OpeningBalance"])
            //                 },
            //                 Turnover = new DataColumnModel
            //                 {
            //                     HasData = !Convert.IsDBNull(reader["turnover"]),
            //                     Value = Converter.To<double>(reader["turnover"])
            //                 },
            //                 VoidCancelled = new DataColumnModel
            //                 {
            //                     HasData = !Convert.IsDBNull(reader["VoidAmt"]),
            //                     Value = Converter.To<double>(reader["VoidAmt"])
            //                 },
            //                 Outstanding = new DataColumnModel
            //                 {
            //                     HasData = !Convert.IsDBNull(reader["Outstanding"]),
            //                     Value = Converter.To<double>(reader["Outstanding"])
            //                 },
            //                 WinLoss = new DataColumnModel
            //                 {
            //                     HasData = !Convert.IsDBNull(reader["WinLost"]),
            //                     Value = Converter.To<double>(reader["WinLost"])
            //                 },
            //                 Commission = new DataColumnModel
            //                 {
            //                     HasData = !Convert.IsDBNull(reader["Comm"]),
            //                     Value = Converter.To<double>(reader["Comm"])
            //                 },
            //                 GrandPrize = new DataColumnModel
            //                 {
            //                     HasData = !Convert.IsDBNull(reader["GrandPrize"]),
            //                     Value = Converter.To<double>(reader["GrandPrize"])
            //                 },
            //                 TransferAmount = new DataColumnModel
            //                 {
            //                     HasData = !Convert.IsDBNull(reader["TransferAmt"]),
            //                     Value = Converter.To<double>(reader["TransferAmt"])
            //                 },
            //                 AccumulatedTotal = new DataColumnModel
            //                 {
            //                     HasData = !Convert.IsDBNull(reader["ClosingBalance"]),
            //                     Value = Converter.To<double>(reader["ClosingBalance"])
            //                 }
            //             };
            //
            //             statementModel.StatementDateString = statementModel.StatementDate.ToString("MM/dd/yyyy");
            //             result.StatementList.Add(statementModel);
            //         }
            //     }
            // }

            // return result;
            var dbResult = repository.Fetch<StatementModelDTO>(criteria);
            return new StatementReportModel
            {
                StatementList = dbResult.Select(x => new StatementModel(x){StatementDateString = x.StatementDate.ToString("MM/dd/yyyy")}).ToList(),
                PageIndex = criteria.PageIndex
            };
        }

        public StatementReportModel TransferDetails(TransferDetailsCriteria criteria)
        {
           //  StatementReportModel result = new StatementReportModel
           // {
           //     StatementList = new List<StatementModel>(),
           //     Total = new StatementModel
           //     {
           //         TransferAmount = new DataColumnModel
           //         {
           //             HasData = true
           //         }
           //     }
           // };
           //
           //  double totalTransferAmount = 0;
           //  if (criteria.IsValid())
           //  {
           //      using (var objDb = new ObjectDb(criteria.GetSettingKey()))
           //      {
           //          objDb.AssignParamValues(criteria.GetSpParams());
           //          var reader = objDb.ExecuteReader();
           //          while (reader.Read())
           //          {
           //              var transferAmount = Converter.To<double>(reader["amount"]);
           //              var statementModel = new StatementModel
           //              {
           //                  StatementDate = Converter.ToDateTime(reader["transdate"]),
           //                  TransactionType = Converter.ToString2(reader["transtype"]),
           //                  Description = Converter.ToString2(reader["transdesc"]),
           //                  TransferAmount = new DataColumnModel
           //                  {
           //                      HasData = true,
           //                      Value = transferAmount
           //                  },
           //                  Creator = Converter.ToString2(reader["creator"])
           //              };
           //
           //              if (criteria.BalanceUpDate <= Converter.ToDateTime(criteria.TransferDate))
           //              {
           //                  statementModel.ExchangeRate = Converter.ToString2(reader["ExRate"]);
           //              }
           //
           //              statementModel.StatementDateString = statementModel.StatementDate.ToString("dddd <br> MM/dd/yyyy <br> hh:mm:ss tt");
           //              result.StatementList.Add(statementModel);
           //              totalTransferAmount += transferAmount;
           //          }
           //      }
           //  }
           //
           //  result.Total.TransferAmount.Value = Math.Round(totalTransferAmount, 2);
            //return result;
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
            // CustomerInformationModel result = new CustomerInformationModel();
            // using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     objDb.AssignParamValues(criteria.GetSpParams());
            //     var reader = objDb.ExecuteReader();
            //     while (reader.Read())
            //     {
            //         result.CustomerId = Converter.ToInt(reader["custid"]);
            //         result.RoleId = Converter.ToInt(reader["roleid"]);
            //     }
            // }

            //return result;
            
            var dbResult = repository.Fetch<CustomerInformationModel>(criteria).FirstOrDefault();
            return dbResult;
        }
    }
}
