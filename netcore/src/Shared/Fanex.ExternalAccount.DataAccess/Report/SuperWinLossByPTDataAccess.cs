namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Common;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.DTO.Report;

    public interface SuperWinLossDetailByPTDataAccess
    {
        SuperWinLossDetailByProductModel GetWinLossByBetType(SuperWinLossDetailByPTCriteria criteria);

        IEnumerable<SuperPositionTakingModel> GetSuperPositionTakingList(SuperPTCriteria criteria);
    }

    public class SuperWinLossDetailByPTDataAccessImpl: SuperWinLossDetailByPTDataAccess
    {
        private readonly IDynamicRepository dynamicRepository;

        public SuperWinLossDetailByPTDataAccessImpl() : this(null)
        {
        }

        public SuperWinLossDetailByPTDataAccessImpl(IDynamicRepository dynamicRepository)
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

        public SuperWinLossDetailByProductModel GetWinLossByBetType(SuperWinLossDetailByPTCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria), "Criteria cannot be null");
            }

            var winlossByBettypeData = new List<SuperWinLossDetailByProductDTO>();
            var grandTotal = new SuperWinLossDetailByProductDTO();
            SuperWinLossDetailByProductDTO dormantAccount = null;
            
            var dataSet = dynamicRepository.FetchDataSet(criteria);
            
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return new SuperWinLossDetailByProductModel();
            }
            
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var winLossByBetType = BuildWinLossByBetType(row);
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
            }
            
            if (dataSet.Tables.Count > 1 && dataSet.Tables[1].Rows.Count > 0)
            {
                grandTotal = BuildWinLossByBetType(dataSet.Tables[1].Rows[0]);
            }
            
            return new SuperWinLossDetailByProductModel(winlossByBettypeData, grandTotal, criteria.TotalRecords, dormantAccount);
        }

        private SuperWinLossDetailByProductDTO BuildWinLossByBetType(DataRow row)
        {
            var custId = Converter.ToInt(row["custid"]);

            return new SuperWinLossDetailByProductDTO(
                custId,
                custId == 0 ? null : row["username"].ToString(),
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
    }
}