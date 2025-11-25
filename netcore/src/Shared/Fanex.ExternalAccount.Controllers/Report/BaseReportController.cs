namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Linq;
    using Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Service.Interface.Report;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    public class BaseReportController : BaseController
    {
        protected readonly IReportService _reportService;

        protected BaseReportController(
            IReportService reportService,
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            IAuthorizationService authorizationService)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            _reportService = reportService;
        }

        protected bool CanView(string[] groups)
            => groups.Contains(CurrentSession.ParentGroupName, StringComparer.CurrentCultureIgnoreCase);

    }
}