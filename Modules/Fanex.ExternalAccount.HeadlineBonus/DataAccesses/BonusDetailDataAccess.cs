namespace Fanex.ExternalAccount.HeadlineBonus.DataAccesses
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.HeadlineBonus.Criteria;
    using Fanex.ExternalAccount.HeadlineBonus.Models;

    internal interface IBonusDetailDataAccess
    {
        IEnumerable<MemberBonusEntity> GetMemberBonus(GetMemberBonusCriteria criteria);

        Result CancelPendingBonus(CancelPendingBonusCriteria criteria);
        
        Result AddNewBonus(AddNewBonusCriteria criteria);
    }

    internal class BonusDetailDataAccess : IBonusDetailDataAccess
    {
        private readonly IDynamicRepository repository;
        public BonusDetailDataAccess(IDynamicRepository repository = null) => this.repository = repository;
        public IEnumerable<MemberBonusEntity> GetMemberBonus(GetMemberBonusCriteria criteria)
        {
            var memberBonus = new List<MemberBonusEntity>();
            
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     using (var reader = db.ExecuteReader(criteria.GetSpParams()))
            //     {
            //         while (reader.Read())
            //         {
            //             memberBonus.Add(GetMemberBonusEntity(reader));
            //         }
            //     }
            //
            //     criteria.TotalItems = Convert.ToInt32(db.GetOuputParams()["totalrows"]);
            // }
            //
            // return memberBonus;
            
            var dbResult = this.repository.Fetch<MemberBonusEntity>(criteria);
            return dbResult;
        }

        // private static MemberBonusEntity GetMemberBonusEntity(IDataReader reader)
        //     => new MemberBonusEntity(
        //         Convert.ToDateTime(reader["createddate"]),
        //         Convert.ToInt64(reader["transid"]),
        //         reader["supername"].ToString(),
        //         Convert.ToInt32(reader["custid"]),
        //         reader["username"].ToString(),
        //         Convert.ToDecimal(reader["amount"]),
        //         ToDateTime(reader["paymentdate"]),
        //         Convert.ToByte(reader["statusid"]),
        //         Convert.ToBoolean(reader["iscomplained"]));

        private static DateTime? ToDateTime(object value)
            => value == null || value == DBNull.Value ? default(DateTime?) : Convert.ToDateTime(value);

        public Result CancelPendingBonus(CancelPendingBonusCriteria criteria)
        {
            this.repository.Execute(criteria);
            return new Result(Convert.ToInt32(criteria.Errcode), criteria.Errmsg);
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     db.ExecuteNonQuery(criteria.GetSpParams());
            //     var ouputParams = db.GetOuputParams();
            //
            //     return new Result(Convert.ToInt32(ouputParams["errcode"]), ouputParams["errmsg"].ToString());
            // }
        }
        public Result AddNewBonus(AddNewBonusCriteria criteria)
        {
            this.repository.Execute(criteria);
            return new Result(Convert.ToInt32(criteria.Errcode), criteria.Errmsg);
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     db.ExecuteNonQuery(criteria.GetSpParams());
            //     var ouputParams = db.GetOuputParams();
            //
            //     return new Result(Convert.ToInt32(ouputParams["errcode"]), ouputParams["errmsg"].ToString());
            // }
        }
    }
}
