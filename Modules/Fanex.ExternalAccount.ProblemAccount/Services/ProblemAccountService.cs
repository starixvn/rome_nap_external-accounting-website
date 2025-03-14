namespace Fanex.ExternalAccount.ProblemAccount.Services
{
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.ProblemAccount.Criteria;
    using Fanex.ExternalAccount.ProblemAccount.DataAccesses;
    using Fanex.ExternalAccount.ProblemAccount.Models;
    using Fanex.ExternalAccount.ProblemAccount.ViewModels;

    public interface IProblemAccountService
    {
        ProblemAccountReport GetProblemAccounts(GetListViewModel viewModel);

        Result SetToReviewed(SetToReviewedViewModel viewModel);
    }

    public class ProblemAccountService : IProblemAccountService
    {
        private readonly IProblemAccountDataAccess dataAccess;

        public ProblemAccountService(IProblemAccountDataAccess dataAccess = null)
        {
            this.dataAccess = dataAccess ?? new ProblemAccountDataAccess();
        }

        public ProblemAccountReport GetProblemAccounts(GetListViewModel viewModel)
        {
            // PageIndex of Fanex.Grid begin at 0 whereas server site begin at 1.
            viewModel.PageIndex++;
            var criteria = new GetProblemAccountCriteria(viewModel);
            var problemAccounts = dataAccess.GetProblemAccounts(criteria);

            return new ProblemAccountReport(criteria.TotalRows, problemAccounts);
        }

        public Result SetToReviewed(SetToReviewedViewModel viewModel)
            => dataAccess.SetToReviewed(new SetToReviewedCommand(viewModel));
    }
}
