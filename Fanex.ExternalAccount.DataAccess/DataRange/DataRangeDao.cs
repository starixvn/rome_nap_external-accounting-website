namespace Fanex.ExternalAccount.DataAccess.DataRange
{
    using System;
    using Common;
    using Core.Shared;
    using Criteria.DataRange;
    using Data.DNAide;
    using DTO;
    using DTO.DataRange;

    public class DataRangeDao : IDataRangeDao
    {
        public Result Update(UpdateDataRangeCriteria criteria)
        {
            var result = DbHelper.ExecuteDbResult(criteria.GetSettingKey(), criteria.GetSpParams());

            var errorCode = Converter.ToInt(result.Outputs["errorcode"]);
            var errorMessage = Converter.ToInt(result.Outputs["errormessage"]);

            return new Result(errorCode, errorMessage);
        }

        public CustomerDataRange Select(SelectDataRangeCriteria criteria)
        {
            var result = DbHelper.ExecuteDbResult(criteria.GetSettingKey(), criteria.GetSpParams());

            var dataTable = result.ResultSet.Tables[0];
            if (dataTable.Rows.Count == 0)
            {
                return new CustomerDataRange { Type = CustomerDataRangeType.None };
            }

            var minDate = dataTable.Rows[0]["MinDate"];
            var limitDays = dataTable.Rows[0]["LimitDays"];
            var type = minDate != DBNull.Value
                ? CustomerDataRangeType.MinDate
                : limitDays != DBNull.Value && Converter.ToInt(limitDays) != 0 ? CustomerDataRangeType.TimePeriod
                : CustomerDataRangeType.None;

            return new CustomerDataRange
            {
                Type = type,
                MinDate = minDate == DBNull.Value ? (DateTime?)null : Converter.ToDateTime(minDate),
                LimitDays = limitDays == DBNull.Value ? (int?)null : Converter.ToInt(limitDays)
            };
        }
    }
}
