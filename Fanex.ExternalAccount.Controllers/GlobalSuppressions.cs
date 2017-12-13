// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;
[assembly:
    SuppressMessage("Sonar Code Smell", "S3995:URI return values should not be strings", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Fanex.ExternalAccount.Controllers.UserControl.UserController.GetInternalLoginUrl(Fanex.ExternalAccount.Controllers.UserControl.UserController.AlphaSiteCode)~System.String")]

[assembly:
    SuppressMessage("Sonar Code Smell", "S3994:URI Parameters should not be strings", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Fanex.ExternalAccount.Controllers.UserControl.MenuController.Add(System.String,System.String,System.String,System.Int32,System.Int32,System.String,System.Int32)~System.Web.Mvc.ActionResult")]
[assembly:
    SuppressMessage("Sonar Code Smell", "S3994:URI Parameters should not be strings", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Fanex.ExternalAccount.Controllers.UserControl.MenuController.Edit(System.Int32,System.String,System.String,System.String,System.Int32,System.Int32,System.String,System.Int32)~System.Web.Mvc.ActionResult")]

[assembly:
    SuppressMessage("Sonar Vulnerability", "S1313:IP addresses should not be hardcoded", Justification = "<Pending>",
        Scope = "member",
        Target =
            "~M:Fanex.ExternalAccount.Controllers.Customer.CustomerController.ViewIP(System.String)~System.Web.Mvc.ActionResult")]