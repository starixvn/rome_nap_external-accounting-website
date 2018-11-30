namespace Fanex.ExternalAccount.DataAccess.UserControl
{
    using Common;
    using Criteria.GroupConfiguration;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DTO;

    public interface IGroupConfigurationDAO
    {
        Result Update(UpdateGroupConfigurationCriteria criteria);
    }

    public class GroupConfigurationDAO : IGroupConfigurationDAO
    {
        public Result Update(UpdateGroupConfigurationCriteria criteria)
        {
            var result = DbHelper.ExecuteDbResult(criteria.GetSettingKey(), criteria.GetSpParams());

            var errorCode = Converter.ToInt(result.Outputs["errorcode"]);
            var errorMessage = Converter.ToInt(result.Outputs["errormessage"]);

            return new Result(errorCode, errorMessage);
        }
    }
}
