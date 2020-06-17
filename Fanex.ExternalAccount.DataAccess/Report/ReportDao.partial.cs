namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Common.Extensions;
    using DTO.Report;

    public partial class ReportDao
    {
        const string eASRptWinLossDetailIBCBetLiveCasinoCom = "EAS_Rpt_WinLossExtraDetail_Com";

        public List<WinLossDetailIBCBetLiveCasino> EAS_Rpt_WinLossDetailIBCBetLiveCasino_Com(
            string groupName,
            int customerId,
            string productIds,
            DateTime fromDate,
            DateTime toDate,
            DateTime balanceUpDay)
        {
            var parameters = new Hashtable
            {
                {ParameterNameConstants.GroupName, groupName},
                {ParameterNameConstants.FromDate, fromDate},
                {ParameterNameConstants.ToDate, toDate},
                {ParameterNameConstants.BalanceUpDay, balanceUpDay},
                {ParameterNameConstants.ProductIds, productIds}
            };
            if (customerId != 0)
            {
                parameters.Add(ParameterNameConstants.CustomerId, customerId);
            }
            var winLossDetailIBCBetLiveCasino =
                GetCommonWinLossDetailIBCBetLiveCasino(eASRptWinLossDetailIBCBetLiveCasinoCom, parameters);

            return winLossDetailIBCBetLiveCasino;
        }

        public List<WinLossDetailIBCBetLiveCasino> EAS_Rpt_WinLossDetailIBCBetLiveCasino_SAg(
            int custId,
            int masterId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            DateTime balanceUpDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds}
            };

            if (masterId != 0)
            {
                parameters.Add("masterid", masterId);
            }

            // Expected SP name: EAS_Rpt_WinLossDetailIBCBetLiveCasino_SAg
            return GetCommonWinLossDetailIBCBetLiveCasino("EAS_Rpt_WinLossDetail_SAg", parameters);
        }

        public List<WinLossDetailIBCBetLiveCasinoMAg> EAS_Rpt_WinLossDetailIBCBetLiveCasino_MAg(
            int custId,
            int agentId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            DateTime balanceUpDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds}
            };

            if (agentId != 0)
            {
                parameters.Add("agentid", agentId);
            }

            // Expected SP name: EAS_Rpt_WinLossDetailIBCBetLiveCasino_MAg
            return GetCommonWinLossDetailIBCBetLiveCasinoMAg("EAS_Rpt_WinLossDetail_MAg", parameters);
        }

        public List<WinLossDetailIBCBetLiveCasinoAg> EAS_Rpt_WinLossDetailIBCBetLiveCasino_Ag(
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            DateTime balanceUpDay)
        {
            var parameters = new Hashtable
            {
                {"custid", custId},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds}
            };

            if (memberId != 0)
            {
                parameters.Add("memberid", memberId);
            }

            // Expected SP name: EAS_Rpt_WinLossDetailIBCBetLiveCasino_Ag
            return GetCommonWinLossDetailIBCBetLiveCasinoAg("EAS_Rpt_WinLossDetail_Ag", parameters);
        }

        private List<WinLossDetailIBCBetLiveCasino> GetCommonWinLossDetailIBCBetLiveCasino(string spName, Hashtable parameters)
        {
            var entityList = new List<WinLossDetailIBCBetLiveCasino>();

            var fakeData = new WinLossDetailIBCBetLiveCasino[]
            {
                new WinLossDetailIBCBetLiveCasino
                {
                    Username = "user1",
                    BetCount=10,
                    Turnover=20000,
                    NetTurnover=20000,
                    GrossComm=20000,
                    MemberWinlost=20000,
                    PlayerComm=20000,
                    MemberTotal=20000,
                    SAgWinlost=20000,
                    SAgComm=20000,
                    SAgTotal=20000,
                    ExtWinlost=20000,
                    ExtComm=20000,
                    ExtTotal=20000,
                    NetCompany=20000,
                },
                new WinLossDetailIBCBetLiveCasino
                {
                    Username = "user2",
                    BetCount=10,
                    Turnover=20000,
                    NetTurnover=20000,
                    GrossComm=20000,
                    MemberWinlost=20000,
                    PlayerComm=20000,
                    MemberTotal=20000,
                    SAgWinlost=20000,
                    SAgComm=20000,
                    SAgTotal=20000,
                    ExtWinlost=20000,
                    ExtComm=20000,
                    ExtTotal=20000,
                    NetCompany=20000,
                },
                new WinLossDetailIBCBetLiveCasino
                {
                    Username = "user3",
                    BetCount=10,
                    Turnover=20000,
                    NetTurnover=20000,
                    GrossComm=20000,
                    MemberWinlost=20000,
                    PlayerComm=20000,
                    MemberTotal=20000,
                    SAgWinlost=20000,
                    SAgComm=20000,
                    SAgTotal=20000,
                    ExtWinlost=20000,
                    ExtComm=20000,
                    ExtTotal=20000,
                    NetCompany=20000,
                },
            };

            entityList.AddRange(fakeData);

            //using (var reader = ExecDataReader(spName, parameters))
            //{
            //    if (reader != null)
            //    {
            //        while (reader.Read())
            //        {
            //            var entity = CreateWinLossDetailIBCBetLiveCasinoEntity(reader);
            //            entityList.Add(entity);
            //        }
            //    }
            //}

            return entityList;
        }

        private static WinLossDetailIBCBetLiveCasino CreateWinLossDetailIBCBetLiveCasinoEntity(IDataReader reader) => new WinLossDetailIBCBetLiveCasino
        {
            Username = reader.To<string>(ColumnNameConstants.UserName),
            BetCount = reader.ToInt(ColumnNameConstants.BetCount),
            Turnover = reader.ToDouble(ColumnNameConstants.Turnover),
            NetTurnover = reader.ToDouble(ColumnNameConstants.NetTurnover),
            GrossComm = reader.ToDouble(ColumnNameConstants.GrossComm),
            MemberWinlost = reader.ToDouble(ColumnNameConstants.MemberWinlost),
            PlayerComm = reader.ToDouble(ColumnNameConstants.PlayerComm),
            MemberTotal = reader.ToDouble(ColumnNameConstants.MemberTotal),
            SAgWinlost = reader.ToDouble(ColumnNameConstants.SAgWinlost),
            SAgComm = reader.ToDouble(ColumnNameConstants.SAgComm),
            SAgTotal = reader.ToDouble(ColumnNameConstants.SAgTotal),
            ExtWinlost = reader.ToDouble(ColumnNameConstants.ExtWinlost),
            ExtComm = reader.ToDouble(ColumnNameConstants.ExtComm),
            ExtTotal = reader.ToDouble(ColumnNameConstants.ExtTotal),
            NetCompany = reader.ToDouble(ColumnNameConstants.NetCompany),
        };

        private List<WinLossDetailIBCBetLiveCasinoMAg> GetCommonWinLossDetailIBCBetLiveCasinoMAg(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossDetailIBCBetLiveCasinoMAg>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossDetailIBCBetLiveCasinoMAgEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static WinLossDetailIBCBetLiveCasinoMAg CreateWinLossDetailIBCBetLiveCasinoMAgEntity(IDataReader reader) => new WinLossDetailIBCBetLiveCasinoMAg
        {
            CustId = reader.ToInt("custid"),
            Username = reader.To<string>("username"),
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
            Company = reader.ToDouble("Company")
        };

        private List<WinLossDetailIBCBetLiveCasinoAg> GetCommonWinLossDetailIBCBetLiveCasinoAg(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossDetailIBCBetLiveCasinoAg>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossDetailIBCBetLiveCasinoAgEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static WinLossDetailIBCBetLiveCasinoAg CreateWinLossDetailIBCBetLiveCasinoAgEntity(IDataReader reader) => new WinLossDetailIBCBetLiveCasinoAg
        {
            CustId = reader.ToInt("custid"),
            Username = reader.To<string>("username"),
            Turnover = reader.ToDouble("Turnover"),
            NetTurnover = reader.ToDouble("TurnoverNoDraw"),
            GrossComm = reader.ToDouble("GrossComm"),
            MemberWinlost = reader.ToDouble("MemberWinlost"),
            PlayerComm = reader.ToDouble("PlayerComm"),
            MemberTotal = reader.ToDouble("MemberTotal"),
            AgWinlost = reader.ToDouble("AgWinlost"),
            AgComm = reader.ToDouble("AgComm"),
            AgTotal = reader.ToDouble("AgTotal"),
            Company = reader.ToDouble("Company")
        };
    }

    static class ParameterNameConstants
    {
        public const string CustomerId = "custid";
        public const string GroupName = "groupname";
        public const string FromDate = "fromdate";
        public const string ToDate = "todate";
        public const string BalanceUpDay = "balanceupday";
        public const string ProductIds = "productids";
    }

    static class ColumnNameConstants
    {
        public const string UserName = "Username";
        public const string BetCount = "BetCount";
        public const string Turnover = "Turnover";
        public const string NetTurnover = "TurnoverNoDraw";
        public const string GrossComm = "GrossComm";
        public const string MemberWinlost = "MemberWinlost";
        public const string PlayerComm = "PlayerComm";
        public const string MemberTotal = "MemberTotal";
        public const string SAgWinlost = "SAgWinlost";
        public const string SAgComm = "SAgComm";
        public const string SAgTotal = "SAgTotal";
        public const string ExtWinlost = "ExtWinloss";
        public const string ExtComm = "ExtComm";
        public const string ExtTotal = "ExtTotal";
        public const string NetCompany = "NetCompany";
    }
}
