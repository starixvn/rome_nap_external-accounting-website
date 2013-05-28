using System;
using Fanex.Data;
using Fanex.Puffin.Admin.Criteria;
using Fanex.Puffin.Admin.Repositories.Interface;
using Fanex.Puffin.Core.Services.Impl;

namespace Fanex.Puffin.Admin.Repositories.Impl
{
    public class AdminRepository: IAdminRepository
    {
        public bool CheckUserOnlineStatus(AccountingUserCheckOnlineStatusCriteria criteria)
        {
            try
            {
                using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
                {
                    objectDb.ExecuteNonQuery(criteria.GetSpParams());
                    return !Convert.ToBoolean(objectDb.GetParameterValue("iskickedout"));
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }
            return false;
        }
    }
}
