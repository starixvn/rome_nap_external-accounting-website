namespace Fanex.ExternalAccount.ProblemAccount.Models
{
    using System.Collections.Generic;

    public class ProblemAccountReport
    {
        public ProblemAccountReport(
            int totalItems,
            IEnumerable<ProblemAccountEntity> problemAccounts)
        {
            TotalItems = totalItems;
            ProblemAccounts = problemAccounts;
        }

        public int TotalItems { get; }

        public IEnumerable<ProblemAccountEntity> ProblemAccounts { get; }
    }
}
