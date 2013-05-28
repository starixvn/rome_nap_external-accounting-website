
using Fanex.Puffin.Admin.Criteria;

namespace Fanex.Puffin.Admin.Repositories.Interface
{
    public interface IAdminRepository
    {
        bool CheckUserOnlineStatus(AccountingUserCheckOnlineStatusCriteria criteria);
    }
}
