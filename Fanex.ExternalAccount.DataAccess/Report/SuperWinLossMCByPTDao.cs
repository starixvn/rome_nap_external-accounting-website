namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using Fanex.Common;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DTO.Report;
    using System;
    using System.Collections.Generic;
    using System.Data;

    public interface SuperWinLossMCDetailByPTDao
    {
        SuperWinLossDetailMCByProductModel GetWinLossMCByBetType(SuperWinLossDetailMCByPTCriteria criteria);

        IEnumerable<SuperPositionTakingModel> GetSuperPositionTakingList(SuperPTCriteria criteria);
    }

    public class SuperWinLossMCDetailByPTDaoImpl : DAOBase, SuperWinLossMCDetailByPTDao
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

        public SuperWinLossDetailMCByProductModel GetWinLossMCByBetType(SuperWinLossDetailMCByPTCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria), "Criteria cannot be null");
            }

            var winlossByBettypeData = new List<SuperWinLossDetailMCByProductDTO>();
            var totalByCurrencies = new List<SuperWinLossDetailMCByProductDTO>();

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
                    totalByCurrencies.Add(BuildTotalWinLossMCByBetType(reader));
                }

                reader.Close();
            }

            return new SuperWinLossDetailMCByProductModel(winlossByBettypeData, totalByCurrencies);
        }

        private static SuperWinLossDetailMCByProductDTO BuildWinLossByBetType(IDataReader reader)
        {
            return new SuperWinLossDetailMCByProductDTO(
                Converter.ToInt(reader["custid"]),
                reader["username"].ToString(),
                Converter.ToInt(reader["currencyid"]),
                reader["currencyname"].ToString(),
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

        private static SuperWinLossDetailMCByProductDTO BuildTotalWinLossMCByBetType(IDataReader reader)
        {
            return new SuperWinLossDetailMCByProductDTO(
                0,
                reader["total"].ToString(),
                0,
                null,
                0,
                null,
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