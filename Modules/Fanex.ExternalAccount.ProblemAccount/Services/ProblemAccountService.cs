namespace Fanex.ExternalAccount.ProblemAccount.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.ProblemAccount.Criteria;
    using Fanex.ExternalAccount.ProblemAccount.Models;
    using Fanex.ExternalAccount.ProblemAccount.ViewModels;
    using Fanex.NAP.Framework.Data;

    public interface IProblemAccountService
    {
        IEnumerable<ProblemAccountEntity> GetProblemAccounts(GetListViewModel viewModel);
    }

    public class ProblemAccountService : IProblemAccountService
    {
        private readonly IDynamicRepository repository;

        public ProblemAccountService(IDynamicRepository repository = null)
        {
            this.repository = repository ?? new DynamicRepository();
        }

        public IEnumerable<ProblemAccountEntity> GetProblemAccounts(GetListViewModel viewModel)
        {
            yield return new ProblemAccountEntity
            {
                CustId = 1,
                Username = "master1",
                FirstName = "Noah",
                LastName = "James",
                Status = 1,
                CreatedDate = DateTime.Now.AddDays(-10),
                NumberOfDownline = 10
            };
            yield return new ProblemAccountEntity
            {
                CustId = 2,
                Username = "master2",
                FirstName = "Liam",
                LastName = "John",
                Status = 2,
                CreatedDate = DateTime.Now.AddDays(-9),
                NumberOfDownline = 9
            };
            yield return new ProblemAccountEntity
            {
                CustId = 3,
                Username = "master3",
                FirstName = "Mason",
                LastName = "Robert",
                Status = 3,
                CreatedDate = DateTime.Now.AddDays(-8),
                NumberOfDownline = 8,
                ClosedReason = 1
            };
            yield return new ProblemAccountEntity
            {
                CustId = 4,
                Username = "master4",
                FirstName = "Jacob",
                LastName = "Michael",
                Status = 4,
                CreatedDate = DateTime.Now.AddDays(-7),
                NumberOfDownline = 7
            };

            yield return new ProblemAccountEntity
            {
                CustId = 5,
                Username = "master5",
                Status = 3,
                CreatedDate = DateTime.Now.AddDays(-6),
                NumberOfDownline = 6,
                IsReviewed = true,
                ClosedReason = 2
            };
            yield return new ProblemAccountEntity
            {
                CustId = 6,
                Username = "master6",
                Status = 3,
                CreatedDate = DateTime.Now.AddDays(-5),
                NumberOfDownline = 5,
                IsReviewed = true,
                ClosedReason = 3
            };
            yield return new ProblemAccountEntity
            {
                CustId = 7,
                Username = "master7",
                Status = 3,
                CreatedDate = DateTime.Now.AddDays(-4),
                IsReviewed = true,
                ClosedReason = 4
            };
        }
            //=> repository.Fetch<ProblemAccountEntity>(new GetProblemAccountCriteria(viewModel));
    }
}
