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
            var total = new SuperWinLossDetailByProductDTO();
            var totalRecords = 0;

            using (var db = new ObjectDb("EAS_Rpt_WinLossDetail_byPT"))
            {
                db.AssignParamValues(new Hashtable {
                    { "groupname", criteria.GroupName },
                    { "fromdate", criteria.FromDate },
                    { "todate", criteria.ToDate },
                    { "balanceupday", criteria.BalanceUpDay },
                    { "movebetsday", criteria.MoveBetsDay },
                    { "productids", criteria.ProductIds },
                    { "productdetails", criteria.ProductDetails },
                    { "pagesize", criteria.PageSize },
                    { "pageindex", criteria.PageIndex }
                });

                using (var dataset = db.ExecuteDataSet())
                {
                    if (dataset != null && dataset.Tables.Count > 0)
                    {
                        foreach (DataRow row in dataset.Tables[0].Rows)
                        {
                            winlossByBettypeData.Add(BuildWinLossByBetType(row));
                        }

                        total = BuildWinLossByBetType(dataset.Tables[1].Rows[0]);
                    }

                    totalRecords = Converter.ToInt(db.GetOuputParams()["totalrecords"]);
                }
            }

            return new SuperWinLossDetailByProductModel(winlossByBettypeData, total, totalRecords);
        }


        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
        private SuperWinLossDetailByProductDTO BuildWinLossByBetType(DataRow row)
        {
            var winLossByBettype = new SuperWinLossDetailByProductDTO
            {
                CustId = Converter.ToInt(row["custid"]),
                Username = row["username"].ToString(),
                ProductId = Converter.ToInt(row["ProductID"]),
                ProductName = row["ProductName"].ToString(),
                Turnover = Converter.ToDecimal(row["Turnover"]),
                NetTurnover = Converter.ToDecimal(row["TurnoverNoDraw"]),
                GrossComm = Converter.ToDecimal(row["GrossComm"]),
                MemberWinlost = Converter.ToDecimal(row["MemberWinlost"]),
                PlayerComm = Converter.ToDecimal(row["PlayerComm"]),
                MemberTotal = Converter.ToDecimal(row["MemberTotal"]),
                AgWinlost = Converter.ToDecimal(row["AgWinlost"]),
                AgComm = Converter.ToDecimal(row["AgComm"]),
                AgTotal = Converter.ToDecimal(row["AgTotal"]),
                MAgWinlost = Converter.ToDecimal(row["MAgWinlost"]),
                MAgComm = Converter.ToDecimal(row["MAgComm"]),
                MAgTotal = Converter.ToDecimal(row["MAgTotal"]),
                SAgWinlost = Converter.ToDecimal(row["SAgWinlost"]),
                SAgComm = Converter.ToDecimal(row["SAgComm"]),
                SAgTotal = Converter.ToDecimal(row["SAgTotal"]),
                Company = Converter.ToDecimal(row["Company"])
            };

            return winLossByBettype;
        }
    }
}