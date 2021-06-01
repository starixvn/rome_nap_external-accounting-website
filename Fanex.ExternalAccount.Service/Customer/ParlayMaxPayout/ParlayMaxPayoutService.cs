namespace Fanex.ExternalAccount.Service.Customer.ParlayMaxPayout
{
    using Fanex.ExternalAccount.DataAccess.Customer.ParlayMaxPayout;
    using Fanex.ExternalAccount.DataAccess.ParlayMaxPayout;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Requests.ParlayMaxPayout;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer.ParlayMaxPayout;
    using Fanex.Tarus.Customer.Criteria.ParlayMaxPayout;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IParlayMaxPayoutService
    {
        ParlayMaxPayoutViewModel Select(ParlayMaxPayoutSelectRequest request);

        Result Update(ParlayMaxPayoutUpdateRequest request);

        IEnumerable<Result> UpdateMultiple(ParlayMaxPayoutUpdateMultipleRequest request);
    }

    public class ParlayMaxPayoutService : IParlayMaxPayoutService
    {
        private readonly IParlayMaxPayoutDao _parlayMaxPayoutDataAccess;

        public ParlayMaxPayoutService() : this(null) { }

        public ParlayMaxPayoutService(IParlayMaxPayoutDao parlayMaxPayoutDataAccess)
        {
            _parlayMaxPayoutDataAccess = parlayMaxPayoutDataAccess ?? new ParlayMaxPayoutDao();
        }

        public ParlayMaxPayoutViewModel Select(ParlayMaxPayoutSelectRequest request)
            => new ParlayMaxPayoutViewModel
            {
                CustId = request.CustId,
                CustIds = request.CustIds,
                CustName = request.CustName,
                CustNames = request.CustNames,
                ListEntityModel = _parlayMaxPayoutDataAccess.Select(new MaxPayoutSelectCriteria { CustId = request.CustId, SettingStatus = Config.SettingStatus })
            };

        public Result Update(ParlayMaxPayoutUpdateRequest request) => _parlayMaxPayoutDataAccess.Update(new MaxPayoutUpdateCriteria(request));

        public IEnumerable<Result> UpdateMultiple(ParlayMaxPayoutUpdateMultipleRequest request)
        {
            var listCustIds = request.CustIds?.Split(',').Select(Int32.Parse).ToList();
            foreach (int custId in listCustIds)
            {
                request.CustId = custId;
                yield return Update(request);
            }
        }
    }
}
