namespace Fanex.ExternalAccount.ViewModel.Customer.OpenCustomer
{
    using DTO.Customer.Status;
    using Requests.Customer.OpenCustomer;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class SingleOpenCustomerViewModel
    {
        private readonly GetSingleOpenCustomerInfoQuery query;
        private readonly IEnumerable<SingleOpenAffectedDownline> affectedDowlines;

        public SingleOpenCustomerViewModel(GetSingleOpenCustomerInfoQuery query, IEnumerable<SingleOpenAffectedDownline> affectedDowlines)
        {
            this.affectedDowlines = affectedDowlines;
            this.query = query;
        }

        public int CustId => query.CustId;

        public string UserName => query.UserName;

        public ReadOnlyCollection<SingleOpenAffectedDownline> AffectedDownlines()
        {
            if (affectedDowlines?.ToList().Count > 1)
            {
                return affectedDowlines?.Concat(new[] { TotalRow })?.ToList()?.AsReadOnly();
            }

            return affectedDowlines?.ToList().AsReadOnly();
        }

        private SingleOpenAffectedDownline TotalRow
        {
            get
            {
                if (affectedDowlines?.ToList().Count > 1)
                {
                    int totalAffectedDownlines = 0;
                    int totalRemainUnchangedDownlines = 0;

                    foreach (var row in affectedDowlines)
                    {
                        totalAffectedDownlines += row.AffectedDownlines;
                        totalRemainUnchangedDownlines += row.RemainUnchangedDownlines;
                    }

                    return new SingleOpenAffectedDownline(0, totalAffectedDownlines, totalRemainUnchangedDownlines);
                }

                return null;
            }
        }
    }
}