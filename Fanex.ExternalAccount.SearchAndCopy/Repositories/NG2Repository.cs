namespace Fanex.ExternalAccount.SearchAndCopy.Repositories
{
    using Fanex.Data;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.NAP.Framework.Data;
    using System;

    public interface ING2Repository
    {
        NG2Status GetStatus(ICriteria criteria);
    }

    public class NG2Repository : ING2Repository
    {
        public NG2Status GetStatus(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    reader.Read();
                    var isSync = Convert.ToBoolean(reader["issync"]);
                    var isDisabled = Convert.ToBoolean(reader["isdisabled"]);
                    return new NG2Status(isSync, isDisabled);
                }
            }

        }
    }
}
