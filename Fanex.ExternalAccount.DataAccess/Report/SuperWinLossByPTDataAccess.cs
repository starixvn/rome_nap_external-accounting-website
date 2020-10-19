namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Report;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;

    public interface SuperWinLossDetailByPTDataAccess
    {
        IEnumerable<WinLossByBetTypeModel> GetWinLossByBetType(SuperWinLossDetailByPTCriteria criteria);
    }

    public class SuperWinLossDetailByPTDataAccessImpl : SuperWinLossDetailByPTDataAccess
    {
        //private readonly CacheRefService cacheRefService;

        //public SuperWinLossDetailByPTDataAccessImpl()
        //{
        //    //this.cacheRefService = cacheRefService;
        //}

        public IEnumerable<WinLossByBetTypeModel> GetWinLossByBetType(SuperWinLossDetailByPTCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria), "Criteria cannot be null");
            }

            var winlossByBettypeData = new List<WinLossByBetTypeModel>();

            using (var db = new ObjectDb("Age_Rpt_WinLossByProduct_Master"))
            {
                var reader = db.ExecuteReader(new Hashtable
                    {
                        { "CustId", criteria.CustId },
                        { "FromDate", criteria.FromDate },
                        { "ToDate", criteria.ToDate },
                        { "BalanceUpDay", criteria.BalanceUpDay },
                        { "MoveBetsDay", criteria.MoveBetsDay },
                        { "ProductIds", criteria.ProductIds },
                        { "ProductDetails", criteria.ProductDetails }
                    });

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        winlossByBettypeData.Add(BuildWinLossByBetType(reader));
                    }
                }
            }

            return winlossByBettypeData;
        }


        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
        private WinLossByBetTypeModel BuildWinLossByBetType(IDataReader reader)
        {
            var productId = Convert.IsDBNull(reader["ProductID"]) ? 0 : reader.ToInt("ProductID");
            var sportTypeId = Convert.IsDBNull(reader["SportType"]) ? 0 : reader.ToInt("SportType");
            var bettypeId = reader.ToInt("bettype");

            var winLossByBettype = new WinLossByBetTypeModel(
                 betTypeId: bettypeId,
                 betTypeName: "betTypeName", //Core.Services.Impl.CacheRef.GetBettype(bettypeId),
                 productId: productId,
                 sportTypeId: sportTypeId,
                 sportTypeName: sportTypeId == 0 ? string.Empty : "Sport Name",//cacheRefService.GetSportName(sportTypeId),
                 liveIndicator: reader.ToString("liveindicator") == "1",
                 betCount: reader.ToInt("NumberOfTickets"),
                 turnover: reader.ToDecimal("TurnOver"),
                 grossComm: reader.ToDecimal("GrossComm"),
                 memberNetWinLoss: null,
                 agentNetWinLoss: NetWinLoss.ReadAgentNetWinLoss(reader, UserLevel.Super),
                 masterNetWinLoss: NetWinLoss.ReadMasterNetWinLoss(reader, UserLevel.Super),
                 superNetWinLoss: NetWinLoss.ReadSuperNetWinLoss(reader, UserLevel.Super));

            return winLossByBettype;
        }
    }
}