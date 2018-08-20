namespace Fanex.ExternalAccount.Service.DataRange
{
    using DataAccess.Criteria.DataRange;
    using DTO;

    public interface IDataRangeService
    {
        Result Update(UpdateDataRangeCriteria criteria);
    }
}
