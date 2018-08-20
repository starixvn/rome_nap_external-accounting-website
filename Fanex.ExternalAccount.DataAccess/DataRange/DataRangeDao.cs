namespace Fanex.ExternalAccount.DataAccess.DataRange
{
    using Common;
    using Criteria.DataRange;
    using Data.DNAide;
    using DTO;

    public class DataRangeDao : IDataRangeDao
    {
        public Result Update(UpdateDataRangeCriteria criteria)
        {
            var result = DbHelper.ExecuteDbResult(criteria.GetSettingKey(), criteria.GetSpParams());

            var errorCode = Converter.ToInt(result.Outputs["errorcode"]);
            var errorMessage = Converter.ToInt(result.Outputs["errormessage"]);

            return new Result(errorCode, errorMessage);
        }
    }
}
