using System.Collections.Generic;
using System.Linq;
using Fanex.ExternalAccount.HeadlineList.Criteria;
using Fanex.ExternalAccount.HeadlineList.DataAccesses;
using Fanex.ExternalAccount.HeadlineList.Models;

namespace Fanex.ExternalAccount.HeadlineList.Services
{
    public interface IHeadlineListService
    {
        SearchHeadlineResult GetHeadlines(int userId, int pageIndex, int pageSize);
    }

    public class HeadlineListService : IHeadlineListService
    {
        private readonly IHeadlinesDataAccess dataAccess;

        public HeadlineListService(IHeadlinesDataAccess dataAccess = null)
        {
            this.dataAccess = dataAccess ?? new HeadlinesDataAccess();
        }

        public SearchHeadlineResult GetHeadlines(int userId, int pageIndex, int pageSize)
        {
            int totalGroups = 0,
                totalMemberNotAssignedToGroup = 0;
            IList<GroupHeadline> groupHeadlines = new List<GroupHeadline>(512);
            var data = this.dataAccess.GetGroupHeadlines(new GetHeadlinesCriteria { UserID = userId });
            var pagingResult = new SearchHeadlineResult()
            {
                TotalGroups = data.Count(),
                PageIndex = pageIndex,
                GroupHeadlines = data.ToList()
                    .Skip(pageSize * (pageIndex - 1))
                    .Take(pageSize)
                    .ToList()
            };
            return pagingResult;
        }
    }
}