using Fanex.ExternalAccount.DataAccess.Products;

namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.DataAccesses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Data;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Models;
    using Fanex.NAP.Framework.Data;

    public interface ISuperWinLossDetailBySubGroupDataAccess
    {
        SuperWinLossDetailBySubGroupReport GetSuperWinLossDetailBySubGroup(ICriteria criteria);
        string GetSubGroupList(int groupId, string subGroup);
    }

    public class SuperWinLossDetailBySubGroupDataAccess : ISuperWinLossDetailBySubGroupDataAccess
    {
        public SuperWinLossDetailBySubGroupReport GetSuperWinLossDetailBySubGroup(ICriteria criteria)
        {
            var superWinLossDetails = new List<SuperWinLossDetailBySubGroupModel>();
            SuperWinLossDetailBySubGroupModel total = null;
            int totalItems;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    while (reader.Read())
                    {
                        superWinLossDetails.Add(GetSuperWinLossDetail(reader));
                    }

                    if (superWinLossDetails.Count > 0 && reader.NextResult() && reader.Read())
                    {
                        total = GetTotal(reader);
                    }
                }

                totalItems = Convert.ToInt32(db.GetOuputParams()["totalrecords"]);
            }

            return new SuperWinLossDetailBySubGroupReport(superWinLossDetails, total, totalItems);
        }

        private static SuperWinLossDetailBySubGroupModel GetSuperWinLossDetail(IDataReader reader)
        {
            var subGroup = reader["SubGroup"];

            var superWinLossDetail = GetTotal(reader);
            superWinLossDetail.SubGroup = subGroup == null || subGroup == DBNull.Value
                ? null
                : subGroup.ToString();

            return superWinLossDetail;
        }

        public string GetSubGroupList(int groupId, string subGroup)
        {
            var criteria = new GetSubGroupListCriteria(groupId, subGroup);
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                var ouputParams = db.GetOuputParams();
                return ouputParams["subgrouplist"] == null ? string.Empty : ouputParams["subgrouplist"].ToString();
            }
        }

        private static SuperWinLossDetailBySubGroupModel GetTotal(IDataReader reader)
            => new SuperWinLossDetailBySubGroupModel(
                reader["UserName"].ToString(),
                Convert.ToDecimal(reader["Turnover"]),
                Convert.ToDecimal(reader["TurnoverNoDraw"]),
                Convert.ToDecimal(reader["GrossComm"]),
                Convert.ToDecimal(reader["MemberWinLost"]),
                Convert.ToDecimal(reader["PlayerComm"]),
                Convert.ToDecimal(reader["MemberTotal"]),
                Convert.ToDecimal(reader["AgWinLost"]),
                Convert.ToDecimal(reader["AgComm"]),
                Convert.ToDecimal(reader["AgTotal"]),
                Convert.ToDecimal(reader["MAgWinLost"]),
                Convert.ToDecimal(reader["MAgComm"]),
                Convert.ToDecimal(reader["MAgTotal"]),
                Convert.ToDecimal(reader["SAgWinLost"]),
                Convert.ToDecimal(reader["SAgComm"]),
                Convert.ToDecimal(reader["SAgTotal"]),
                Convert.ToDecimal(reader["Company"]));
    }
}
