namespace Fanex.ExternalAccount.DataAccess.DataRange
{
    using Criteria.DataRange;
    using DTO;

    public interface IDataRangeDao
    {
        Result Update(UpdateDataRangeCriteria criteria);
    }
}
