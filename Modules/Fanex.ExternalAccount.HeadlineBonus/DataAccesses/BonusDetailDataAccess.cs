namespace Fanex.ExternalAccount.HeadlineBonus.DataAccesses
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.HeadlineBonus.Criteria;
    using Fanex.ExternalAccount.HeadlineBonus.Models;

    internal interface IBonusDetailDataAccess
    {
        IEnumerable<MemberBonusEntity> GetMemberBonus(GetMemberBonusCriteria criteria);

        Result CancelPendingBonus(CancelPendingBonusCriteria criteria);

        Result AddNewBonus(AddNewBonusCriteria criteria);
    }

    internal class BonusDetailDataAccess : IBonusDetailDataAccess
    {
        private readonly IDynamicRepository repository;

        public BonusDetailDataAccess(IDynamicRepository repository = null) =>
            this.repository = repository ?? new DynamicRepository();

        public IEnumerable<MemberBonusEntity> GetMemberBonus(GetMemberBonusCriteria criteria)
            => repository.Fetch<MemberBonusEntity>(criteria);
        
        public Result CancelPendingBonus(CancelPendingBonusCriteria criteria)
        {
            repository.Execute(criteria);
            return new Result(Convert.ToInt32(criteria.Errcode), criteria.Errmsg);
        }

        public Result AddNewBonus(AddNewBonusCriteria criteria)
        {
            repository.Execute(criteria);
            return new Result(Convert.ToInt32(criteria.Errcode), criteria.Errmsg);
        }
    }
}