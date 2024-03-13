namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Common;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DTO.Report;

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

            using (var reader = ExecDataReader(criteria.GetSettingKey(), criteria.GetSpParams()))
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
            SuperWinLossDetailByProductDTO dormantAccount = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    while (reader.Read())
                    {
                        var winLossByBetType = BuildWinLossByBetType(reader);
                        if (winLossByBetType.CustId == 0)
                        {
                            dormantAccount = winLossByBetType;
                            dormantAccount.ProductNames = new List<string>();
                        }
                        else
                        {
                            winlossByBettypeData.Add(winLossByBetType);
                        }
                    }

                    reader.NextResult();
                    while (reader.Read())
                    {
                        grandTotal = BuildWinLossByBetType(reader);
                    }
                }

                totalRecords = Converter.ToInt(db.GetOuputParams()["totalrecords"]);
            }

            return new SuperWinLossDetailByProductModel(winlossByBettypeData, grandTotal, totalRecords, dormantAccount);
        }

        private SuperWinLossDetailByProductDTO BuildWinLossByBetType(IDataReader reader)
        {
            var custId = Converter.ToInt(reader["custid"]);

            return new SuperWinLossDetailByProductDTO(
                custId,
                custId == 0 ? null : reader["username"].ToString(),
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