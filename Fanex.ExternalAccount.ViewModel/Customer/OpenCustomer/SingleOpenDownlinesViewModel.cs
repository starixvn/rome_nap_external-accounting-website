namespace Fanex.ExternalAccount.ViewModel.Customer.OpenCustomer
{
    using DTO.Customer.Status;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class SingleOpenDownlinesViewModel
    {
        private readonly IEnumerable<SingleOpenDownline> downlinesDetail;

        public SingleOpenDownlinesViewModel(int totalRows, IEnumerable<SingleOpenDownline> downlinesDetail)
        {
            TotalRows = totalRows;
            this.downlinesDetail = downlinesDetail;
        }

        public int TotalRows { get; }

        public ReadOnlyCollection<SingleOpenDownline> AffectedDownlines
        {
            get
            {
                return downlinesDetail.ToList().AsReadOnly();
            }
        }
    }
}