using Fanex.Data;
using Fanex.ExternalAccount.ViewModel.ViewMaxPayoutSettingInformation;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.Service.ViewMaxPayoutSettingInformation
{
    public class ViewMaxPayoutSettingInformationService
    {
        public IEnumerable<SabaSportViewModel> GetSabaSports(string site)
        {
            using (var objDb = new ObjectDb("Acc_SabaVirtualSportsBetSettingRef_Sel"))
            {
                objDb.AddInParameter("site", System.Data.DbType.String, site);
                var reader = objDb.ExecuteReader();
                while (reader.Read())
                {
                    yield return new SabaSportViewModel {
                        SettingId = reader[0].ToString(),
                        SettingIdFollowed = reader[1].ToString()
                    };
                }
            }
        }
    }
}
