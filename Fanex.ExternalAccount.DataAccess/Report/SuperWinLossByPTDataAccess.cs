namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DTO.Report;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;

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
                        superPTList.Add(new SuperPositionTakingModel
                        {
                            CustId = reader.ToInt("CustID"),
                            PositionTaking = reader.To<decimal>("SuperPT"),
                            ProductId = reader.ToInt("ProductID"),
                            ProductName = reader.To<string>("ProductName"),
                        });
                    }
                }
            }

            return superPTList;
        }

        //private readonly CacheRefService cacheRefService;

        //public SuperWinLossDetailByPTDataAccessImpl()
        //{
        //    //this.cacheRefService = cacheRefService;
        //}

        public SuperWinLossDetailByProductModel GetWinLossByBetType(SuperWinLossDetailByPTCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria), "Criteria cannot be null");
            }

            var winlossByBettypeData = new List<SuperWinLossDetailByProductDTO>();
            var total = new SuperWinLossDetailByProductDTO();

            using (var reader = ExecDataReader("EAS_Rpt_WinLossDetail_byPT", new Hashtable
            {
                { "groupname", criteria.GroupName },
                { "fromdate", criteria.FromDate },
                { "todate", criteria.ToDate },
                { "balanceupday", criteria.BalanceUpDay },
                { "movebetsday", criteria.MoveBetsDay },
                { "productids", criteria.ProductIds },
                { "productdetails", criteria.ProductDetails },
                { "pagesize", criteria.PageSize },
                { "pageindex", criteria.PageIndex }
            }))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        winlossByBettypeData.Add(BuildWinLossByBetType(reader));
                    }

                    if (reader.NextResult() && reader.Read())
                    {
                        total = BuildWinLossByBetType(reader);
                    }
                }
            }

            return new SuperWinLossDetailByProductModel(winlossByBettypeData, total);
        }


        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
        private SuperWinLossDetailByProductDTO BuildWinLossByBetType(IDataReader reader)
        {
            var winLossByBettype = new SuperWinLossDetailByProductDTO
            {
                CustId = reader.ToInt("custid"),
                Username = reader.To<string>("username"),
                ProductId = reader.ToInt("ProductID"),
                ProductName = reader.To<string>("ProductName"),
                Turnover = reader.ToDouble("Turnover"),
                NetTurnover = reader.ToDouble("TurnoverNoDraw"),
                GrossComm = reader.ToDouble("GrossComm"),
                MemberWinlost = reader.ToDouble("MemberWinlost"),
                PlayerComm = reader.ToDouble("PlayerComm"),
                MemberTotal = reader.ToDouble("MemberTotal"),
                AgWinlost = reader.ToDouble("AgWinlost"),
                AgComm = reader.ToDouble("AgComm"),
                AgTotal = reader.ToDouble("AgTotal"),
                MAgWinlost = reader.ToDouble("MAgWinlost"),
                MAgComm = reader.ToDouble("MAgComm"),
                MAgTotal = reader.ToDouble("MAgTotal"),
                SAgWinlost = reader.ToDouble("SAgWinlost"),
                SAgComm = reader.ToDouble("SAgComm"),
                SAgTotal = reader.ToDouble("SAgTotal"),
                Company = reader.ToDouble("Company")
            };

            return winLossByBettype;
        }
    }
}