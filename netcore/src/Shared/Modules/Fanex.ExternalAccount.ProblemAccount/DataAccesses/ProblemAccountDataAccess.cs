namespace Fanex.ExternalAccount.ProblemAccount.DataAccesses
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.ProblemAccount.Criteria;
    using Fanex.ExternalAccount.ProblemAccount.Models;

    public interface IProblemAccountDataAccess
    {
        IEnumerable<ProblemAccountEntity> GetProblemAccounts(GetProblemAccountCriteria criteria);

        Result SetToReviewed(SetToReviewedCommand command);
    }

    public class ProblemAccountDataAccess : IProblemAccountDataAccess
    {
        private readonly IDynamicRepository _dynamicRepository;

        public ProblemAccountDataAccess(): this(null)
        {
        }

        public ProblemAccountDataAccess(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<ProblemAccountEntity> GetProblemAccounts(GetProblemAccountCriteria criteria)
            => _dynamicRepository.Fetch<ProblemAccountEntity>(criteria);

        public Result SetToReviewed(SetToReviewedCommand command)
        {
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }
    }
}
