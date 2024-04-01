namespace Fanex.ExternalAccount.HeadlineList.Services
{
    using System.Linq;
    using Criteria;
    using DataAccesses;
    using Models;
    using ViewModels;

    public interface IHeadlineListService
    {
        SearchHeadlineResult GetHeadlines(SearchHeadlineViewModel query, int userId);
    }

    public class HeadlineListService : IHeadlineListService
    {
        private readonly IHeadlinesDataAccess _dataAccess;

        public HeadlineListService(IHeadlinesDataAccess dataAccess = null)
        {
            _dataAccess = dataAccess ?? new HeadlinesDataAccess();
        }

        public SearchHeadlineResult GetHeadlines(SearchHeadlineViewModel query, int userId)
        {
            var dbResult = _dataAccess.GetGroupHeadlines(new GetHeadlinesCriteria(query.Username, query.GroupHeadline, query.Status, userId));
            var pagingResult = new SearchHeadlineResult
            {
                TotalGroups = dbResult.TotalHeadlineGroups,
                PageIndex = query.PageIndex,
                TotalHeadlines = dbResult.TotalHeadlines,
                GroupHeadlines = dbResult.GroupHeadlines
                    .Skip(query.PageSize * (query.PageIndex - 1))
                    .Take(query.PageSize)
                    .ToList()
            };
            return pagingResult;
        }
    }
}