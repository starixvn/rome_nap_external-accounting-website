namespace Fanex.ExternalAccount.ViewModel.Customer.Suspend
{
    using DTO.Customer.Status;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class SingleUpdateViewModel
    {
        private readonly IEnumerable<SingleUpdateAffectedDownline> _affectedDowlines;

        public SingleUpdateViewModel(int custId, string userName, IEnumerable<SingleUpdateAffectedDownline> affectedDowlines)
        {
            CustId = custId;
            UserName = userName;
            _affectedDowlines = affectedDowlines;
        }

        public int CustId { get; }

        public string UserName { get; }

        public ReadOnlyCollection<SingleUpdateAffectedDownline> AffectedDowlines()
        {
            if (_affectedDowlines?.ToList().Count > 1)
            {
                return _affectedDowlines?.Concat(new[] { TotalRow })?.ToList()?.AsReadOnly();
            }

            return _affectedDowlines?.ToList().AsReadOnly();
        }

        private SingleUpdateAffectedDownline TotalRow
        {
            get
            {
                if (_affectedDowlines?.ToList().Count > 1)
                {
                    int totalAffectedDownlines = 0;
                    int totalRemainUnchangedDownlines = 0;
                    foreach (var row in _affectedDowlines)
                    {
                        totalAffectedDownlines += row.AffectedDownlines;
                        totalRemainUnchangedDownlines += row.RemainUnchangedDownlines;
                    }

                    return new SingleUpdateAffectedDownline(0, totalAffectedDownlines, totalRemainUnchangedDownlines);
                }

                return null;
            }
        }
    }
}
