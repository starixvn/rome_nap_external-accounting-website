namespace Fanex.ExternalAccount.DataAccess.DataRange
{
    using Core.Shared;
    using Criteria.DataRange;
    using Fanex.Data.Repository;
    using DTO;
    using DTO.DataRange;

    public class DataRangeDao : IDataRangeDao
    {
        private readonly IDynamicRepository dynamicRepository;

        public DataRangeDao() : this(null)
        { }

        public DataRangeDao(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public Result Update(UpdateDataRangeCriteria criteria)
        {
            dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrorCode, criteria.ErrorMessage);
        }

        public CustomerDataRange Select(SelectDataRangeCriteria criteria)
        {
            var dataRangeModel = dynamicRepository.Get<DataRangeModel>(criteria);

            if (dataRangeModel == null)
            {
                return new CustomerDataRange { Type = CustomerDataRangeType.None };
            }

            var minDate = dataRangeModel.MinDate;
            var limitDays = dataRangeModel.LimitDays;
#pragma warning disable S3358 // Ternary operators should not be nested
            var type = minDate.HasValue
                ? CustomerDataRangeType.MinDate
                : limitDays.HasValue && limitDays.Value != 0 ? CustomerDataRangeType.TimePeriod
                : CustomerDataRangeType.None;
#pragma warning restore S3358 // Ternary operators should not be nested

            return new CustomerDataRange
            {
                Type = type,
                MinDate = minDate,
                LimitDays = limitDays
            };
        }
    }
}
