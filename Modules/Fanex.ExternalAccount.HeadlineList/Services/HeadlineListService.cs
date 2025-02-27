namespace Fanex.ExternalAccount.HeadlineList.Services
{
    using System.Linq;
    using Criteria;
    using DataAccesses;
    using Models;
    using ViewModels;

    public interface IHeadlineListService
    {
        SearchHeadlineResult GetHeadlines(SearchHeadlineQuery query, int userId, string groupName);
    }

    public class HeadlineListService : IHeadlineListService
    {
        private readonly IHeadlinesDataAccess _dataAccess;

        public HeadlineListService(IHeadlinesDataAccess dataAccess = null)
        {
            _dataAccess = dataAccess ?? new HeadlinesDataAccess(null);
        }

        public SearchHeadlineResult GetHeadlines(SearchHeadlineQuery query, int userId, string groupName)
        {
            var criteria = new GetHeadlinesCriteria(
                query.Username,
                query.GroupHeadline,
                query.Status, userId,
                groupName
            );
            var dbResult = _dataAccess.GetGroupHeadlines(criteria);
            var pagingResult = new SearchHeadlineResult
            {
                TotalGroups = dbResult.TotalHeadlineGroups,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalHeadlines = dbResult.TotalHeadlines,
                GroupHeadlines = dbResult.Groups
                    .Skip(query.PageSize * (query.Page - 1))
                    .Take(query.PageSize)
                
            };
            return pagingResult;
        }
    }
}