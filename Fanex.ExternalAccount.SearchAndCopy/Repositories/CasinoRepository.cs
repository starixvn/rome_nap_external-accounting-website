namespace Fanex.ExternalAccount.SearchAndCopy.Repositories
{
    using Fanex.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.NAP.Framework.Data;
    using System;

    public interface ICasinoRepository
    {
        Result UpdateSyncCasinoForIBC(ICriteria criteria);
    }

    public class CasinoRepository : ICasinoRepository
    {
        public Result UpdateSyncCasinoForIBC(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                var outputParams = db.GetOuputParams();
                var errorCode = Convert.ToInt32(outputParams["errcode"]);
                var errorMessage = outputParams["errmsg"].ToString();
                return new Result(errorCode, errorMessage);
            }
        }
    }
}
