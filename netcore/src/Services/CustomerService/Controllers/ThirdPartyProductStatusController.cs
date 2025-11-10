using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.ExternalAccount.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThirdPartyProductStatusController : ControllerBase
    {
        [HttpGet]
        [Route("Index")]
        [Authorize] // TODO: Update with proper authorization policy
        public IActionResult Index([FromQuery] ThirdPartyProductStatusViewModel viewModel)
        {
            // TODO: Implement proper view rendering or API response
            return Ok(viewModel);
        }
    }

    // TODO: Define proper view model
    public class ThirdPartyProductStatusViewModel
    {
        // TODO: Add properties as needed
        public string Placeholder { get; set; } = "ThirdPartyProductStatus";
    }
}
