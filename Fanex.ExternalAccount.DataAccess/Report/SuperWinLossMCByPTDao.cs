namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using Fanex.Common;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.DTO.Report;
    using System;
    using System.Collections.Generic;
    using System.Data;

    public interface SuperWinLossMCDetailByPTDao
    {
        SuperWinLossDetailMCByProductModel GetWinLossMCByBetType(SuperWinLossDetailMCByPTCriteria criteria);

        IEnumerable<SuperPositionTakingModel> GetSuperPositionTakingList(SuperPTCriteria criteria);
    }

    public class SuperWinLossMCDetailByPTDaoImpl : SuperWinLossMCDetailByPTDao
    {
        private readonly IDynamicRepository dynamicRepository;

        public SuperWinLossMCDetailByPTDaoImpl() : this(null)
        {
        }

        public SuperWinLossMCDetailByPTDaoImpl(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<SuperPositionTakingModel> GetSuperPositionTakingList(SuperPTCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria), "Criteria cannot be null");
            }

            return dynamicRepository.Fetch<SuperPositionTakingModel>(criteria);
        }

        public SuperWinLossDetailMCByProductModel GetWinLossMCByBetType(SuperWinLossDetailMCByPTCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria), "Criteria cannot be null");
            }

            var winlossByBettypeData = new List<SuperWinLossDetailMCByProductDTO>();
            var totalByCurrencies = new List<SuperWinLossDetailMCByProductDTO>();
            
            var dataSet = dynamicRepository.FetchDataSet(criteria);
            
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return new SuperWinLossDetailMCByProductModel(new List<SuperWinLossDetailMCByProductDTO>(), new List<SuperWinLossDetailMCByProductDTO>());
            }
            
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    winlossByBettypeData.Add(BuildWinLossByBetType(row));
                }
            }
            
            if (dataSet.Tables.Count > 1 && dataSet.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[1].Rows)
                {
                    totalByCurrencies.Add(BuildTotalWinLossMCByBetType(row));
                }
            }
            
            return new SuperWinLossDetailMCByProductModel(winlossByBettypeData, totalByCurrencies);
        }

        private static SuperWinLossDetailMCByProductDTO BuildWinLossByBetType(DataRow row)
        {
            return new SuperWinLossDetailMCByProductDTO(
                Converter.ToInt(row["custid"]),
                row["username"].ToString(),
                Converter.ToInt(row["currencyid"]),
                row["currencyname"].ToString(),
                Converter.ToInt(row["ProductID"]),
                row["ProductName"].ToString(),
                Converter.ToDecimal(row["Turnover"]),
                Converter.ToDecimal(row["TurnoverNoDraw"]),
                Converter.ToDecimal(row["GrossComm"]),
                Converter.ToDecimal(row["MemberWinlost"]),
                Converter.ToDecimal(row["PlayerComm"]),
                Converter.ToDecimal(row["MemberTotal"]),
                Converter.ToDecimal(row["AgWinlost"]),
                Converter.ToDecimal(row["AgComm"]),
                Converter.ToDecimal(row["AgTotal"]),
                Converter.ToDecimal(row["MAgWinlost"]),
                Converter.ToDecimal(row["MAgComm"]),
                Converter.ToDecimal(row["MAgTotal"]),
                Converter.ToDecimal(row["SAgWinlost"]),
                Converter.ToDecimal(row["SAgComm"]),
                Converter.ToDecimal(row["SAgTotal"]),
                Converter.ToDecimal(row["Company"])
            );
        }

        private static SuperWinLossDetailMCByProductDTO BuildTotalWinLossMCByBetType(DataRow row)
        {
            return new SuperWinLossDetailMCByProductDTO(
                0,
                row["total"].ToString(),
                0,
                null,
                0,
                null,
                Converter.ToDecimal(row["Turnover"]),
                Converter.ToDecimal(row["TurnoverNoDraw"]),
                Converter.ToDecimal(row["GrossComm"]),
                Converter.ToDecimal(row["MemberWinlost"]),
                Converter.ToDecimal(row["PlayerComm"]),
                Converter.ToDecimal(row["MemberTotal"]),
                Converter.ToDecimal(row["AgWinlost"]),
                Converter.ToDecimal(row["AgComm"]),
                Converter.ToDecimal(row["AgTotal"]),
                Converter.ToDecimal(row["MAgWinlost"]),
                Converter.ToDecimal(row["MAgComm"]),
                Converter.ToDecimal(row["MAgTotal"]),
                Converter.ToDecimal(row["SAgWinlost"]),
                Converter.ToDecimal(row["SAgComm"]),
                Converter.ToDecimal(row["SAgTotal"]),
                Converter.ToDecimal(row["Company"])
            );
        }
    }
}