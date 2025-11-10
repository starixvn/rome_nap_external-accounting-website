using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (MarkAccountType)
    public class MarkAccountTypeController : ControllerBase
    {
        private readonly IMarkAccountTypeService _markAccountTypeService;
        private readonly ICurrentSession _currentSession;
        private readonly ICurrentContext _currentContext;
        private const int DefaultPageSize = 50;

        public MarkAccountTypeController(IMarkAccountTypeService markAccountTypeService, ICurrentSession currentSession, ICurrentContext currentContext)
        {
            _markAccountTypeService = markAccountTypeService;
            _currentSession = currentSession;
            _currentContext = currentContext;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            // TODO: Implement proper view rendering
            return Ok(new { message = "Mark Account Type Index" });
        }

        [HttpPost]
        [Route("Search")]
        public IActionResult Search([FromBody] SearchMarkAccountRequest request)
        {
            var result = _markAccountTypeService.Search(new SearchMarkAccountTypeParams
            {
                GroupId = request.GroupId,
                GroupName = request.GroupName,
                Level = request.Level,
                Type = request.Type,
                UserId = _currentSession.UserId,
                UserName = request.UserName,
                PageIndex = request.PageIndex ?? 1,
                PageSize = request.PageSize ?? DefaultPageSize
            });

            return Ok(result);
        }

        [HttpGet]
        [Route("MarkAccount")]
        public IActionResult MarkAccount()
        {
            // TODO: Implement proper view rendering
            return Ok(new { message = "Mark Account View" });
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add([FromBody] NewMarkAccountRequest request)
        {
            var result = _markAccountTypeService.Add(new NewMarkAccountTypeParams
            {
                GroupName = request.GroupName,
                TypeId = request.TypeId,
                Usernames = request.Usernames,
                Remark = request.Remark,
                UserId = _currentSession.UserId,
                IP = _currentContext.Ip4Address
            });

            return Ok(result);
        }

        [HttpPost]
        [Route("Update")]
        public IActionResult Update([FromBody] UpdateMarkAccountRequest request)
        {
            var result = _markAccountTypeService.SingleUpdate(new UpdateMarkAccountTypeParams
            {
                CustId = request.CustId,
                TypeId = request.TypeId,
                Remark = request.Remark,
                UserId = _currentSession.UserId,
                IP = _currentContext.Ip4Address
            });

            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateMultiple")]
        public IActionResult UpdateMultiple([FromBody] UpdateMultipelMarkAccountRequest request)
        {
            var updateAccountTypes = request.UpdateAccounts.Select(account => new UpdateMarkAccountTypeParams
            {
                CustId = account.CustId,
                Username = account.Username,
                TypeId = account.TypeId,
                Remark = account.Remark,
                UserId = _currentSession.UserId,
                IP = _currentContext.Ip4Address
            });

            var result = _markAccountTypeService.MultipleUpdate(updateAccountTypes);
            return Ok(result);
        }

        [HttpPost]
        [Route("Unmark")]
        public IActionResult Unmark([FromBody] UnmarkAccountRequest request)
        {
            var result = _markAccountTypeService.Unmark(new UnmarkAccountTypeParams
            {
                CustId = request.CustId,
                UserId = _currentSession.UserId,
                IP = _currentContext.Ip4Address
            });

            return Ok(result);
        }

        [HttpPost]
        [Route("UnmarkAll")]
        public IActionResult UnmarkAll([FromBody] UnmarkAccountRequest request)
        {
            var result = _markAccountTypeService.UnmarkAll(new UnmarkAccountTypeParams
            {
                Accounts = request.Accounts,
                UserId = _currentSession.UserId,
                IP = _currentContext.Ip4Address
            });

            return Ok(result);
        }

        [HttpPost]
        [Route("MultipleEditAccountType")]
        public IActionResult MultipleEditAccountType([FromBody] UnmarkAccountRequest request)
        {
            // TODO: Implement proper view rendering
            return Ok(new { message = "Multiple Edit Account Type View" });
        }
    }

    // TODO: Define proper interfaces and models
    public interface IMarkAccountTypeService
    {
        object Search(SearchMarkAccountTypeParams parameters);
        object Add(NewMarkAccountTypeParams parameters);
        object SingleUpdate(UpdateMarkAccountTypeParams parameters);
        object MultipleUpdate(IEnumerable<UpdateMarkAccountTypeParams> parameters);
        object Unmark(UnmarkAccountTypeParams parameters);
        object UnmarkAll(UnmarkAccountTypeParams parameters);
    }

    public class MarkAccountTypeService : IMarkAccountTypeService
    {
        public object Search(SearchMarkAccountTypeParams parameters)
        {
            // TODO: Implement actual service logic
            return new { results = new[] { new { id = 1, name = "Account 1" } } };
        }

        public object Add(NewMarkAccountTypeParams parameters)
        {
            // TODO: Implement actual service logic
            return new { success = true, id = 1 };
        }

        public object SingleUpdate(UpdateMarkAccountTypeParams parameters)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object MultipleUpdate(IEnumerable<UpdateMarkAccountTypeParams> parameters)
        {
            // TODO: Implement actual service logic
            return new { success = true, count = parameters.Count() };
        }

        public object Unmark(UnmarkAccountTypeParams parameters)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }

        public object UnmarkAll(UnmarkAccountTypeParams parameters)
        {
            // TODO: Implement actual service logic
            return new { success = true };
        }
    }

    // TODO: Define proper request/response models
    public class SearchMarkAccountRequest
    {
        public int? GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int? Level { get; set; }
        public int? Type { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }

    public class NewMarkAccountRequest
    {
        public string GroupName { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public string[] Usernames { get; set; } = Array.Empty<string>();
        public string Remark { get; set; } = string.Empty;
    }

    public class UpdateMarkAccountRequest
    {
        public int CustId { get; set; }
        public int TypeId { get; set; }
        public string Remark { get; set; } = string.Empty;
    }

    public class UpdateMultipelMarkAccountRequest
    {
        public IEnumerable<UpdateMarkAccountItem> UpdateAccounts { get; set; } = Array.Empty<UpdateMarkAccountItem>();
    }

    public class UpdateMarkAccountItem
    {
        public int CustId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public string Remark { get; set; } = string.Empty;
    }

    public class UnmarkAccountRequest
    {
        public int CustId { get; set; }
        public IEnumerable<int> Accounts { get; set; } = Array.Empty<int>();
    }

    // TODO: Define proper parameter models
    public class SearchMarkAccountTypeParams
    {
        public int? GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int? Level { get; set; }
        public int? Type { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class NewMarkAccountTypeParams
    {
        public string GroupName { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public string[] Usernames { get; set; } = Array.Empty<string>();
        public string Remark { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string IP { get; set; } = string.Empty;
    }

    public class UpdateMarkAccountTypeParams
    {
        public int CustId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public string Remark { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string IP { get; set; } = string.Empty;
    }

    public class UnmarkAccountTypeParams
    {
        public int CustId { get; set; }
        public IEnumerable<int> Accounts { get; set; } = Array.Empty<int>();
        public int UserId { get; set; }
        public string IP { get; set; } = string.Empty;
    }
}
