namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.DataAccesses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Data;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Models;
    // using Fanex.NAP.Framework.Data;
    using System.Linq;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Products;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Criteria;
    using System.Linq;
    
    public interface ISuperWinLossDetailBySubGroupDataAccess
    {
        SuperWinLossDetailBySubGroupReport GetSuperWinLossDetailBySubGroup(GetSuperWinLossDetailBySubGroupCriteria criteria);
        string GetSubGroupList(int groupId, string subGroup);
    }

    public class SuperWinLossDetailBySubGroupDataAccess : ISuperWinLossDetailBySubGroupDataAccess
    {
        private readonly IDynamicRepository repository;
        public SuperWinLossDetailBySubGroupDataAccess(IDynamicRepository repository = null) => this.repository = repository ?? new DynamicRepository();
        public SuperWinLossDetailBySubGroupReport GetSuperWinLossDetailBySubGroup(GetSuperWinLossDetailBySubGroupCriteria criteria)
        {
            var superWinLossDetails = new List<SuperWinLossDetailBySubGroupModel>();
            // SuperWinLossDetailBySubGroupModel total = null;
            // int totalItems;
            //
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     using (var reader = db.ExecuteReader(criteria.GetSpParams()))
            //     {
            //         while (reader.Read())
            //         {
            //             superWinLossDetails.Add(GetSuperWinLossDetail(reader));
            //         }
            //
            //         if (superWinLossDetails.Count > 0 && reader.NextResult() && reader.Read())
            //         {
            //             total = GetTotal(reader);
            //         }
            //     }
            //
            //     totalItems = Convert.ToInt32(db.GetOuputParams()["totalrecords"]);
            // }

            //return new SuperWinLossDetailBySubGroupReport(superWinLossDetails, total, totalItems);
           var dbResult = repository.FetchMultiple<SuperWinLossDetailBySubGroupModel, SuperWinLossDetailBySubGroupModel>(criteria);
           return new SuperWinLossDetailBySubGroupReport(dbResult.Item1, dbResult.Item2.FirstOrDefault(), criteria.TotalRecords);
        }

        // private static SuperWinLossDetailBySubGroupModel GetSuperWinLossDetail(IDataReader reader)
        // {
        //     var subGroup = reader["SubGroup"];
        //
        //     var superWinLossDetail = GetTotal(reader);
        //     superWinLossDetail.SubGroup = subGroup == null || subGroup == DBNull.Value
        //         ? null
        //         : subGroup.ToString();
        //
        //     return superWinLossDetail;
        // }

        public string GetSubGroupList(int groupId, string subGroup)
        {
            var criteria = new GetSubGroupListCriteria(groupId, subGroup);
           
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     db.ExecuteNonQuery(criteria.GetSpParams());
            //     var ouputParams = db.GetOuputParams();
            //     return ouputParams["subgrouplist"] == null ? string.Empty : ouputParams["subgrouplist"].ToString();
            // }
            repository.Execute(criteria);
            return criteria.Subgrouplist;
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
