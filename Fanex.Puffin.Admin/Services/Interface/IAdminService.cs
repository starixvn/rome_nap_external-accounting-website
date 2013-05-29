
using Fanex.Puffin.Admin.Criteria;

namespace Fanex.Puffin.Admin.Services.Interface
{
    public interface IAdminService
    {
        bool CheckUserOnlineStatus(AccountingUserCheckOnlineStatusCriteria criteria);
    }
}
