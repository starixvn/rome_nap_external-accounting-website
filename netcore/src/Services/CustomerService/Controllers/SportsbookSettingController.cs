using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (Sportsbook Read)
    public class SportsbookSettingController : ControllerBase
    {
        private const int NG2Start = 213;
        private const int NG2End = 220;

        private readonly ISiteDBConfigService _siteDBConfigService;

        public SportsbookSettingController(ISiteDBConfigService siteDBConfigService)
        {
            _siteDBConfigService = siteDBConfigService;
        }

        [HttpGet]
        [Route("ViewPtInfo")]
        public IActionResult ViewPtInfo([FromQuery] int custId)
        {
            var exclusiveRefSportIds = new List<int>(8);

            for (var i = NG2Start; i <= NG2End; i++)
            {
                exclusiveRefSportIds.Add(i);
            }

            var result = new
            {
                ExclusiveRefSportIds = exclusiveRefSportIds.ToArray(),
                SiteDB = _siteDBConfigService.GetSiteDBConfigByCustId(custId)
            };

            return Ok(result);
        }
    }

    // TODO: Define proper interface
    public interface ISiteDBConfigService
    {
        object GetSiteDBConfigByCustId(int custId);
    }

    public class SiteDBConfigService : ISiteDBConfigService
    {
        public object GetSiteDBConfigByCustId(int custId)
        {
            // TODO: Implement actual service logic
            return new { CustId = custId, Config = "placeholder" };
        }
    }
}