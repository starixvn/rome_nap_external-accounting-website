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
        public List<WinLossDetailIBCBetLiveCasino> EAS_Rpt_WinLossDetailIBCBetLiveCasino_Com(
            string groupname,
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            DateTime balanceUpDay)
        {
            var parameters = new Hashtable
            {
#pragma warning disable S1192 // String literals should not be duplicated
                {"groupname", groupname},
                {"fromdate", fromDate},
                {"todate", toDate},
                {"balanceupday", balanceUpDay},
                {"productids", productIds}
#pragma warning restore S1192 // String literals should not be duplicated
            };

            if (custId != 0)
            {
                parameters.Add("custid", custId);
            }

            // Expected SP name: EAS_Rpt_WinLossDetailIBCBetLiveCasino_Com
            return GetCommonWinLossDetailIBCBetLiveCasino("EAS_Rpt_WinLossDetail_Com", parameters);
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

        private List<WinLossDetailIBCBetLiveCasino> GetCommonWinLossDetailIBCBetLiveCasino(string spname, Hashtable parameters)
        {
            var entityList = new List<WinLossDetailIBCBetLiveCasino>();

            using (var reader = ExecDataReader(spname, parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var entity = CreateWinLossDetailIBCBetLiveCasinoEntity(reader);
                        entityList.Add(entity);
                    }
                }
            }

            return entityList;
        }

        private static WinLossDetailIBCBetLiveCasino CreateWinLossDetailIBCBetLiveCasinoEntity(IDataReader reader) => new WinLossDetailIBCBetLiveCasino
        {
            CustId = reader.ToInt("custid"),
            Username = reader.To<string>("username"),
            Turnover = reader.ToDouble("Turnover"),
            NetTurnover = reader.ToDouble("TurnoverNoDraw"),
            GrossComm = reader.ToDouble("GrossComm"),
            MemberWinlost = reader.ToDouble("MemberWinlost"),
            PlayerComm = reader.ToDouble("PlayerComm"),
            MemberTotal = reader.ToDouble("MemberTotal"),
#pragma warning disable S1192 // String literals should not be duplicated
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
#pragma warning restore S1192 // String literals should not be duplicated
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
}
