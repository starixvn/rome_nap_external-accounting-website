// namespace Fanex.ExternalAccount.Service.Impl.Statement
// {
//     using System;
//     using System.Collections.Generic;
//     using DataAccess.Impl.Statement;
//     using DataAccess.Interface.Statement;
//     using DTO.Statement;
//     using Interface.Statement;
//     using Report;
//
//     public class StatementService : IStatementService
//     {
//         private readonly IStatementDao StatementRepository;
//
//         public StatementService(IStatementDao statementRepository = null)
//         {
//             StatementRepository = statementRepository ?? new StatementDao();
//         }
//
//         public List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate)
//         {
//             var includeAccBal = fromDate != ReportService.BalanceUpDate;
//
//             return StatementRepository.Acc_AllStatement_Full(custId, fromDate, toDate, ReportService.BalanceUpDate,
//                 ReportService.MoveBetsDate, includeAccBal);
//         }
//     }
// }