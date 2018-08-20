namespace Fanex.ExternalAccount.Service.DataRange
{
    using DataAccess.Criteria.DataRange;
    using DataAccess.DataRange;
    using DTO;

    public class DataRangeService : IDataRangeService
    {
        private readonly IDataRangeDao dataRangeDao;

        public DataRangeService(IDataRangeDao dataRangeDao = null)
        {
            this.dataRangeDao = dataRangeDao ?? new DataRangeDao();
        }

        public Result Update(UpdateDataRangeCriteria criteria)
            => dataRangeDao.Update(criteria);
    }
}
