namespace Fanex.ExternalAccount.DataAccess.DataRange
{
    using Criteria.DataRange;
    using DTO;
    using DTO.DataRange;

    public interface IDataRangeDao
    {
        Result Update(UpdateDataRangeCriteria criteria);

        CustomerDataRange Select(SelectDataRangeCriteria criteria);
    }
}
