namespace Fanex.ExternalAccount.Service.DataRange
{
    using DataAccess.Criteria.DataRange;
    using DTO;
    using DTO.DataRange;

    public interface IDataRangeService
    {
        Result Update(UpdateDataRangeCriteria criteria);

        CustomerDataRange Select(SelectDataRangeCriteria criteria);
    }
}
