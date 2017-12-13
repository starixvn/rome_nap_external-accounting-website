using System.Diagnostics.CodeAnalysis;

// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

[assembly: SuppressMessage(
    "Sonar Code Smell",
    "S1694:An abstract class should have both abstract and concrete methods",
    Justification = "<Pending>",
    Scope = "type",
    Target = "~T:Fanex.ExternalAccount.Excel.TemplateReport")]

[assembly: SuppressMessage(
    "Potential Code Quality Issues",
    "RECS0018:Comparison of floating point numbers with equality operator",
    Justification = "<Pending>",
    Scope = "member",
    Target = "~M:Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport.DailyWinLossReport.WriteTotal(NPOI.SS.UserModel.ISheet@,NPOI.SS.UserModel.ISheet@,System.Int32,System.Int32,System.Int32)")]
