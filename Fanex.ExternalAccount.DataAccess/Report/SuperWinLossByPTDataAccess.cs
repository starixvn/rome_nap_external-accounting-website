namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using Fanex.Common;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DTO.Report;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    public interface SuperWinLossDetailByPTDataAccess
    {
        SuperWinLossDetailByProductModel GetWinLossByBetType(SuperWinLossDetailByPTCriteria criteria);

        IEnumerable<SuperPositionTakingModel> GetSuperPositionTakingList(SuperPTCriteria criteria);
    }

    public class SuperWinLossDetailByPTDataAccessImpl : DAOBase, SuperWinLossDetailByPTDataAccess
    {
        public IEnumerable<SuperPositionTakingModel> GetSuperPositionTakingList(SuperPTCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria), "Criteria cannot be null");
            }

            var superPTList = new List<SuperPositionTakingModel>();

            using (var reader = ExecDataReader(criteria.GetSettingKey(), new Hashtable
                    {
                        { "supers", criteria.Supers }
                    }))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        superPTList.Add(new SuperPositionTakingModel(
                            reader.ToInt("CustID"),
                            reader.To<decimal>("SuperPT"),
                            reader.ToInt("ProductID"),
                            reader.To<string>("ProductName")));
                    }
                }
            }

            return superPTList;
        }

        public SuperWinLossDetailByProductModel GetWinLossByBetType(SuperWinLossDetailByPTCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria), "Criteria cannot be null");
            }

            var winlossByBettypeData = new List<SuperWinLossDetailByProductDTO>();
            var grandTotal = new SuperWinLossDetailByProductDTO();
            var totalRecords = 0;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = db.ExecuteReader(criteria.GetSpParams());

                while (reader.Read())
                {
                    winlossByBettypeData.Add(BuildWinLossByBetType(reader));
                }

                reader.NextResult();
                while (reader.Read())
                {
                    grandTotal = BuildWinLossByBetType(reader);
                }

                reader.Close();
                totalRecords = Converter.ToInt(db.GetOuputParams()["totalrecords"]);
            }

            return new SuperWinLossDetailByProductModel(winlossByBettypeData, grandTotal, totalRecords);
        }

        private SuperWinLossDetailByProductDTO BuildWinLossByBetType(IDataReader reader)
        {
            return new SuperWinLossDetailByProductDTO(
                Converter.ToInt(reader["custid"]),
                reader["username"].ToString(),
                Converter.ToInt(reader["ProductID"]),
                reader["ProductName"].ToString(),
                Converter.ToDecimal(reader["Turnover"]),
                Converter.ToDecimal(reader["TurnoverNoDraw"]),
                Converter.ToDecimal(reader["GrossComm"]),
                Converter.ToDecimal(reader["MemberWinlost"]),
                Converter.ToDecimal(reader["PlayerComm"]),
                Converter.ToDecimal(reader["MemberTotal"]),
                Converter.ToDecimal(reader["AgWinlost"]),
                Converter.ToDecimal(reader["AgComm"]),
                Converter.ToDecimal(reader["AgTotal"]),
                Converter.ToDecimal(reader["MAgWinlost"]),
                Converter.ToDecimal(reader["MAgComm"]),
                Converter.ToDecimal(reader["MAgTotal"]),
                Converter.ToDecimal(reader["SAgWinlost"]),
                Converter.ToDecimal(reader["SAgComm"]),
                Converter.ToDecimal(reader["SAgTotal"]),
                Converter.ToDecimal(reader["Company"])
            );
        }
    }
}